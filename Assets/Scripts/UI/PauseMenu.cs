using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

// Меню паузи у світі: Esc ставить гру на паузу і показує панель з кнопками Resume та Main Menu.
// Кнопки викликають Resume() та OpenMainMenu() через OnClick в Inspector.
// Виконується раніше за InventoryWindow, щоб Esc, який закриває інвентар, не відкривав ще й паузу.
[DefaultExecutionOrder(-100)]
public sealed class PauseMenu : MonoBehaviour
{
    // Напівпрозорий фон з кнопками (вимкнений, поки гра не на паузі)
    [SerializeField] private GameObject panel;
    // Вікно інвентаря гравця: поки воно відкрите, пауза не вмикається
    [SerializeField] private InventoryWindow inventory;
    // Замок руху гравця: на паузі гравець не реагує на клавіші
    [SerializeField] private WorldInputGate inputGate;
    // Сцена головного меню
    [SerializeField] private string mainMenuSceneName = "MainMenu";

    // true - гра зараз на паузі
    public bool IsPaused { get; private set; }

    private void Awake()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null || !keyboard.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (IsPaused)
        {
            Resume();
        }
        else if (CanPause())
        {
            Pause();
        }
    }

    // Чи можна зараз поставити паузу
    private bool CanPause()
    {
        // Esc у відкритому інвентарі закриває інвентар, а не відкриває паузу
        if (inventory != null && inventory.IsOpen)
        {
            return false;
        }

        // Під час бою або переходу в бій пауза не потрібна
        BattleTransition battle = BattleTransition.Instance;
        if (battle != null && (battle.EncounterActive || battle.IsTransitioning))
        {
            return false;
        }

        // Під час завантаження іншої сцени теж
        SceneTransitionManager scenes = SceneTransitionManager.Instance;
        if (scenes != null && scenes.IsTransitioning)
        {
            return false;
        }

        return true;
    }

    // Поставити гру на паузу
    public void Pause()
    {
        IsPaused = true;
        // timeScale = 0 зупиняє фізику, рух ворогів і анімації
        Time.timeScale = 0f;
        panel.SetActive(true);

        if (inventory != null)
        {
            // Вимкнений компонент не отримує Update, тому клавіша I не спрацює
            inventory.enabled = false;
        }
        if (inputGate != null)
        {
            inputGate.Acquire(this);
        }
    }

    // Зняти паузу (кнопка Resume або Esc)
    public void Resume()
    {
        IsPaused = false;
        Time.timeScale = 1f;
        panel.SetActive(false);

        if (inventory != null)
        {
            inventory.enabled = true;
        }
        if (inputGate != null)
        {
            inputGate.Release(this);
        }
    }

    // Вийти в головне меню (кнопка Main Menu)
    public void OpenMainMenu()
    {
        // Без цього в головному меню час залишився б зупиненим
        Time.timeScale = 1f;
        SceneManager.LoadScene(mainMenuSceneName);
    }

    private void OnDestroy()
    {
        // Якщо об'єкт знищено прямо на паузі - повертаємо нормальний час
        if (IsPaused)
        {
            Time.timeScale = 1f;
        }
    }
}
