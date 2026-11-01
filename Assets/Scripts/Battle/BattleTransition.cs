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
    [SerializeField] private Image fade;
    private float protectedUntil;
    private EnemyAI protectedEnemyAI;
    private bool protectedEnemyAIWasEnabled;
    public bool IsEncounterProtected => Time.realtimeSinceStartup < protectedUntil;

    private void LateUpdate() => ReleaseProtectionIfExpired();

    private void ReleaseProtectionIfExpired()
    {
        if (IsEncounterProtected || protectedEnemyAI == null) return;
        protectedEnemyAI.enabled = protectedEnemyAIWasEnabled;
        protectedEnemyAI = null;
    }

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
        ReleaseProtectionIfExpired();
        if (!Application.isPlaying || EncounterActive || IsTransitioning || IsEncounterProtected || rules == null ||
            player == null || enemy == null || player == enemy || !player.IsAlive || !enemy.IsAlive) return false;
        if (!Application.CanStreamedLevelBeLoaded(battleSceneName))
        {
            Debug.LogError("BattleScene is missing from the build settings.", this);
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
        if (!EncounterActive || IsTransitioning || Player == null || !Player.IsAlive || Enemy == null || Enemy.IsAlive) return false;
        IsTransitioning = true;
        StartCoroutine(ReturnToWorld(true));
        return true;
    }

    public bool ReturnAfterEscape()
    {
        if (!EncounterActive || IsTransitioning || Player == null || !Player.IsAlive || Enemy == null || !Enemy.IsAlive) return false;
        IsTransitioning = true;
        StartCoroutine(ReturnToWorld(false));
        return true;
    }

    private IEnumerator ReturnToWorld(bool victory)
    {
        yield return FadeTo(1f);
        SceneManager.SetActiveScene(worldScene);
        var battleScene = SceneManager.GetSceneByName(battleSceneName);
        if (battleScene.IsValid() && battleScene.isLoaded) yield return SceneManager.UnloadSceneAsync(battleScene);
        RestoreWorld(victory);
        if (!victory)
        {
            protectedUntil = Time.realtimeSinceStartup + Mathf.Max(0.1f, rules.fleeProtectionSeconds);
            protectedEnemyAI = Enemy.GetComponent<EnemyAI>();
            if (protectedEnemyAI != null)
            {
                protectedEnemyAIWasEnabled = protectedEnemyAI.enabled;
                protectedEnemyAI.enabled = false;
            }
            StopBody(Enemy);
            PlacePlayerSafely();
        }
        yield return FadeTo(0f);
        EncounterActive = IsTransitioning = false;
        Player = Enemy = null;
    }

    // Search nearby free space using the player's collision bounds. If no safe
    // point exists, retain the original position; protection still allows retreat.
    private void PlacePlayerSafely()
    {
        Physics2D.SyncTransforms();
        var collider = Player.GetComponent<Collider2D>();
        if (collider == null) return;
        Vector2 origin = Player.transform.position;
        Vector2 enemyPosition = Enemy.transform.position;
        Vector2 away = origin - enemyPosition;
        float distance = Mathf.Max(0.1f, rules.fleeSafeDistance);
        Vector2 offset = (Vector2)collider.bounds.center - origin;
        Vector2 size = collider.bounds.size;
        int mask = Physics2D.GetLayerCollisionMask(Player.gameObject.layer);
        if (away.magnitude >= distance && IsFree(origin + offset, size, mask)) return;
        if (away.sqrMagnitude < 0.001f) away = Vector2.left;
        away.Normalize();
        for (int radius = 1; radius <= 2; radius++)
        {
            for (int direction = 0; direction < 8; direction++)
            {
                float angle = direction * Mathf.PI / 4f;
                var rotated = new Vector2(away.x * Mathf.Cos(angle) - away.y * Mathf.Sin(angle),
                    away.x * Mathf.Sin(angle) + away.y * Mathf.Cos(angle));
                Vector2 candidate = enemyPosition + rotated * distance * radius;
                if (!IsFree(candidate + offset, size, mask)) continue;
                var body = Player.GetComponent<Rigidbody2D>();
                if (body != null) body.position = candidate;
                Player.transform.position = new Vector3(candidate.x, candidate.y, Player.transform.position.z);
                StopBody(Player); Physics2D.SyncTransforms();
                return;
            }
        }
    }

    private bool IsFree(Vector2 center, Vector2 size, int mask)
    {
        foreach (var other in Physics2D.OverlapBoxAll(center, size, 0f, mask))
            if (!other.isTrigger && other.gameObject != Player.gameObject && !other.transform.IsChildOf(Player.transform)) return false;
        return true;
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
        var overlay = fade.transform.parent.gameObject;
        var reveal = overlay.GetComponent<BattleScreenFade>();
        if (reveal == null)
        {
            Debug.LogError("BattleTransition: BattleScreenFade is missing from the fade overlay.", this);
            yield break;
        }
        reveal.panel = fade;
        reveal.LoadAndReveal(scene, rules.fadeDuration);
    }

    private IEnumerator FadeTo(float alpha)
    {
        if (fade == null)
        {
            Debug.LogError("BattleTransition: fade overlay is not assigned in the scene.", this);
            yield break;
        }
        yield return BattleScreenFade.Fade(fade, alpha, rules.fadeDuration);
    }
}
