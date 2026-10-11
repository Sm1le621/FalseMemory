using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// One coordinator in the exploration scene; no persistent copies or HP snapshots.
// Additive loading preserves the actual world objects, their positions and Health.
public sealed class BattleTransition : MonoBehaviour
{
    public static BattleTransition Instance { get; private set; }
    public BattleRules rules;
    public string battleSceneName = "BattleScene";
    public string mainMenuSceneName = "MainMenu";
    public BattleUnit Player { get; private set; }
    public BattleUnit Enemy { get; private set; }
    public bool EncounterActive { get; private set; }
    public bool IsTransitioning { get; private set; }

    private Scene worldScene;
    private readonly List<GameObject> hiddenRoots = new List<GameObject>();
    private readonly Dictionary<Behaviour, bool> pausedBehaviours = new Dictionary<Behaviour, bool>();
    private Image fade;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics() => Instance = null;

    private void Awake()
    {
        if (Instance != null && Instance != this) { enabled = false; return; }
        Instance = this;
    }

    private void OnDestroy() { if (Instance == this) Instance = null; }

    public bool TryBegin(BattleUnit player, BattleUnit enemy)
    {
        if (!Application.isPlaying || EncounterActive || IsTransitioning || rules == null ||
            player == null || enemy == null || player == enemy || !player.IsAlive || !enemy.IsAlive) return false;
        if (!Application.CanStreamedLevelBeLoaded(battleSceneName))
        {
            Debug.LogError("BattleScene отсутствует в списке сцен сборки.", this);
            return false;
        }
        if (player.gameObject.scene != enemy.gameObject.scene || gameObject.scene != player.gameObject.scene) return false;
        EncounterActive = true; // Set synchronously, before any coroutine/collision can repeat.
        IsTransitioning = true;
        Player = player;
        Enemy = enemy;
        worldScene = player.gameObject.scene;
        player.ResetBattleState(); enemy.ResetBattleState();
        Pause(player.GetComponent<PlayerController>());
        Pause(enemy.GetComponent<EnemyAI>());
        Pause(enemy.GetComponent<EnemyAttack>());
        StopBody(player); StopBody(enemy);
        StartCoroutine(EnterBattle());
        return true;
    }

    private void Pause(Behaviour behaviour)
    {
        if (behaviour == null) return;
        pausedBehaviours[behaviour] = behaviour.enabled;
        behaviour.enabled = false;
    }

    private static void StopBody(BattleUnit unit)
    {
        var body = unit.GetComponent<Rigidbody2D>();
        if (body != null) { body.linearVelocity = Vector2.zero; body.angularVelocity = 0f; }
    }

    private IEnumerator EnterBattle()
    {
        yield return FadeTo(1f);
        hiddenRoots.Clear();
        foreach (var root in worldScene.GetRootGameObjects())
        {
            if (root == transform.root.gameObject || !root.activeSelf) continue;
            hiddenRoots.Add(root);
            root.SetActive(false);
        }
        AsyncOperation load = null;
        try { load = SceneManager.LoadSceneAsync(battleSceneName, LoadSceneMode.Additive); }
        catch (System.Exception exception) { Debug.LogException(exception, this); }
        if (load == null)
        {
            RestoreWorld(false);
            yield return FadeTo(0f);
            EncounterActive = IsTransitioning = false;
            yield break;
        }
        yield return load;
        Scene battleScene = SceneManager.GetSceneByName(battleSceneName);
        SceneManager.SetActiveScene(battleScene);
        yield return FadeTo(0f);
        IsTransitioning = false;
    }

    public bool ReturnAfterVictory()
    {
        if (!EncounterActive || IsTransitioning || Enemy == null || Enemy.IsAlive) return false;
        IsTransitioning = true;
        StartCoroutine(ReturnToWorld());
        return true;
    }

    private IEnumerator ReturnToWorld()
    {
        yield return FadeTo(1f);
        SceneManager.SetActiveScene(worldScene);
        var battleScene = SceneManager.GetSceneByName(battleSceneName);
        if (battleScene.IsValid() && battleScene.isLoaded) yield return SceneManager.UnloadSceneAsync(battleScene);
        RestoreWorld(true);
        yield return FadeTo(0f);
        EncounterActive = IsTransitioning = false;
        Player = Enemy = null;
    }

    private void RestoreWorld(bool victory)
    {
        // The defeated skeleton is disabled before any world physics resumes.
        if (victory && Enemy != null) Enemy.gameObject.SetActive(false);
        foreach (var root in hiddenRoots)
            if (root != null && !(victory && Enemy != null && root == Enemy.gameObject)) root.SetActive(true);
        hiddenRoots.Clear();
        foreach (var entry in pausedBehaviours)
            if (entry.Key != null) entry.Key.enabled = entry.Value;
        pausedBehaviours.Clear();
        if (Player != null) Player.ResetBattleState();
        if (Enemy != null) Enemy.ResetBattleState();
    }

    public void RestartGame() => LeaveFor(worldScene.path);
    public void ReturnToMainMenu() => LeaveFor(mainMenuSceneName);

    private void LeaveFor(string scene)
    {
        if (IsTransitioning || !Application.CanStreamedLevelBeLoaded(scene)) return;
        IsTransitioning = true;
        StartCoroutine(LoadFresh(scene));
    }

    private IEnumerator LoadFresh(string scene)
    {
        yield return FadeTo(1f);
        // Single mode intentionally discards this session only for restart/menu.
        SceneManager.LoadSceneAsync(scene, LoadSceneMode.Single);
    }

    private IEnumerator FadeTo(float alpha)
    {
        if (fade == null)
        {
            var overlay = new GameObject("BattleFade", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler));
            overlay.transform.SetParent(transform, false);
            var canvas = overlay.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 1000;
            var panel = new GameObject("Fade", typeof(RectTransform), typeof(Image));
            panel.transform.SetParent(overlay.transform, false);
            fade = panel.GetComponent<Image>(); fade.raycastTarget = false;
            var rect = panel.GetComponent<RectTransform>();
            rect.anchorMin = Vector2.zero; rect.anchorMax = Vector2.one;
            rect.offsetMin = rect.offsetMax = Vector2.zero;
            fade.color = new Color(0f, 0f, 0f, 0f);
        }
        float start = fade.color.a;
        float duration = Mathf.Max(0f, rules.fadeDuration);
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            fade.color = new Color(0f, 0f, 0f, Mathf.Lerp(start, alpha, elapsed / duration));
            yield return null;
        }
        fade.color = new Color(0f, 0f, 0f, alpha);
    }
}
