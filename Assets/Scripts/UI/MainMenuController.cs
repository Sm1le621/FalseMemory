using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Головне меню: кнопки Start, Settings, Exit і панель налаштувань
// (гучність і повноекранний режим). Методи викликаються з кнопок (OnClick в інспекторі).
// Налаштування зберігаються в PlayerPrefs (залишаються після перезапуску гри).
public class MainMenuController : MonoBehaviour
{
    // Ключі, під якими зберігаються налаштування
    private const string VolumePrefKey = "FalseMemory.Volume";
    private const string FullscreenPrefKey = "FalseMemory.Fullscreen";

    // Яку сцену завантажити при натисканні Start
    [SerializeField] private string gameSceneName = "SampleScene";

    [Header("Settings panel")]
    // Панель налаштувань
    [SerializeField] private GameObject settingsRoot;
    [SerializeField] private Slider volumeSlider;
    // Текст "Fullscreen: On/Off"
    [SerializeField] private TextMeshProUGUI fullscreenLabel;

    private bool fullscreen;

    private void Start()
    {
        HideSettings();
        LoadSettings();
    }

    private void OnEnable()
    {
        RefreshFullscreenLabel();
    }

    // ===== Кнопки головного меню =====

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
        if (settingsRoot == null)
        {
            Debug.LogError("Settings panel is not assigned.");
            return;
        }

        settingsRoot.SetActive(true);
        LoadSettings();
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

    // ===== Панель налаштувань =====

    // Кнопка Back - сховати налаштування
    public void HideSettings()
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }
    }

    // Викликається слайдером гучності
    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(VolumePrefKey, AudioListener.volume);
        PlayerPrefs.Save();
    }

    // Кнопка "Fullscreen"
    public void ToggleFullscreen()
    {
        fullscreen = !fullscreen;
        Screen.fullScreen = fullscreen;

        int value = 0;
        if (fullscreen)
        {
            value = 1;
        }
        PlayerPrefs.SetInt(FullscreenPrefKey, value);
        PlayerPrefs.Save();

        RefreshFullscreenLabel();
    }

    // Завантажити збережені налаштування і застосувати їх
    private void LoadSettings()
    {
        // Гучність (за замовчуванням 1 = 100%)
        float volume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        AudioListener.volume = Mathf.Clamp01(volume);
        if (volumeSlider != null)
        {
            // WithoutNotify - щоб не викликати OnVolumeChanged
            volumeSlider.SetValueWithoutNotify(AudioListener.volume);
        }

        // Повноекранний режим (за замовчуванням - як зараз)
        int defaultFullscreen = 0;
        if (Screen.fullScreen)
        {
            defaultFullscreen = 1;
        }
        fullscreen = PlayerPrefs.GetInt(FullscreenPrefKey, defaultFullscreen) == 1;

        Screen.fullScreen = fullscreen;
        RefreshFullscreenLabel();
    }

    private void RefreshFullscreenLabel()
    {
        if (fullscreenLabel == null)
        {
            return;
        }

        if (fullscreen)
        {
            fullscreenLabel.text = "Fullscreen: On";
        }
        else
        {
            fullscreenLabel.text = "Fullscreen: Off";
        }
    }
}
