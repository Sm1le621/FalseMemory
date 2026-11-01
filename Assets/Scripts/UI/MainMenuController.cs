using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private string gameSceneName = "SampleScene";
    [SerializeField] private SettingsMenu settingsMenu;

    public void OnStart()
    {
        if (string.IsNullOrWhiteSpace(gameSceneName))
        {
            Debug.LogError("Game scene name is not set.");
            return;
        }

        SceneManager.LoadScene(gameSceneName);
    }

    public void OnSettings()
    {
        if (settingsMenu == null)
        {
            Debug.LogError("Settings panel is not assigned.");
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
