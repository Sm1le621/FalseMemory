using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public class MainMenuBootstrap : MonoBehaviour
{
    [SerializeField] private string gameTitle = "FALSE MEMORY";
    [SerializeField] private string gameSceneName = "SampleScene";

    private void Awake()
    {
        EnsureEventSystem();
        BuildUi();
    }

    private static void EnsureEventSystem()
    {
        if (FindAnyObjectByType<EventSystem>() != null) return;

        GameObject eventSystemObject = new GameObject("EventSystem");
        eventSystemObject.AddComponent<EventSystem>();
        eventSystemObject.AddComponent<InputSystemUIInputModule>();
    }

    private void BuildUi()
    {
        Canvas canvas = PixelUi.CreateCanvas("MainMenuCanvas");
        Image background = PixelUi.CreatePanel(canvas.transform, "Background", PixelUi.Background);
        PixelUi.Stretch(background.rectTransform);

        RectTransform menuRoot = CreateCenteredColumn(canvas.transform, "MenuRoot", new Vector2(360f, 280f));
        VerticalLayoutGroup menuLayout = menuRoot.gameObject.AddComponent<VerticalLayoutGroup>();
        menuLayout.padding = new RectOffset(16, 16, 16, 16);
        menuLayout.spacing = 12f;
        menuLayout.childAlignment = TextAnchor.MiddleCenter;
        menuLayout.childControlHeight = false;
        menuLayout.childControlWidth = true;
        menuLayout.childForceExpandHeight = false;
        menuLayout.childForceExpandWidth = true;

        TextMeshProUGUI title = PixelUi.CreateLabel(
            menuRoot,
            "Title",
            gameTitle,
            16,
            TextAlignmentOptions.Center);
        LayoutElement titleLayout = title.gameObject.AddComponent<LayoutElement>();
        titleLayout.preferredHeight = 48f;
        titleLayout.minHeight = 48f;

        MainMenuController controller = gameObject.AddComponent<MainMenuController>();

        Button startButton = PixelUi.CreateButton(menuRoot, "StartButton", "START", new Vector2(280f, 40f));
        startButton.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
        startButton.onClick.AddListener(controller.OnStart);

        Button settingsButton = PixelUi.CreateButton(menuRoot, "SettingsButton", "SETTING", new Vector2(280f, 40f));
        settingsButton.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;

        Button exitButton = PixelUi.CreateButton(menuRoot, "ExitButton", "EXIT", new Vector2(280f, 40f));
        exitButton.gameObject.AddComponent<LayoutElement>().preferredHeight = 40f;
        exitButton.onClick.AddListener(controller.OnExit);

        SettingsMenu settingsMenu = BuildSettingsPanel(canvas.transform);
        controller.Initialize(gameSceneName, settingsMenu);
        settingsButton.onClick.AddListener(controller.OnSettings);
    }

    private static SettingsMenu BuildSettingsPanel(Transform canvas)
    {
        Image dim = PixelUi.CreatePanel(canvas, "SettingsDim", new Color(0f, 0f, 0f, 0.65f));
        PixelUi.Stretch(dim.rectTransform);

        RectTransform panel = CreateCenteredColumn(dim.transform, "SettingsPanel", new Vector2(380f, 280f));
        Image panelImage = panel.gameObject.AddComponent<Image>();
        panelImage.sprite = PixelUi.WhiteSprite;
        panelImage.color = PixelUi.Panel;
        Outline panelOutline = panel.gameObject.AddComponent<Outline>();
        panelOutline.effectColor = PixelUi.Outline;
        panelOutline.effectDistance = new Vector2(2f, -2f);

        VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
        layout.padding = new RectOffset(20, 20, 18, 18);
        layout.spacing = 12f;
        layout.childAlignment = TextAnchor.UpperCenter;
        layout.childControlHeight = false;
        layout.childControlWidth = true;
        layout.childForceExpandHeight = false;
        layout.childForceExpandWidth = true;

        TextMeshProUGUI heading = PixelUi.CreateLabel(
            panel,
            "SettingsTitle",
            "SETTING",
            20,
            TextAlignmentOptions.Center);
        heading.gameObject.AddComponent<LayoutElement>().preferredHeight = 32f;

        TextMeshProUGUI volumeLabel = PixelUi.CreateLabel(
            panel,
            "VolumeLabel",
            "VOLUME",
            12,
            TextAlignmentOptions.Center);
        volumeLabel.color = PixelUi.TextMuted;
        volumeLabel.gameObject.AddComponent<LayoutElement>().preferredHeight = 18f;

        Slider volumeSlider = PixelUi.CreateSlider(panel, "VolumeSlider", new Vector2(280f, 18f));
        volumeSlider.gameObject.AddComponent<LayoutElement>().preferredHeight = 18f;

        Button fullscreenButton = PixelUi.CreateButton(panel, "FullscreenButton", "FULLSCREEN  ON", new Vector2(280f, 36f));
        fullscreenButton.gameObject.AddComponent<LayoutElement>().preferredHeight = 36f;
        TextMeshProUGUI fullscreenLabel = fullscreenButton.GetComponentInChildren<TextMeshProUGUI>();

        Button backButton = PixelUi.CreateButton(panel, "BackButton", "BACK", new Vector2(280f, 36f));
        backButton.gameObject.AddComponent<LayoutElement>().preferredHeight = 36f;

        SettingsMenu settings = dim.gameObject.AddComponent<SettingsMenu>();
        settings.Bind(dim.gameObject, volumeSlider, fullscreenLabel);
        volumeSlider.onValueChanged.AddListener(settings.OnVolumeChanged);
        fullscreenButton.onClick.AddListener(settings.ToggleFullscreen);
        backButton.onClick.AddListener(settings.Hide);
        return settings;
    }

    private static RectTransform CreateCenteredColumn(Transform parent, string objectName, Vector2 size)
    {
        GameObject column = new GameObject(objectName);
        column.transform.SetParent(parent, false);
        RectTransform rect = column.AddComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = size;
        rect.anchoredPosition = Vector2.zero;
        return rect;
    }
}
