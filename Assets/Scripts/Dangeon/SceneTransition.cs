using UnityEngine;

// Тригер дверей або печери. Коли гравець входить у нього,
// запускається перехід в іншу сцену.
// Сам перехід і перенесення гравця робить SceneTransitionManager.
public class SceneTransition : MonoBehaviour
{
    // Ім'я сцени, яку треба завантажити
    [SerializeField] private string targetSceneName = "Dungeon";

    [Header("Spawn")]
    // Чи ставити гравця в задану точку нової сцени
    [SerializeField] private bool useSpawnPosition = true;
    // Точка появи гравця в новій сцені
    [SerializeField] private Vector2 spawnPosition = Vector2.zero;

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

        bool started = manager.ChangeScene(targetSceneName, useSpawnPosition, spawnPosition);
        if (!started)
        {
            Debug.LogWarning("SceneTransition: could not start transition to '" + targetSceneName + "'.", this);
        }
    }
}
