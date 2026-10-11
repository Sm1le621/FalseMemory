using System;
using System.Collections.Generic;
using System.Collections;
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
    public Image feedbackGlow;
    public TextMeshProUGUI feedbackText;
    [Min(0.1f)] public float feedbackDuration = 0.8f;
    public Image resultPanel;
    public TextMeshProUGUI resultText;
    public BattleHUD hud;
    public BattleAudioController audioController;

    private Action[] callbacks = Array.Empty<Action>();
    private string[] labels = Array.Empty<string>();
    private int selected;
    private int acceptAfterFrame;
    private bool commandsEnabled;
    private bool[] available = Array.Empty<bool>();
    private Action backAction;
    private int firstVisible;
    private Coroutine feedbackRoutine;
    public bool CommandsEnabled => commandsEnabled;
    public int MenuCount => labels.Length;
    public int SelectedIndex => selected;
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
            commandButtons[i].onClick.AddListener(() => Invoke(firstVisible + index));
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
        else if (keyboard.escapeKey.wasPressedThisFrame) { if (!CancelMenu()) Select(0); }
    }

    private void Invoke(int index)
    {
        if (!commandsEnabled || index < 0 || index >= callbacks.Length || !available[index] || Time.frameCount <= acceptAfterFrame) return;
        if (audioController != null) audioController.Play(BattleSound.Confirm);
        callbacks[index]?.Invoke();
    }

    public void ShowMenu(string[] names, Action[] actions, bool[] enabledOptions = null, Action onBack = null)
    {
        if (names.Length != actions.Length || (enabledOptions != null && enabledOptions.Length != names.Length))
            throw new ArgumentException("Menu labels/actions must have matching lengths.");
        labels = names; callbacks = actions; selected = 0;
        available = enabledOptions ?? Array.ConvertAll(names, _ => true);
        backAction = onBack; firstVisible = 0;
        acceptAfterFrame = Time.frameCount + 1;
        SetCommandsEnabled(true);
        Select(0);
        if (hud != null) hud.MenuChanged(onBack != null);
        if (audioController != null) audioController.Play(BattleSound.MenuOpen);
    }

    public bool CancelMenu()
    {
        if (!commandsEnabled || backAction == null || Time.frameCount <= acceptAfterFrame) return false;
        if (audioController != null) audioController.Play(BattleSound.Cancel);
        backAction();
        return true;
    }

    private void Select(int index)
    {
        if (index != selected && audioController != null) audioController.Play(BattleSound.Move);
        selected = index;
        if (selected < firstVisible) firstVisible = selected;
        if (selected >= firstVisible + commandButtons.Length) firstVisible = selected - commandButtons.Length + 1;
        RenderMenu();
    }

    private void RenderMenu()
    {
        for (int i = 0; i < commandButtons.Length; i++)
        {
            int index = firstVisible + i;
            commandButtons[i].gameObject.SetActive(index < labels.Length);
            if (index >= labels.Length) continue;
            commandLabels[i].text = (index == selected ? "> " : "  ") + labels[index];
            commandLabels[i].color = available[index] ? new Color32(232, 222, 201, 255) : new Color32(122, 113, 108, 255);
            commandButtons[i].interactable = commandsEnabled && available[index];
            if (hud != null) hud.StyleCommand(commandButtons[i], commandLabels[i], index == selected, commandsEnabled && available[index]);
        }
    }

    public void SetCommandsEnabled(bool enabled)
    {
        commandsEnabled = enabled;
        RenderMenu();
    }

    public void ShowFeedback(bool onPlayer, string text, Color color)
    {
        if (feedbackGlow == null || feedbackText == null) return;
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        var target = onPlayer ? playerPortrait.rectTransform : enemyPortrait.rectTransform;
        feedbackGlow.rectTransform.position = target.position;
        feedbackGlow.rectTransform.anchorMin = feedbackGlow.rectTransform.anchorMax = target.anchorMin;
        feedbackGlow.rectTransform.pivot = target.pivot;
        feedbackGlow.rectTransform.anchoredPosition = target.anchoredPosition;
        feedbackGlow.rectTransform.sizeDelta = target.rect.size;
        feedbackText.rectTransform.anchorMin = feedbackText.rectTransform.anchorMax = target.anchorMin;
        feedbackText.rectTransform.pivot = new Vector2(0.5f, 0f);
        feedbackText.rectTransform.anchoredPosition = target.anchoredPosition + new Vector2(target.rect.width * (0.5f-target.pivot.x), target.rect.height * (1f-target.pivot.y) + 4f);
        feedbackText.rectTransform.sizeDelta = new Vector2(220, 38);
        feedbackText.text = text; feedbackText.color = color;
        feedbackText.fontSize = text.Contains("CRITICAL") ? 22 : 18;
        feedbackRoutine = StartCoroutine(Feedback(color));
    }

    private IEnumerator Feedback(Color color)
    {
        bool impact = feedbackText.text != "MISS";
        feedbackGlow.gameObject.SetActive(impact); feedbackText.gameObject.SetActive(true);
        float duration = Mathf.Max(0.1f, feedbackDuration);
        Vector2 origin = feedbackText.rectTransform.anchoredPosition;
        for (float elapsed = 0f; elapsed < duration; elapsed += Time.unscaledDeltaTime)
        {
            color.a = Mathf.Sin(elapsed / duration * Mathf.PI) * 0.35f;
            feedbackGlow.color = color;
            feedbackText.rectTransform.anchoredPosition = origin + Vector2.up * (22f * elapsed / duration);
            Color textColor = feedbackText.color; textColor.a = Mathf.Clamp01((1f-elapsed/duration)*4f); feedbackText.color = textColor;
            yield return null;
        }
        feedbackGlow.gameObject.SetActive(false); feedbackText.gameObject.SetActive(false);
        feedbackRoutine = null;
    }

    public void ShowMessage(string message) => messageText.text = message;

    public void ShowVictoryResults(int gained, BattleUnit player)
    {
        resultPanel.gameObject.SetActive(true);
        resultText.text = "Victory!\nEXP Gained: +" + gained + "\nTotal EXP: " + player.CurrentExperience + "\nCurrent Level: " + player.level;
    }

    public void ShowUnits(BattleUnit player, BattleUnit enemy)
    {
        ApplyPortrait(playerPortrait, player);
        ApplyPortrait(enemyPortrait, enemy);
        RefreshStats(player, enemy, 0);
        if (hud != null) hud.Bind(player, enemy);
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
        playerStats.text = hud == null ? Stats(player) : player.displayName + "   LV " + player.level + "\nHP  " + player.Health.currentHealth + " / " + player.Health.maxHealth + "\nMP  " + player.CurrentMP + " / " + player.MaxMP;
        enemyStats.text = hud == null ? Stats(enemy) : enemy.displayName + "   LV " + enemy.level + "   |   HP " + enemy.Health.currentHealth + " / " + enemy.Health.maxHealth + (enemy.IsDefending ? "   DEFEND" : "");
        roundText.text = "РАУНД " + round;
        enemyPortrait.color = enemy.IsAlive ? enemy.spriteColor : new Color(0.35f, 0.35f, 0.35f, 0.35f);
    }

    private static string Stats(BattleUnit unit) =>
        unit.displayName + "   LV " + unit.level + "\nHP  " + unit.Health.currentHealth + " / " + unit.Health.maxHealth +
        "\nMP  " + unit.CurrentMP + " / " + unit.MaxMP +
        "\nATK " + unit.attack + "   DEF " + unit.defense + "   SPD " + unit.Speed +
        "\nMATK " + unit.magicAttack + "   MDEF " + unit.magicDefense +
        (unit.Inventory != null ? "\n" + unit.Inventory.Summary : "") + "\nEXP " + unit.CurrentExperience + (unit.IsDefending ? "\nDEFEND" : "");

