using TMPro;
using UnityEngine;
using UnityEngine.TextCore.LowLevel;
using UnityEngine.UI;

public static class PixelUi
{
    public const string FontResourcePath = "Fonts/PressStart2P-Regular";

    private static TMP_FontAsset cachedFont;
    private static Sprite whiteSprite;

    public static Color Background = new Color32(18, 12, 28, 255);
    public static Color Panel = new Color32(32, 22, 48, 255);
    public static Color ButtonNormal = new Color32(56, 38, 88, 255);
    public static Color ButtonHover = new Color32(92, 62, 140, 255);
    public static Color ButtonPressed = new Color32(40, 26, 64, 255);
    public static Color Outline = new Color32(244, 232, 193, 255);
    public static Color Text = new Color32(244, 232, 193, 255);
    public static Color TextMuted = new Color32(176, 160, 128, 255);

    public static TMP_FontAsset Font
    {
        get
        {
            if (cachedFont != null) return cachedFont;

            Font source = Resources.Load<Font>(FontResourcePath);
            if (source != null)
            {
                try
                {
                    cachedFont = TMP_FontAsset.CreateFontAsset(
                        source,
                        16,
                        1,
                        GlyphRenderMode.RASTER_HINTED,
                        512,
                        512,
                        AtlasPopulationMode.Dynamic);
                }
                catch (System.Exception)
                {
                    cachedFont = TMP_FontAsset.CreateFontAsset(source);
                }
            }

            if (cachedFont == null)
            {
                cachedFont = TMP_Settings.defaultFontAsset;
            }

            ApplyPointFilter(cachedFont);
            return cachedFont;
        }
    }

    public static Sprite WhiteSprite
    {
        get
        {
            if (whiteSprite != null) return whiteSprite;
            Texture2D texture = Texture2D.whiteTexture;
            whiteSprite = Sprite.Create(
                texture,
                new Rect(0f, 0f, texture.width, texture.height),
                new Vector2(0.5f, 0.5f),
                100f);
            return whiteSprite;
        }
    }

    public static Canvas CreateCanvas(string objectName)
    {
        GameObject canvasObject = new GameObject(objectName);
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.pixelPerfect = true;

        CanvasScaler scaler = canvasObject.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ConstantPixelSize;
        scaler.scaleFactor = Mathf.Max(1f, Mathf.Floor(Screen.height / 360f));

        canvasObject.AddComponent<GraphicRaycaster>();
        return canvas;
    }

    public static TextMeshProUGUI CreateLabel(
        Transform parent,
        string objectName,
        string content,
        int fontSize,
        TextAlignmentOptions alignment)
    {
        GameObject labelObject = new GameObject(objectName);
        labelObject.transform.SetParent(parent, false);

        TextMeshProUGUI label = labelObject.AddComponent<TextMeshProUGUI>();
        label.font = Font;
        label.text = content;
        label.fontSize = fontSize;
        label.color = Text;
        label.alignment = alignment;
        label.raycastTarget = false;
        label.enableAutoSizing = false;
        label.overflowMode = TextOverflowModes.Overflow;
        label.extraPadding = true;
        return label;
    }

    public static Image CreatePanel(Transform parent, string objectName, Color color)
    {
        GameObject panelObject = new GameObject(objectName);
        panelObject.transform.SetParent(parent, false);
        Image image = panelObject.AddComponent<Image>();
        image.sprite = WhiteSprite;
        image.color = color;
        image.raycastTarget = true;
        return image;
    }

    public static Button CreateButton(Transform parent, string objectName, string caption, Vector2 size)
    {
        Image background = CreatePanel(parent, objectName, ButtonNormal);
        RectTransform rect = background.rectTransform;
        rect.sizeDelta = size;

        Outline outline = background.gameObject.AddComponent<Outline>();
        outline.effectColor = Outline;
        outline.effectDistance = new Vector2(2f, -2f);

        TextMeshProUGUI label = CreateLabel(
            background.transform,
            "Label",
            caption,
            16,
            TextAlignmentOptions.Center);
        Stretch(label.rectTransform);

        Button button = background.gameObject.AddComponent<Button>();
        button.targetGraphic = background;
        ColorBlock colors = button.colors;
        colors.normalColor = ButtonNormal;
        colors.highlightedColor = ButtonHover;
        colors.pressedColor = ButtonPressed;
        colors.selectedColor = ButtonHover;
        colors.disabledColor = new Color(0.25f, 0.2f, 0.3f, 1f);
        colors.colorMultiplier = 1f;
        colors.fadeDuration = 0.05f;
        button.colors = colors;
        return button;
    }

    public static Slider CreateSlider(Transform parent, string objectName, Vector2 size)
    {
        Image track = CreatePanel(parent, objectName, new Color32(20, 14, 32, 255));
        track.rectTransform.sizeDelta = size;

        Image fillAreaImage = CreatePanel(track.transform, "Fill Area", new Color(0f, 0f, 0f, 0f));
        Stretch(fillAreaImage.rectTransform, 6f, 6f, 6f, 6f);
        fillAreaImage.raycastTarget = false;

        Image fill = CreatePanel(fillAreaImage.transform, "Fill", ButtonHover);
        Stretch(fill.rectTransform);
        fill.raycastTarget = false;

        Image handleArea = CreatePanel(track.transform, "Handle Slide Area", new Color(0f, 0f, 0f, 0f));
        Stretch(handleArea.rectTransform, 8f, 8f, 4f, 4f);
        handleArea.raycastTarget = false;

        Image handle = CreatePanel(handleArea.transform, "Handle", Outline);
        handle.rectTransform.sizeDelta = new Vector2(12f, 0f);

        Slider slider = track.gameObject.AddComponent<Slider>();
        slider.fillRect = fill.rectTransform;
        slider.handleRect = handle.rectTransform;
        slider.targetGraphic = handle;
        slider.direction = Slider.Direction.LeftToRight;
        slider.minValue = 0f;
        slider.maxValue = 1f;
        slider.wholeNumbers = false;
        return slider;
    }

    public static void Stretch(RectTransform rect, float left = 0f, float right = 0f, float top = 0f, float bottom = 0f)
    {
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = new Vector2(left, bottom);
        rect.offsetMax = new Vector2(-right, -top);
    }

    private static void ApplyPointFilter(TMP_FontAsset fontAsset)
    {
        if (fontAsset == null) return;

        if (fontAsset.atlasTextures != null)
        {
            for (int i = 0; i < fontAsset.atlasTextures.Length; i++)
            {
                if (fontAsset.atlasTextures[i] != null)
                {
                    fontAsset.atlasTextures[i].filterMode = FilterMode.Point;
                }
            }
        }

        if (fontAsset.material != null && fontAsset.material.mainTexture != null)
        {
            fontAsset.material.mainTexture.filterMode = FilterMode.Point;
        }
    }
}
