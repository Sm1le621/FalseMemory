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
    public Sprite impactSprite;
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

    public void ShowFeedback(bool onPlayer, string text, Color color, bool showImpact=true)
    {
        if (feedbackGlow == null || feedbackText == null) return;
        if (feedbackRoutine != null) StopCoroutine(feedbackRoutine);
        var target = onPlayer ? playerPortrait.rectTransform : enemyPortrait.rectTransform;
        feedbackGlow.rectTransform.position = target.position;
        feedbackGlow.rectTransform.anchorMin = feedbackGlow.rectTransform.anchorMax = target.anchorMin;
        feedbackGlow.rectTransform.pivot = target.pivot;
        feedbackGlow.rectTransform.anchoredPosition = target.anchoredPosition;
        feedbackGlow.rectTransform.sizeDelta = target.rect.size;
        feedbackGlow.sprite=impactSprite;feedbackGlow.preserveAspect=true;
        feedbackText.rectTransform.anchorMin = feedbackText.rectTransform.anchorMax = target.anchorMin;
        feedbackText.rectTransform.pivot = new Vector2(0.5f, 0f);
        feedbackText.rectTransform.anchoredPosition = target.anchoredPosition + new Vector2(target.rect.width * (0.5f-target.pivot.x), target.rect.height * (0.75f-target.pivot.y));
        feedbackText.rectTransform.sizeDelta = new Vector2(220, 38);
        feedbackText.text = text; feedbackText.color = color;
        feedbackText.fontSize = text.Contains("CRITICAL") ? 22 : 18;
        feedbackRoutine = StartCoroutine(Feedback(color,showImpact));
    }

    private IEnumerator Feedback(Color color,bool showImpact)
    {
        bool impact = showImpact && feedbackText.text != "MISS";
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

    public void ShowVictoryResults(BattleUnit player)
    {
        if (resultPanel == null || resultText == null) return;
        resultPanel.gameObject.SetActive(true);
        resultText.text = "VICTORY\n" + player.displayName + " stands victorious.";
        if(hud!=null)hud.RevealResult();
    }

    public void ShowDefeatResults()
    {
        if(resultPanel==null || resultText==null)return;
        resultPanel.color=new Color32(43,24,35,248);resultPanel.gameObject.SetActive(true);
        resultText.text="DEFEAT\nYour journey ends here.";
        if(hud!=null)hud.RevealResult();
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
        playerStats.text = Stats(player);
        enemyStats.text = Stats(enemy) + (enemy.IsDefending ? "\nDEFENDING" : "");
        roundText.text = "ROUND " + round;
        if(enemy.IsAlive)enemyPortrait.color=enemy.spriteColor;
        else if(GetComponent<BattlePresentation>()==null)enemyPortrait.color=new Color(0.35f,0.35f,0.35f,0.35f);
    }

    private static string Stats(BattleUnit unit) =>
        unit.displayName + "\nHP " + unit.Health.currentHealth + " / " + unit.Health.maxHealth +
        "   MP " + unit.CurrentMP + " / " + unit.MaxMP +
        "\nATK " + unit.EffectiveAttack + "   DEF " + unit.EffectiveDefense + "   SPD " + unit.Speed +
        (unit.Inventory!=null ? "\n" + unit.Inventory.Summary : "") +
        (unit.IsDefending ? "\nDEFENDING" : "");
}
