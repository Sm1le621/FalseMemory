using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Інтерфейс бою: повідомлення, статистика, портрети, меню команд
// (керування клавіатурою і мишкою) і "вилітаючі" цифри шкоди.
public sealed class BattleUI : MonoBehaviour
{
    // Текст повідомлень ("Your turn." і т.д.)
    public TextMeshProUGUI messageText;
    // Статистика гравця і ворога
    public TextMeshProUGUI playerStats;
    public TextMeshProUGUI enemyStats;
    // "ROUND 1"
    public TextMeshProUGUI roundText;
    // Картинки персонажів
    public Image playerPortrait;
    public Image enemyPortrait;
    // Кнопки меню і їх підписи (кнопок може бути менше, ніж пунктів - тоді список прокручується)
    public Button[] commandButtons;
    public TextMeshProUGUI[] commandLabels;
    // Спалах удару поверх портрета
    public Image feedbackGlow;
    public Sprite impactSprite;
    // Текст "-10" / "MISS" над персонажем
    public TextMeshProUGUI feedbackText;
    [Min(0.1f)]
    public float feedbackDuration = 0.8f;
    // Панель результату (перемога / поразка)
    public Image resultPanel;
    public TextMeshProUGUI resultText;
    public BattleHUD hud;
    public BattleAudioController audioController;

    // Що робить кожен пункт меню
    private Action[] callbacks = Array.Empty<Action>();
    // Назви пунктів меню
    private string[] labels = Array.Empty<string>();
    // Номер вибраного пункту
    private int selected;
    // До якого кадру ігноруємо натискання (щоб одне натискання не "проскочило" в нове меню)
    private int acceptAfterFrame;
    private bool commandsEnabled;
    // Які пункти доступні
    private bool[] available = Array.Empty<bool>();
    // Що робити при Esc (null - Esc нічого не робить)
    private Action backAction;
    // Номер першого видимого пункту (для прокрутки)
    private int firstVisible;
    private Coroutine feedbackRoutine;

    public bool CommandsEnabled
    {
        get { return commandsEnabled; }
    }

    // Скільки пунктів у меню
    public int MenuCount
    {
        get { return labels.Length; }
    }

    public int SelectedIndex
    {
        get { return selected; }
    }

    // Кеш копій спрайтів з піксельною фільтрацією (оригінал -> копія)
    private readonly Dictionary<Sprite, Sprite> pointSprites = new Dictionary<Sprite, Sprite>();

    private void Awake()
    {
        // Модуль UI обробляє тільки мишку. Клавіатуру обробляємо самі нижче,
        // інакше Enter спрацьовував би двічі (і в UI, і в нашому коді).
        EventSystem system = EventSystem.current;
        if (system != null)
        {
            InputSystemUIInputModule module = system.GetComponent<InputSystemUIInputModule>();
            if (module != null)
            {
                module.move = null;
                module.submit = null;
                module.cancel = null;
            }
        }

        // Підписуємо кожну кнопку
        for (int i = 0; i < commandButtons.Length; i++)
        {
            // Окрема змінна, щоб кожна кнопка запам'ятала свій номер
            int index = i;
            commandButtons[i].onClick.AddListener(() => Invoke(firstVisible + index));
        }
    }

    private void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (!commandsEnabled || keyboard == null || labels.Length == 0 || Time.frameCount <= acceptAfterFrame)
        {
            return;
        }

        // wasPressedThisFrame - тільки в момент натискання (утримання не рахується)
        bool up = keyboard.upArrowKey.wasPressedThisFrame || keyboard.wKey.wasPressedThisFrame ||
                  keyboard.leftArrowKey.wasPressedThisFrame || keyboard.aKey.wasPressedThisFrame;
        bool down = keyboard.downArrowKey.wasPressedThisFrame || keyboard.sKey.wasPressedThisFrame ||
                    keyboard.rightArrowKey.wasPressedThisFrame || keyboard.dKey.wasPressedThisFrame;
        bool enter = keyboard.enterKey.wasPressedThisFrame || keyboard.numpadEnterKey.wasPressedThisFrame;

