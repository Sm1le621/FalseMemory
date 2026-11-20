using UnityEngine;

// Тригер дверей або печери. Коли гравець входить у нього,
// запускається перехід в іншу сцену.
// Вхід у печеру (SampleScene -> Dungeon) і вихід з неї (Dungeon -> SampleScene)
// використовують цей самий скрипт, різняться тільки налаштування в Inspector.
// Сам перехід і перенесення гравця робить SceneTransitionManager.
public class SceneTransition : MonoBehaviour
{
    // Ім'я сцени, яку треба завантажити (Dungeon для входу, SampleScene для виходу)
    [SerializeField] private string targetSceneName = "Dungeon";

    [Header("Spawn")]
    // Чи ставити гравця в задану точку нової сцени
    [SerializeField] private bool useSpawnPosition = true;
    // Точка появи гравця в новій сцені.
    // Вона має бути ПОЗА тригером переходу в тій сцені, інакше гравець одразу піде назад
    [SerializeField] private Vector2 spawnPosition = Vector2.zero;

    // Ім'я цільової сцени (тільки читання, наприклад для тестів)
    public string TargetSceneName
    {
        get { return targetSceneName; }
    }

    private void Awake()
    {
        // Якщо поле залишилось порожнім, використовуємо Dungeon
        if (string.IsNullOrEmpty(targetSceneName))
        {
            targetSceneName = "Dungeon";
            Debug.LogWarning("SceneTransition: target scene was empty. Using 'Dungeon'.", this);
        }
    }

    // Unity викликає цей метод, коли щось входить у тригер
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Переходимо тільки тоді, коли входить гравець
        if (!other.CompareTag("Player"))
        {
            return;
        }

        // Під час бою або вже початого переходу нічого не робимо
        BattleTransition battle = BattleTransition.Instance;
        if (battle != null)
        {
            if (battle.IsTransitioning)
            {
                return;
            }
            if (battle.EncounterActive)
            {
                return;
            }
        }

        SceneTransitionManager manager = SceneTransitionManager.GetOrCreate();

        // Один перехід за раз
        if (manager.IsTransitioning)
        {
            return;
        }

        // Гравець щойно прибув - не відправляємо його одразу назад
        if (manager.IsArrivalProtected)
        {
            return;
        }

        bool started = manager.ChangeScene(targetSceneName, useSpawnPosition, spawnPosition);
        if (!started)
        {
            Debug.LogWarning("SceneTransition: could not start transition to '" + targetSceneName + "'.", this);
        }
    }
}
