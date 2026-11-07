using UnityEngine;
using UnityEngine.SceneManagement;

// Кнопки головного меню: Start, Settings, Exit.
// Методи викликаються з кнопок (OnClick в інспекторі).
public class MainMenuController : MonoBehaviour
{
    // Яку сцену завантажити при натисканні Start
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private SettingsMenu settingsMenu;

    // Кнопка Start
    public void OnStart()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("Game scene name is not set.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    // Кнопка Settings
    public void OnSettings()
    {
        if (settingsMenu == null)
        {
            Debug.LogError("Settings panel is not assigned.");
            return;
        }

        settingsMenu.Show();
    }

    // Кнопка Exit
    public void OnExit()
    {
        // #if - це умова для компілятора.
        // У редакторі зупиняємо Play Mode, а в зібраній грі - закриваємо програму
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
