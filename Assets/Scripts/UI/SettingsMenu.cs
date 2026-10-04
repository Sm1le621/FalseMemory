using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private const string VolumePrefKey = "FalseMemory.Volume";
    private const string FullscreenPrefKey = "FalseMemory.Fullscreen";

    [SerializeField] private GameObject root;
    [SerializeField] private Slider volumeSlider;
    [SerializeField] private TextMeshProUGUI fullscreenLabel;

    private bool fullscreen;

    public void Bind(GameObject panelRoot, Slider slider, TextMeshProUGUI fullscreenValueLabel)
    {
        root = panelRoot;
        volumeSlider = slider;
        fullscreenLabel = fullscreenValueLabel;
        Hide();
        LoadAndApply();
    }

    public void Show()
    {
        if (root != null) root.SetActive(true);
        LoadAndApply();
    }

    public void Hide()
    {
        if (root != null) root.SetActive(false);
    }

    public void OnVolumeChanged(float value)
    {
        AudioListener.volume = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(VolumePrefKey, AudioListener.volume);
        PlayerPrefs.Save();
    }

    public void ToggleFullscreen()
    {
        fullscreen = !fullscreen;
        ApplyFullscreen();
        PlayerPrefs.SetInt(FullscreenPrefKey, fullscreen ? 1 : 0);
        PlayerPrefs.Save();
        RefreshFullscreenLabel();
    }

    private void LoadAndApply()
    {
        float volume = PlayerPrefs.GetFloat(VolumePrefKey, 1f);
        AudioListener.volume = Mathf.Clamp01(volume);
        if (volumeSlider != null)
        {
            volumeSlider.SetValueWithoutNotify(AudioListener.volume);
        }

        fullscreen = PlayerPrefs.GetInt(FullscreenPrefKey, Screen.fullScreen ? 1 : 0) == 1;
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
            fullscreenLabel.text = fullscreen ? "FULLSCREEN  ON" : "FULLSCREEN  OFF";
        }
    }
}
