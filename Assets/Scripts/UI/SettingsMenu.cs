using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Меню налаштувань: гучність і повноекранний режим.
// Налаштування зберігаються в PlayerPrefs (залишаються після перезапуску гри).
public class SettingsMenu : MonoBehaviour
{
    // Ключі, під якими зберігаються налаштування
    private const string VolumePrefKey = "FalseMemory.Volume";
    private const string FullscreenPrefKey = "FalseMemory.Fullscreen";

    // Панель меню
    [SerializeField] private GameObject root;
    [SerializeField] private Slider volumeSlider;
    // Текст "Fullscreen: On/Off"
    [SerializeField] private TextMeshProUGUI fullscreenLabel;

    private bool fullscreen;

    private void Start()
    {
        Hide();
        LoadAndApply();
    }

    // Показати меню
    public void Show()
    {
        if (root != null)
        {
            root.SetActive(true);
        }
        LoadAndApply();
    }

    // Сховати меню
    public void Hide()
    {
        if (root != null)
        {
            root.SetActive(false);
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
        ApplyFullscreen();

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
    private void LoadAndApply()
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

        ApplyFullscreen();
        RefreshFullscreenLabel();
    }

    private void ApplyFullscreen()
    {
        Screen.fullScreen = fullscreen;
    }

    private void RefreshFullscreenLabel()
    {
        if (fullscreenLabel != null)
        {
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

    private void OnEnable()
    {
        RefreshFullscreenLabel();
    }
}
