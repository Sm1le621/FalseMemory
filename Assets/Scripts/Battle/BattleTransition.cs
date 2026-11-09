using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Перехід між світом і боєм. Один такий об'єкт живе у сцені світу.
// Сцена бою завантажується ДОДАТКОВО (Additive), тому справжні об'єкти світу
// нікуди не зникають: їх позиції і Health залишаються як були.
// Ніяких копій і "збережених" HP тут немає.
public sealed class BattleTransition : MonoBehaviour
{
    // Єдиний екземпляр (щоб інші скрипти могли знайти його без пошуку)
    public static BattleTransition Instance { get; private set; }

    public BattleRules rules;
    public string battleSceneName = "BattleScene";
    public string mainMenuSceneName = "MainMenu";

    public BattleUnit Player { get; private set; }
    public BattleUnit Enemy { get; private set; }
    // Чи йде зараз бій
    public bool EncounterActive { get; private set; }
    // Чи йде зараз затемнення / завантаження
    public bool IsTransitioning { get; private set; }

    // Сцена світу
    private Scene worldScene;
    // Об'єкти світу, які ми сховали на час бою
    private readonly List<GameObject> hiddenRoots = new List<GameObject>();
    // Компоненти, які ми вимкнули на час бою, і чи були вони увімкнені до цього
    private readonly Dictionary<Behaviour, bool> pausedBehaviours = new Dictionary<Behaviour, bool>();
    // Чорна картинка для затемнення
    [SerializeField] private Image fade;

    // До якого часу після втечі ворог не може знову почати бій
    private float protectedUntil;
    // AI ворога, який ми вимкнули після втечі
    private EnemyAI protectedEnemyAI;
    private bool protectedEnemyAIWasEnabled;

    // Чи діє зараз захист після втечі
    public bool IsEncounterProtected
    {
        get { return Time.realtimeSinceStartup < protectedUntil; }
    }

    private void LateUpdate()
    {
        ReleaseProtectionIfExpired();
    }

    // Якщо час захисту минув - вмикаємо AI ворога назад
    private void ReleaseProtectionIfExpired()
    {
        if (IsEncounterProtected || protectedEnemyAI == null)
        {
            return;
        }

        protectedEnemyAI.enabled = protectedEnemyAIWasEnabled;
        protectedEnemyAI = null;
    }

