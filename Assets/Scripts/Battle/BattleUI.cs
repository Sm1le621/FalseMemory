using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

public sealed class BattleUI : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public TextMeshProUGUI playerStats;
    public TextMeshProUGUI enemyStats;
    public TextMeshProUGUI roundText;
    public Image playerPortrait;
    public Image enemyPortrait;
    public Button[] commandButtons;
    public TextMeshProUGUI[] commandLabels;
    public Sprite previewPlayerSprite;
    public Sprite previewEnemySprite;

    private Action[] callbacks = Array.Empty<Action>();
    private string[] labels = Array.Empty<string>();
    private int selected;
    private int acceptAfterFrame;
    private bool commandsEnabled;
    private readonly Dictionary<Sprite, Sprite> pointSprites = new Dictionary<Sprite, Sprite>();

    private void Awake()
    {
        // The module handles mouse only. Keyboard is edge-triggered below,
        // preventing UI Submit and our keyboard handler from firing twice.
        var system = EventSystem.current;
        if (system != null)
        {
            var module = system.GetComponent<InputSystemUIInputModule>();
            if (module != null) { module.move = null; module.submit = null; module.cancel = null; }
        }
        for (int i = 0; i < commandButtons.Length; i++)
        {
            int index = i;
            commandButtons[i].onClick.AddListener(() => Invoke(index));
        }
    }

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (!commandsEnabled || keyboard == null || labels.Length == 0 || Time.frameCount <= acceptAfterFrame) return;
        if (keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame ||
            keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame)
            Select((selected + labels.Length - 1) % labels.Length);
        else if (keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame ||
                 keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame)
            Select((selected + 1) % labels.Length);
        else if (keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame) Invoke(selected);
        else if (keyboard.escapeKey.wasPressedThisFrame) Select(0); // Stage 1 has no submenus.
    }

    private void Invoke(int index)
    {
        if (!commandsEnabled || index < 0 || index >= callbacks.Length || Time.frameCount <= acceptAfterFrame) return;
        callbacks[index]?.Invoke();
    }

    public void ShowMenu(string[] names, Action[] actions)
    {
        labels = names; callbacks = actions; selected = 0;
        acceptAfterFrame = Time.frameCount + 1;
        for (int i = 0; i < commandButtons.Length; i++) commandButtons[i].gameObject.SetActive(i < names.Length);
        SetCommandsEnabled(true);
        Select(0);
    }

    private void Select(int index)
    {
        selected = index;
        for (int i = 0; i < labels.Length; i++) commandLabels[i].text = (i == index ? "> " : "  ") + labels[i];
    }

    public void SetCommandsEnabled(bool enabled)
    {
        commandsEnabled = enabled;
        foreach (var button in commandButtons) button.interactable = enabled;
    }

    public void ShowMessage(string message) => messageText.text = message;

    public void ShowUnits(BattleUnit player, BattleUnit enemy)
    {
        ApplyPortrait(playerPortrait, player);
        ApplyPortrait(enemyPortrait, enemy);
        RefreshStats(player, enemy, 0);
    }

    private void ApplyPortrait(Image image, BattleUnit unit)
    {
        image.sprite = PointSprite(unit.battleSprite);
        image.color = unit.spriteColor;
        image.rectTransform.localScale = new Vector3(unit.flipInBattle ? -1f : 1f, 1f, 1f);
    }

    // Keep the user's source texture/import settings intact, even if bilinear.
    private Sprite PointSprite(Sprite source)
    {
        if (source == null || source.texture.filterMode == FilterMode.Point) return source;
        if (pointSprites.TryGetValue(source, out var copy)) return copy;
        // Imported sprites can be non-readable. Copy through the GPU instead
        // of Instantiate(Texture2D), which Unity rejects for those imports.
        var original = source.texture;
        var temporary = RenderTexture.GetTemporary(original.width, original.height, 0);
        var previous = RenderTexture.active;
        var texture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        try
        {
            Graphics.Blit(original, temporary);
            RenderTexture.active = temporary;
            texture.ReadPixels(new Rect(0, 0, original.width, original.height), 0, 0);
            texture.Apply(false, true);
        }
        finally { RenderTexture.active = previous; RenderTexture.ReleaseTemporary(temporary); }
        texture.filterMode = FilterMode.Point;
        copy = Sprite.Create(texture, source.rect, new Vector2(source.pivot.x / source.rect.width,
            source.pivot.y / source.rect.height), source.pixelsPerUnit);
        pointSprites.Add(source, copy);
        return copy;
    }

    private void OnDestroy()
    {
        foreach (var sprite in pointSprites.Values) { Destroy(sprite.texture); Destroy(sprite); }
        pointSprites.Clear();
    }

    public void RefreshStats(BattleUnit player, BattleUnit enemy, int round)
    {
        playerStats.text = Stats(player);
        enemyStats.text = Stats(enemy);
        roundText.text = "ROUND " + round;
        enemyPortrait.color = enemy.IsAlive ? enemy.spriteColor : new Color(0.35f, 0.35f, 0.35f, 0.35f);
    }

    private static string Stats(BattleUnit unit) =>
        unit.displayName + "   LV " + unit.level + "\nHP  " + unit.Health.currentHealth + " / " + unit.Health.maxHealth +
        "\nATK " + unit.attack + "   DEF " + unit.defense + "   SPD " + unit.Speed + (unit.IsDefending ? "\nDEFEND" : "");