        if (up)
        {
            // Вгору (з першого пункту переходимо на останній)
            Select((selected + labels.Length - 1) % labels.Length);
        }
        else if (down)
        {
            // Вниз (з останнього - на перший)
            Select((selected + 1) % labels.Length);
        }
        else if (enter)
        {
            Invoke(selected);
        }
        else if (keyboard.escapeKey.wasPressedThisFrame)
        {
            // Esc: назад, а якщо назад нікуди - на перший пункт
            if (!CancelMenu())
            {
                Select(0);
            }
        }
    }

    // Виконати пункт меню номер index
    private void Invoke(int index)
    {
        if (!commandsEnabled || index < 0 || index >= callbacks.Length || !available[index] || Time.frameCount <= acceptAfterFrame)
        {
            return;
        }

        if (audioController != null)
        {
            audioController.Play(BattleSound.Confirm);
        }

        if (callbacks[index] != null)
        {
            callbacks[index].Invoke();
        }
    }

    // Показати нове меню
    public void ShowMenu(string[] names, Action[] actions, bool[] enabledOptions = null, Action onBack = null)
    {
        // Кількість назв, дій і прапорців має збігатися
        if (names.Length != actions.Length || (enabledOptions != null && enabledOptions.Length != names.Length))
        {
            throw new ArgumentException("Menu labels/actions must have matching lengths.");
        }

        labels = names;
        callbacks = actions;
        selected = 0;

        if (enabledOptions != null)
        {
            available = enabledOptions;
        }
        else
        {
            // Якщо не передали - всі пункти доступні
            available = new bool[names.Length];
            for (int i = 0; i < available.Length; i++)
            {
                available[i] = true;
            }
        }

        backAction = onBack;
        firstVisible = 0;
        // Натискання в цьому і наступному кадрі ігноруємо
        acceptAfterFrame = Time.frameCount + 1;

        SetCommandsEnabled(true);
        Select(0);

        if (hud != null)
        {
            hud.MenuChanged(onBack != null);
        }
        if (audioController != null)
        {
            audioController.Play(BattleSound.MenuOpen);
        }
    }

    // Назад (Esc). Повертає true, якщо назад було куди
    public bool CancelMenu()
    {
        if (!commandsEnabled || backAction == null || Time.frameCount <= acceptAfterFrame)
        {
            return false;
        }

        if (audioController != null)
        {
            audioController.Play(BattleSound.Cancel);
        }
        backAction();
        return true;
    }

    // Вибрати пункт меню
    private void Select(int index)
    {
        if (index != selected && audioController != null)
        {
            audioController.Play(BattleSound.Move);
        }

        selected = index;

        // Прокручуємо список, щоб вибраний пункт було видно
        if (selected < firstVisible)
        {
            firstVisible = selected;
        }
        if (selected >= firstVisible + commandButtons.Length)
        {
            firstVisible = selected - commandButtons.Length + 1;
        }

        RenderMenu();
    }

    // Намалювати меню (тексти, кольори, які кнопки видно)
    private void RenderMenu()
    {
        for (int i = 0; i < commandButtons.Length; i++)
        {
            int index = firstVisible + i;

            // Зайві кнопки ховаємо
            commandButtons[i].gameObject.SetActive(index < labels.Length);
            if (index >= labels.Length)
            {
                continue;
            }

            // Біля вибраного пункту - стрілка "> "
            if (index == selected)
            {
                commandLabels[i].text = "> " + labels[index];
            }
            else
            {
                commandLabels[i].text = "  " + labels[index];
            }

            // Недоступні пункти - сірі
            if (available[index])
            {
                commandLabels[i].color = new Color32(232, 222, 201, 255);
            }
            else
            {
                commandLabels[i].color = new Color32(122, 113, 108, 255);
            }

            commandButtons[i].interactable = commandsEnabled && available[index];

            if (hud != null)
            {
                hud.StyleCommand(commandButtons[i], commandLabels[i], index == selected, commandsEnabled && available[index]);
            }
        }
    }

    // Увімкнути / вимкнути меню
    public void SetCommandsEnabled(bool enabled)
    {
        commandsEnabled = enabled;
        RenderMenu();
    }

    // Показати "вилітаючий" текст (шкода, MISS) над персонажем
    public void ShowFeedback(bool onPlayer, string text, Color color, bool showImpact = true)
    {
        if (feedbackGlow == null || feedbackText == null)
        {
            return;
        }

        // Якщо попередній ще показується - зупиняємо
        if (feedbackRoutine != null)
        {
            StopCoroutine(feedbackRoutine);
        }

        RectTransform target;
        if (onPlayer)
        {
            target = playerPortrait.rectTransform;
        }
        else
        {
            target = enemyPortrait.rectTransform;
        }

        // Ставимо спалах точно на портрет
        RectTransform glowRect = feedbackGlow.rectTransform;
        glowRect.position = target.position;
        glowRect.anchorMin = target.anchorMin;
        glowRect.anchorMax = target.anchorMin;
        glowRect.pivot = target.pivot;
        glowRect.anchoredPosition = target.anchoredPosition;
        glowRect.sizeDelta = target.rect.size;
        feedbackGlow.sprite = impactSprite;
        feedbackGlow.preserveAspect = true;

        // Текст - над портретом
        RectTransform textRect = feedbackText.rectTransform;
        textRect.anchorMin = target.anchorMin;
        textRect.anchorMax = target.anchorMin;
        textRect.pivot = new Vector2(0.5f, 0f);
        float offsetX = target.rect.width * (0.5f - target.pivot.x);
        float offsetY = target.rect.height * (0.75f - target.pivot.y);
        textRect.anchoredPosition = target.anchoredPosition + new Vector2(offsetX, offsetY);
        textRect.sizeDelta = new Vector2(220, 38);

        feedbackText.text = text;
        feedbackText.color = color;

        // Крит - більшим шрифтом
        if (text.Contains("CRITICAL"))
        {
            feedbackText.fontSize = 22;
        }
        else
        {
            feedbackText.fontSize = 18;
        }

        feedbackRoutine = StartCoroutine(Feedback(color, showImpact));
    }

    // Анімація тексту: летить вгору і зникає
    private IEnumerator Feedback(Color color, bool showImpact)
    {
        // При промаху спалах не показуємо
        bool impact = showImpact && feedbackText.text != "MISS";
        feedbackGlow.gameObject.SetActive(impact);
        feedbackText.gameObject.SetActive(true);

        float duration = Mathf.Max(0.1f, feedbackDuration);
        Vector2 origin = feedbackText.rectTransform.anchoredPosition;

        for (float elapsed = 0f; elapsed < duration; elapsed = elapsed + Time.unscaledDeltaTime)
        {
            // Спалах: з'являється і зникає (максимум 35% непрозорості)
            color.a = Mathf.Sin(elapsed / duration * Mathf.PI) * 0.35f;
            feedbackGlow.color = color;

            // Текст піднімається на 22 пікселі
            feedbackText.rectTransform.anchoredPosition = origin + Vector2.up * (22f * elapsed / duration);

            // Текст зникає в останній чверті часу
            Color textColor = feedbackText.color;
            textColor.a = Mathf.Clamp01((1f - elapsed / duration) * 4f);
            feedbackText.color = textColor;

            yield return null;
        }

        feedbackGlow.gameObject.SetActive(false);
        feedbackText.gameObject.SetActive(false);
        feedbackRoutine = null;
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
    }

    // Показати панель перемоги
    public void ShowVictoryResults(BattleUnit player)
    {
        if (resultPanel == null || resultText == null)
        {
            return;
        }

        resultPanel.gameObject.SetActive(true);
        resultText.text = "VICTORY\n" + player.displayName + " stands victorious.";

        if (hud != null)
        {
            hud.RevealResult();
        }
    }

    // Показати панель поразки
    public void ShowDefeatResults()
    {
        if (resultPanel == null || resultText == null)
        {
            return;
        }

        resultPanel.color = new Color32(43, 24, 35, 248);
        resultPanel.gameObject.SetActive(true);
        resultText.text = "DEFEAT\nYour journey ends here.";

        if (hud != null)
        {
            hud.RevealResult();
        }
    }

    // Показати учасників бою
    public void ShowUnits(BattleUnit player, BattleUnit enemy)
    {
        ApplyPortrait(playerPortrait, player);
        ApplyPortrait(enemyPortrait, enemy);
        RefreshStats(player, enemy, 0);

        if (hud != null)
        {
            hud.Bind(player, enemy);
        }
    }

    // Поставити картинку, колір і віддзеркалення портрета
    private void ApplyPortrait(Image image, BattleUnit unit)
    {
        image.sprite = PointSprite(unit.battleSprite);
        image.color = unit.spriteColor;

        float scaleX = 1f;
        if (unit.flipInBattle)
        {
            scaleX = -1f;
        }
        image.rectTransform.localScale = new Vector3(scaleX, 1f, 1f);
    }

    // Повертає копію спрайта з піксельною фільтрацією (Point), щоб піксель-арт не "милився".
    // Оригінальну текстуру і її налаштування імпорту НЕ чіпаємо.
    private Sprite PointSprite(Sprite source)
    {
        // Якщо вже Point - копія не потрібна
        if (source == null || source.texture.filterMode == FilterMode.Point)
        {
            return source;
        }

        // Якщо копію вже робили - беремо з кешу
        Sprite copy;
        if (pointSprites.TryGetValue(source, out copy))
        {
            return copy;
        }

        // Імпортовані текстури можуть бути "нечитабельними" з коду.
        // Тому копіюємо через відеокарту (RenderTexture), а не через Instantiate(Texture2D),
        // який Unity для таких текстур не дозволяє.
        Texture2D original = source.texture;
        RenderTexture temporary = RenderTexture.GetTemporary(original.width, original.height, 0);
        RenderTexture previous = RenderTexture.active;
        Texture2D texture = new Texture2D(original.width, original.height, TextureFormat.RGBA32, false);
        try
        {
            Graphics.Blit(original, temporary);
            RenderTexture.active = temporary;
            texture.ReadPixels(new Rect(0, 0, original.width, original.height), 0, 0);
            texture.Apply(false, true);
        }
        finally
        {
            // Обов'язково повертаємо все як було
            RenderTexture.active = previous;
            RenderTexture.ReleaseTemporary(temporary);
        }

        texture.filterMode = FilterMode.Point;

        Vector2 pivot = new Vector2(source.pivot.x / source.rect.width, source.pivot.y / source.rect.height);
        copy = Sprite.Create(texture, source.rect, pivot, source.pixelsPerUnit);
        pointSprites.Add(source, copy);
        return copy;
    }

    private void OnDestroy()
    {
        // Видаляємо всі створені копії (інакше буде витік пам'яті)
        foreach (Sprite sprite in pointSprites.Values)
        {
            Destroy(sprite.texture);
            Destroy(sprite);
        }
        pointSprites.Clear();
    }

    // Оновити тексти статистики
    public void RefreshStats(BattleUnit player, BattleUnit enemy, int round)
    {
        playerStats.text = Stats(player);

        string enemyText = Stats(enemy);
        if (enemy.IsDefending)
        {
            enemyText = enemyText + "\nDEFENDING";
        }
        enemyStats.text = enemyText;

        roundText.text = "ROUND " + round;

        if (enemy.IsAlive)
        {
            enemyPortrait.color = enemy.spriteColor;
        }
        else if (GetComponent<BattlePresentation>() == null)
        {
            // Якщо немає BattlePresentation - мертвого ворога просто робимо сірим
            enemyPortrait.color = new Color(0.35f, 0.35f, 0.35f, 0.35f);
        }
    }

    // Текст статистики одного учасника
    private static string Stats(BattleUnit unit)
    {
        string text = unit.displayName;
        text = text + "\nHP " + unit.Health.currentHealth + " / " + unit.Health.maxHealth;
        text = text + "   MP " + unit.CurrentMP + " / " + unit.MaxMP;
        text = text + "\nATK " + unit.EffectiveAttack + "   DEF " + unit.EffectiveDefense + "   SPD " + unit.Speed;
        if (unit.IsDefending)
        {
            text = text + "\nDEFENDING";
        }
        return text;
    }
}
