using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private SettingsMenu settingsMenu;

    public void Initialize(string sceneName, SettingsMenu menu)
    {
        gameSceneName = sceneName;
        settingsMenu = menu;
    }

    public void OnStart()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("Не задано имя игровой сцены.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettings()
    {
        if (settingsMenu == null)
        {
            Debug.LogError("Панель настроек не подключена.");
            return;
        }

        settingsMenu.Show();
    }

    public void OnExit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