#if UNITY_EDITOR
    // Upgrade the saved stage-1 objects in place, invoked only through Unity MCP.
    public void UpgradeLayout()
    {
        var root = messageText.transform.parent;
        Fit(root.Find("Battlefield").GetComponent<RectTransform>(), new Vector2(0.02f, 0.44f), new Vector2(0.98f, 0.82f));
        Fit(playerPortrait.rectTransform, new Vector2(0.70f, 0.49f), new Vector2(0.84f, 0.76f));
        Fit(enemyPortrait.rectTransform, new Vector2(0.19f, 0.49f), new Vector2(0.33f, 0.76f));
        Fit(root.Find("StatusPanel").GetComponent<RectTransform>(), new Vector2(0.02f, 0.02f), new Vector2(0.65f, 0.40f));
        Fit(root.Find("CommandPanel").GetComponent<RectTransform>(), new Vector2(0.67f, 0.02f), new Vector2(0.98f, 0.40f));
        Fit(playerStats.rectTransform, new Vector2(0.04f, 0.09f), new Vector2(0.34f, 0.37f));
        Fit(enemyStats.rectTransform, new Vector2(0.35f, 0.09f), new Vector2(0.63f, 0.37f));
        foreach (var text in new[] {playerStats, enemyStats})
        { text.fontSize = 17; text.enableAutoSizing = true; text.fontSizeMin = 12; text.fontSizeMax = 17; }
        Array.Resize(ref commandButtons, 5); Array.Resize(ref commandLabels, 5);
        for (int i = 0; i < 5; i++)
        {
            float y = 0.315f - i * 0.055f;
            if (commandButtons[i] == null)
            {
                var image = Panel(root, "Command" + (i + 1), new Color32(48, 40, 58, 255), Vector2.zero, Vector2.one);
                image.raycastTarget = true;
                var button = image.gameObject.AddComponent<Button>(); button.targetGraphic = image;
                button.navigation = new Navigation {mode = Navigation.Mode.None}; button.colors = commandButtons[0].colors;
                commandButtons[i] = button;
                commandLabels[i] = Label(image.transform, "Label", "", 17, new Vector2(0.05f, 0f), new Vector2(0.95f, 1f));
            }
            Fit(commandButtons[i].GetComponent<RectTransform>(), new Vector2(0.69f, y), new Vector2(0.96f, y + 0.047f));
            commandLabels[i].fontSize = 17;
            commandLabels[i].enableAutoSizing = true; commandLabels[i].fontSizeMin = 11; commandLabels[i].fontSizeMax = 17;
            commandLabels[i].text = new[] {"> Attack", "Magic", "Items", "Defend", "Flee"}[i];
        }
        var controls = root.Find("Controls").GetComponent<TextMeshProUGUI>();
        controls.text = "WASD / стрелки • Enter • Esc — назад"; controls.fontSize = 11;
        controls.enableAutoSizing = true; controls.fontSizeMin = 9; controls.fontSizeMax = 11;
        if (feedbackGlow == null) feedbackGlow = Panel(root, "AbilityGlow", Color.clear, Vector2.zero, Vector2.zero);
        if (feedbackText == null) feedbackText = Label(root, "AbilityAmount", "", 17, Vector2.zero, Vector2.zero);
        feedbackText.alignment = TextAlignmentOptions.Center;
        feedbackText.enableAutoSizing = true; feedbackText.fontSizeMin = 11; feedbackText.fontSizeMax = 17;
        feedbackGlow.gameObject.SetActive(false); feedbackText.gameObject.SetActive(false);
        if (resultPanel == null)
            resultPanel = Panel(root, "BattleResults", new Color32(36, 30, 44, 255), new Vector2(0.30f, 0.46f), new Vector2(0.67f, 0.81f));
        if (resultText == null)
            resultText = Label(resultPanel.transform, "VictoryResultText", "Victory!\nEXP Gained: +20\nTotal EXP: 20\nCurrent Level: 1", 21,
                new Vector2(0.05f, 0.05f), new Vector2(0.95f, 0.95f));
        resultText.alignment = TextAlignmentOptions.Center;
        resultText.enableAutoSizing = true; resultText.fontSizeMin = 14; resultText.fontSizeMax = 21;
        resultPanel.gameObject.SetActive(false);
        UnityEditor.EditorUtility.SetDirty(this);
    }

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
        messageText = Label(root, "BattleMessage", "Скелет преграждает путь.", 23, new Vector2(0.04f, 0.84f), new Vector2(0.96f, 0.98f));
        playerPortrait = Portrait(root, "PlayerBattleSprite", previewPlayerSprite, new Vector2(0.70f, 0.42f), new Vector2(0.84f, 0.76f));
        playerPortrait.rectTransform.localScale = new Vector3(-1f, 1f, 1f);
        enemyPortrait = Portrait(root, "SkeletonBattleSprite", previewEnemySprite, new Vector2(0.19f, 0.42f), new Vector2(0.33f, 0.76f));
        Panel(root, "StatusPanel", new Color32(26, 22, 34, 255), new Vector2(0.02f, 0.02f), new Vector2(0.65f, 0.30f));
        playerStats = Label(root, "PlayerStatus", "Player   LV 1\nHP 100 / 100\nATK 15   DEF 8   SPD 10", 19, new Vector2(0.04f, 0.07f), new Vector2(0.34f, 0.27f));
        enemyStats = Label(root, "EnemyStatus", "Skeleton   LV 1\nHP 60 / 60\nATK 12   DEF 5   SPD 7", 19, new Vector2(0.35f, 0.07f), new Vector2(0.63f, 0.27f));
        roundText = Label(root, "Round", "РАУНД 1", 13, new Vector2(0.04f, 0.025f), new Vector2(0.30f, 0.065f));
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
        Label(root, "Controls", "WASD / стрелки • Enter — выбор", 12, new Vector2(0.69f, 0.025f), new Vector2(0.96f, 0.075f));
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