#if UNITY_EDITOR
    // Called via Unity MCP once; all resulting UI objects/references are saved in BattleScene.
    public void BuildLayout()
    {
        if (messageText != null) throw new InvalidOperationException("Battle layout already exists.");
        var canvasObject = new GameObject("BattleCanvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        canvasObject.transform.SetParent(transform, false);
        var canvas = canvasObject.GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay; canvas.pixelPerfect = true;
        var scaler = canvasObject.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(960f, 540f); scaler.matchWidthOrHeight = 0.5f;
        var root = canvasObject.transform;
        Panel(root, "Backdrop", new Color32(18, 14, 24, 255), Vector2.zero, Vector2.one);
        Panel(root, "Battlefield", new Color32(31, 27, 37, 255), new Vector2(0.02f, 0.32f), new Vector2(0.98f, 0.82f));
        messageText = Label(root, "BattleMessage", "Skeleton blocks the way.", 23, new Vector2(0.04f, 0.84f), new Vector2(0.96f, 0.98f));
        playerPortrait = Portrait(root, "PlayerBattleSprite", previewPlayerSprite, new Vector2(0.70f, 0.42f), new Vector2(0.84f, 0.76f));
        playerPortrait.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        enemyPortrait = Portrait(root, "SkeletonBattleSprite", previewEnemySprite, new Vector2(0.19f, 0.42f), new Vector2(0.33f, 0.76f));
        Panel(root, "StatusPanel", new Color32(26, 22, 34, 255), new Vector2(0.02f, 0.02f), new Vector2(0.65f, 0.30f));
        playerStats = Label(root, "PlayerStatus", "Player   LV 1\nHP 100 / 100\nATK 15   DEF 8   SPD 10", 19, new Vector2(0.04f, 0.07f), new Vector2(0.34f, 0.27f));
        enemyStats = Label(root, "EnemyStatus", "Skeleton   LV 1\nHP 60 / 60\nATK 12   DEF 5   SPD 7", 19, new Vector2(0.35f, 0.07f), new Vector2(0.63f, 0.27f));
        roundText = Label(root, "Round", "ROUND 1", 13, new Vector2(0.04f, 0.025f), new Vector2(0.30f, 0.065f));
        Panel(root, "CommandPanel", new Color32(26, 22, 34, 255), new Vector2(0.67f, 0.02f), new Vector2(0.98f, 0.30f));
        commandButtons = new Button[2]; commandLabels = new TextMeshProUGUI[2];
        for (int i = 0; i < 2; i++)
        {
            float y = 0.19f - i * 0.075f;
            var image = Panel(root, i == 0 ? "AttackButton" : "DefendButton", new Color32(48, 40, 58, 255), new Vector2(0.69f, y), new Vector2(0.96f, y + 0.065f));
            var button = image.gameObject.AddComponent<Button>();
            image.raycastTarget = true;
            button.targetGraphic = image;
            button.navigation = new Navigation { mode = Navigation.Mode.None };
            var colors = button.colors; colors.highlightedColor = new Color(1f, 0.9f, 0.7f); colors.disabledColor = new Color(0.6f, 0.6f, 0.6f); button.colors = colors;
            commandButtons[i] = button;
            commandLabels[i] = Label(image.transform, "Label", i == 0 ? "> Attack" : "  Defend", 18, new Vector2(0.05f, 0f), new Vector2(0.95f, 1f));
        }
        Label(root, "Controls", "WASD / Arrows • Enter — choice", 12, new Vector2(0.69f, 0.025f), new Vector2(0.96f, 0.075f));
        UnityEditor.EditorUtility.SetDirty(this);
    }

    private static void Fit(RectTransform rect, Vector2 min, Vector2 max)
    {
        rect.anchorMin = min; rect.anchorMax = max;
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

    private static Image Panel(Transform parent, string name, Color color, Vector2 min, Vector2 max)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(Image)); obj.transform.SetParent(parent, false);
        var image = obj.GetComponent<Image>(); image.color = color; image.raycastTarget = false;
        Fit(image.rectTransform, min, max); return image;
    }

    private static Image Portrait(Transform parent, string name, Sprite sprite, Vector2 min, Vector2 max)
    {
        var image = Panel(parent, name, Color.white, min, max); image.sprite = sprite; image.preserveAspect = true; return image;
    }

    private static TextMeshProUGUI Label(Transform parent, string name, string value, int size, Vector2 min, Vector2 max)
    {
        var obj = new GameObject(name, typeof(RectTransform), typeof(TextMeshProUGUI)); obj.transform.SetParent(parent, false);
        var text = obj.GetComponent<TextMeshProUGUI>(); text.font = TMP_Settings.defaultFontAsset;
        text.text = value; text.fontSize = size; text.color = new Color32(232, 222, 201, 255);
        text.alignment = TextAlignmentOptions.MidlineLeft; text.raycastTarget = false;
        Fit(text.rectTransform, min, max); return text;
    }
#endif
}