    // Unity викликає це при старті гри.
    // Скидаємо static змінну (інакше в редакторі вона може лишитись зі старого запуску)
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        Instance = null;
    }

    private void Awake()
    {
        // Якщо такий об'єкт уже є - цей вимикаємо
        if (Instance != null && Instance != this)
        {
            enabled = false;
            return;
        }
        Instance = this;
    }

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    // Спробувати почати бій між player і enemy
    public bool TryBegin(BattleUnit player, BattleUnit enemy)
    {
        ReleaseProtectionIfExpired();

        // Перевіряємо, чи можна зараз почати бій
        if (!Application.isPlaying)
        {
            return false;
        }
        if (EncounterActive || IsTransitioning || IsEncounterProtected)
        {
            return false;
        }
        if (rules == null)
        {
            return false;
        }
        if (player == null || enemy == null || player == enemy)
        {
            return false;
        }
        if (!player.IsAlive || !enemy.IsAlive)
        {
            return false;
        }

        // Сцена бою має бути додана в Build Settings
        if (!Application.CanStreamedLevelBeLoaded(battleSceneName))
        {
            Debug.LogError("BattleScene is missing from the build settings.", this);
            return false;
        }

        // Всі мають бути в одній сцені
        if (player.gameObject.scene != enemy.gameObject.scene || gameObject.scene != player.gameObject.scene)
        {
            return false;
        }

        // Ставимо одразу (не в корутині), щоб повторне зіткнення не почало ще один бій
        EncounterActive = true;
        IsTransitioning = true;
        Player = player;
        Enemy = enemy;
        worldScene = player.gameObject.scene;

        player.ResetBattleState();
        enemy.ResetBattleState();

        // Вимикаємо керування і AI на час бою
        Pause(player.GetComponent<PlayerController>());
        Pause(enemy.GetComponent<EnemyAI>());

        StopBody(player);
        StopBody(enemy);

        StartCoroutine(EnterBattle());
        return true;
    }

    // Вимкнути компонент і запам'ятати, чи був він увімкнений
    private void Pause(Behaviour behaviour)
    {
        if (behaviour == null)
        {
            return;
        }

        pausedBehaviours[behaviour] = behaviour.enabled;
        behaviour.enabled = false;
    }

    // Зупинити фізичне тіло
    private static void StopBody(BattleUnit unit)
    {
        Rigidbody2D body = unit.GetComponent<Rigidbody2D>();
        if (body != null)
        {
            body.linearVelocity = Vector2.zero;
            body.angularVelocity = 0f;
        }
    }

    // Перехід у бій: затемнення -> ховаємо світ -> завантажуємо бій -> прибираємо затемнення
    private IEnumerator EnterBattle()
    {
        yield return FadeTo(1f);

        // Ховаємо всі об'єкти світу (крім себе і вже вимкнених)
        hiddenRoots.Clear();
        foreach (GameObject root in worldScene.GetRootGameObjects())
        {
            if (root == transform.root.gameObject || !root.activeSelf)
            {
                continue;
            }
            hiddenRoots.Add(root);
            root.SetActive(false);
        }

        // Завантажуємо сцену бою поверх світу
        AsyncOperation load = null;
        try
        {
            load = SceneManager.LoadSceneAsync(battleSceneName, LoadSceneMode.Additive);
        }
        catch (System.Exception exception)
        {
            Debug.LogException(exception, this);
        }

        // Якщо не вийшло - повертаємо світ як було
        if (load == null)
        {
            RestoreWorld(false);
            yield return FadeTo(0f);
            EncounterActive = false;
            IsTransitioning = false;
            yield break;
        }

        yield return load;

        Scene battleScene = SceneManager.GetSceneByName(battleSceneName);
        SceneManager.SetActiveScene(battleScene);

        yield return FadeTo(0f);
        IsTransitioning = false;
    }

    // Повернутись у світ після перемоги
    public bool ReturnAfterVictory()
    {
        if (!EncounterActive || IsTransitioning)
        {
            return false;
        }
        // Гравець має бути живий, а ворог - мертвий
        if (Player == null || !Player.IsAlive || Enemy == null || Enemy.IsAlive)
        {
            return false;
        }

        IsTransitioning = true;
        StartCoroutine(ReturnToWorld(true));
        return true;
    }

    // Повернутись у світ після втечі
    public bool ReturnAfterEscape()
    {
        if (!EncounterActive || IsTransitioning)
        {
            return false;
        }
        // Обидва мають бути живі
        if (Player == null || !Player.IsAlive || Enemy == null || !Enemy.IsAlive)
        {
            return false;
        }

        IsTransitioning = true;
        StartCoroutine(ReturnToWorld(false));
        return true;
    }

    private IEnumerator ReturnToWorld(bool victory)
    {
        yield return FadeTo(1f);

        // Вивантажуємо сцену бою
        SceneManager.SetActiveScene(worldScene);
        Scene battleScene = SceneManager.GetSceneByName(battleSceneName);
        if (battleScene.IsValid() && battleScene.isLoaded)
        {
            yield return SceneManager.UnloadSceneAsync(battleScene);
        }

        RestoreWorld(victory);

        // Якщо це була втеча - вмикаємо захист і відсуваємо гравця від ворога
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

        EncounterActive = false;
        IsTransitioning = false;
        Player = null;
        Enemy = null;
    }

    // Шукаємо вільне місце поруч, враховуючи розмір колайдера гравця.
    // Якщо безпечного місця немає - гравець лишається де був
    // (захист від повторного бою все одно дає час відійти).
    private void PlacePlayerSafely()
    {
        Physics2D.SyncTransforms();

        Collider2D collider = Player.GetComponent<Collider2D>();
        if (collider == null)
        {
            return;
        }

        Vector2 origin = Player.transform.position;
        Vector2 enemyPosition = Enemy.transform.position;
        // Напрямок "від ворога"
        Vector2 away = origin - enemyPosition;
        float distance = Mathf.Max(0.1f, rules.fleeSafeDistance);
        // Зсув центру колайдера від позиції гравця
        Vector2 offset = (Vector2)collider.bounds.center - origin;
        Vector2 size = collider.bounds.size;
        // З якими шарами гравець стикається
        int mask = Physics2D.GetLayerCollisionMask(Player.gameObject.layer);

        // Якщо гравець і так далеко і місце вільне - нічого не робимо
        if (away.magnitude >= distance && IsFree(origin + offset, size, mask))
        {
            return;
        }

        // Якщо гравець стоїть прямо на ворогу - напрямок вибираємо вліво
        if (away.sqrMagnitude < 0.001f)
        {
            away = Vector2.left;
        }
        away.Normalize();

        // Перебираємо 8 напрямків на двох відстанях
        for (int radius = 1; radius <= 2; radius++)
        {
            for (int direction = 0; direction < 8; direction++)
            {
                // Кут: 0, 45, 90 ... градусів (в радіанах)
                float angle = direction * Mathf.PI / 4f;

                // Повертаємо вектор away на цей кут
                float rotatedX = away.x * Mathf.Cos(angle) - away.y * Mathf.Sin(angle);
                float rotatedY = away.x * Mathf.Sin(angle) + away.y * Mathf.Cos(angle);
                Vector2 rotated = new Vector2(rotatedX, rotatedY);

                Vector2 candidate = enemyPosition + rotated * distance * radius;
                if (!IsFree(candidate + offset, size, mask))
                {
                    continue;
                }

                // Знайшли вільне місце - переносимо гравця
                Rigidbody2D body = Player.GetComponent<Rigidbody2D>();
                if (body != null)
                {
                    body.position = candidate;
                }
                Player.transform.position = new Vector3(candidate.x, candidate.y, Player.transform.position.z);
                StopBody(Player);
                Physics2D.SyncTransforms();
                return;
            }
        }
    }

    // Чи вільна прямокутна область (немає твердих колайдерів, крім самого гравця)
    private bool IsFree(Vector2 center, Vector2 size, int mask)
    {
        Collider2D[] hits = Physics2D.OverlapBoxAll(center, size, 0f, mask);
        foreach (Collider2D other in hits)
        {
            if (!other.isTrigger && other.gameObject != Player.gameObject && !other.transform.IsChildOf(Player.transform))
            {
                return false;
            }
        }
        return true;
    }

    // Показати світ назад і ввімкнути компоненти
    private void RestoreWorld(bool victory)
    {
        // Переможеного скелета вимикаємо ДО того, як фізика світу знову запрацює
        if (victory && Enemy != null)
        {
            Enemy.gameObject.SetActive(false);
        }

        foreach (GameObject root in hiddenRoots)
        {
            if (root == null)
            {
                continue;
            }
            // Переможеного ворога назад не вмикаємо
            if (victory && Enemy != null && root == Enemy.gameObject)
            {
                continue;
            }
            root.SetActive(true);
        }
        hiddenRoots.Clear();

        // Повертаємо компонентам їх старий стан enabled
        foreach (KeyValuePair<Behaviour, bool> entry in pausedBehaviours)
        {
            if (entry.Key != null)
            {
                entry.Key.enabled = entry.Value;
            }
        }
        pausedBehaviours.Clear();

        if (Player != null)
        {
            Player.ResetBattleState();
        }
        if (Enemy != null)
        {
            Enemy.ResetBattleState();
        }
    }

    // Почати гру заново (перезавантажити сцену світу)
    public void RestartGame()
    {
        LeaveFor(worldScene.path);
    }

    // Вийти в головне меню
    public void ReturnToMainMenu()
    {
        LeaveFor(mainMenuSceneName);
    }

    private void LeaveFor(string scene)
    {
        if (IsTransitioning || !Application.CanStreamedLevelBeLoaded(scene))
        {
            return;
        }

        IsTransitioning = true;
        StartCoroutine(LoadFresh(scene));
    }

    private IEnumerator LoadFresh(string scene)
    {
        yield return FadeTo(1f);

        // Режим Single навмисно скидає всю поточну сесію (тільки для рестарту/меню)
        GameObject overlay = fade.transform.parent.gameObject;
        BattleScreenFade reveal = overlay.GetComponent<BattleScreenFade>();
        if (reveal == null)
        {
            Debug.LogError("BattleTransition: BattleScreenFade is missing from the fade overlay.", this);
            yield break;
        }

        reveal.panel = fade;
        reveal.LoadAndReveal(scene, rules.fadeDuration);
    }

    // Затемнити (1) або прибрати затемнення (0)
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
