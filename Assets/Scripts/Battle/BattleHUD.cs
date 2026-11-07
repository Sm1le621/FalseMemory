using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Інтерфейс бою: смужки HP/MP, напис "LOW HP", поява меню і екрана.
// Тільки ЧИТАЄ значення з Health/BattleUnit, нічого в них не змінює.
[DisallowMultipleComponent]
public sealed class BattleHUD : MonoBehaviour
{
    // Смужки HP і MP гравця, HP ворога
    public Slider hpBar;
    public Slider mpBar;
    public Slider enemyHPBar;

    // Текст стану ("DEFENDING"), напис "LOW HP", заголовок меню
    public TextMeshProUGUI resourceText;
    public TextMeshProUGUI lowHealthText;
    public TextMeshProUGUI menuTitle;

    // Весь вміст HUD (його масштабуємо під розмір екрана)
    public RectTransform content;

    // Групи для плавної появи (через alpha)
    public CanvasGroup menuGroup;
    public CanvasGroup screenGroup;
    public CanvasGroup resultGroup;

    // Іконки команд: 0 - атака, 1 - магія, 2 - захист, 3 - втеча
    public Sprite[] commandIcons;

    [Range(0f, 1f)]
    public float revealDuration = 0.22f;
    // Скільки секунд смужка "доїжджає" до нового значення
    [Min(0f)]
    public float barDuration = 0.22f;
    [Min(0f)]
    public float menuDuration = 0.12f;
    // Нижче якої частки HP показувати "LOW HP" (0.25 = 25%)
    [Range(0f, 1f)]
    public float lowHealthThreshold = 0.25f;

    private BattleUnit player;
    private BattleUnit enemy;
    // false - смужки ще ні разу не оновлювались (тоді ставимо значення одразу, без анімації)
    private bool initialized;
    private Coroutine menuRoutine;

    // Прив'язати HUD до гравця і ворога
    public void Bind(BattleUnit hero, BattleUnit foe)
    {
        player = hero;
        enemy = foe;
        initialized = false;
        Refresh();
    }

    private void Update()
    {
        Refresh();

        // Якщо екран вужчий за 960 пікселів - зменшуємо HUD
        if (content != null)
        {
            RectTransform canvasRect = content.parent as RectTransform;
            float scale = 1f;
            if (canvasRect != null)
            {
                scale = Mathf.Min(1f, canvasRect.rect.width / 960f);
            }
            content.localScale = Vector3.one * Mathf.Max(0.1f, scale);
        }
    }

    // Оновити всі смужки і написи
    private void Refresh()
    {
        if (player == null || enemy == null)
        {
            return;
        }

        SetBar(hpBar, player.Health.currentHealth, player.Health.maxHealth);
        SetBar(mpBar, player.CurrentMP, player.MaxMP);
        SetBar(enemyHPBar, enemy.Health.currentHealth, enemy.Health.maxHealth);

        if (resourceText != null)
        {
            if (player.IsDefending)
            {
                resourceText.text = "DEFENDING";
            }
            else
            {
                resourceText.text = "";
            }
        }

        // Мало здоров'я?
        bool low = player.IsAlive && player.Health.currentHealth <= player.Health.maxHealth * lowHealthThreshold;

        if (lowHealthText != null)
        {
            lowHealthText.gameObject.SetActive(low);
            lowHealthText.text = "LOW HP";
        }

        // Колір смужки HP: червоний, якщо мало, інакше зелений
        if (hpBar != null && hpBar.fillRect != null)
        {
            Image fill = hpBar.fillRect.GetComponent<Image>();
            if (low)
            {
                fill.color = new Color32(222, 96, 89, 255);
            }
            else
            {
                fill.color = new Color32(86, 168, 121, 255);
            }
        }

        initialized = true;
    }

    // Плавно рухаємо смужку до потрібного значення
    private void SetBar(Slider bar, int value, int maximum)
    {
        if (bar == null)
        {
            return;
        }

        // Частка від 0 до 1
        float target = 0f;
        if (maximum > 0)
        {
            target = Mathf.Clamp01((float)value / maximum);
        }

        float current;
        if (initialized && barDuration > 0f)
        {
            current = Mathf.MoveTowards(bar.value, target, Time.unscaledDeltaTime / barDuration);
        }
        else
        {
            // Перший раз - ставимо одразу
            current = target;
        }

        // WithoutNotify - щоб не викликати події слайдера
        bar.SetValueWithoutNotify(current);
    }

    // Меню змінилось (головне або підменю заклинань)
    public void MenuChanged(bool submenu)
    {
        if (menuTitle != null)
        {
            if (submenu)
            {
                menuTitle.text = "SPELLS";
            }
            else
            {
                menuTitle.text = "COMMANDS";
            }
        }

        if (menuGroup == null || !isActiveAndEnabled)
        {
            return;
        }

        // Якщо попередня анімація ще йде - зупиняємо
        if (menuRoutine != null)
        {
            StopCoroutine(menuRoutine);
        }
        menuRoutine = StartCoroutine(RevealMenu());
    }

    // Сховати HUD перед появою бою
    public void HideForEntrance()
    {
        if (isActiveAndEnabled && screenGroup != null)
        {
            screenGroup.alpha = 0f;
        }
    }

    // Плавно показати HUD. pace - темп бою (actionDelay)
    public IEnumerator RevealScreen(float pace)
    {
        if (screenGroup == null || !isActiveAndEnabled)
        {
            yield break;
        }

        float duration = Mathf.Clamp(revealDuration, 0, 1) * Mathf.Clamp(pace / 0.8f, 0.05f, 3f);

        for (float t = 0; t < duration; t = t + Time.unscaledDeltaTime)
        {
            screenGroup.alpha = Mathf.SmoothStep(0, 1, t / duration);
            yield return null;
        }
        screenGroup.alpha = 1f;
    }

    // Показати панель результату (перемога / поразка)
    public void RevealResult()
    {
        if (resultGroup != null && isActiveAndEnabled)
        {
            StartCoroutine(ResultFade());
        }
    }

    private IEnumerator ResultFade()
    {
        float duration = Mathf.Clamp(revealDuration, 0, 1);

        for (float t = 0; t < duration; t = t + Time.unscaledDeltaTime)
        {
            resultGroup.alpha = Mathf.SmoothStep(0, 1, t / duration);
            yield return null;
        }
        resultGroup.alpha = 1f;
    }

    // Якщо HUD вимкнули посеред анімації - робимо все видимим
    private void OnDisable()
    {
        if (screenGroup != null)
        {
            screenGroup.alpha = 1f;
        }
        if (resultGroup != null)
        {
            resultGroup.alpha = 1f;
        }
    }

    // Меню "проявляється" з напівпрозорого до повністю видимого
    private IEnumerator RevealMenu()
    {
        float duration = Mathf.Clamp(menuDuration, 0f, 0.5f);

        for (float t = 0f; t < duration; t = t + Time.unscaledDeltaTime)
        {
            menuGroup.alpha = Mathf.Lerp(0.55f, 1f, t / duration);
            yield return null;
        }

        menuGroup.alpha = 1f;
        menuRoutine = null;
    }

    // Розфарбувати кнопку команди (вибрана / звичайна / вимкнена) і поставити іконку
    public void StyleCommand(Button button, TextMeshProUGUI label, bool selected, bool enabled)
    {
        if (button == null || label == null)
        {
            return;
        }

        // Кольори фону кнопки
        ColorBlock colors = button.colors;
        if (enabled)
        {
            if (selected)
            {
                colors.normalColor = new Color32(72, 58, 77, 255);
            }
            else
            {
                colors.normalColor = new Color32(32, 29, 41, 255);
            }
        }
        else
        {
            colors.normalColor = new Color32(24, 22, 30, 255);
        }
        colors.highlightedColor = new Color32(82, 68, 89, 255);
        colors.pressedColor = new Color32(106, 84, 96, 255);
        colors.disabledColor = new Color32(24, 22, 30, 255);
        colors.fadeDuration = 0.08f;
        button.colors = colors;

        // Колір тексту
        if (enabled)
        {
            if (selected)
            {
                label.color = new Color32(255, 226, 158, 255);
            }
            else
            {
                label.color = new Color32(224, 216, 204, 255);
            }
        }
        else
        {
            label.color = new Color32(119, 111, 123, 255);
        }

        // Іконка команди (дочірній об'єкт з ім'ям "CommandIcon")
        Transform iconTransform = button.transform.Find("CommandIcon");
        if (iconTransform != null)
        {
            // Прибираємо пробіли і стрілку "> " на початку тексту
            string caption = label.text.TrimStart(' ', '>');

            // Яку іконку показати (-1 = ніяку)
            int icon = -1;
            if (caption.StartsWith("Attack"))
            {
                icon = 0;
            }
            else if (caption.StartsWith("Magic") || caption.StartsWith("Fire"))
            {
                icon = 1;
            }
            else if (caption.StartsWith("Defend"))
            {
                icon = 2;
            }
            else if (caption.StartsWith("Flee"))
            {
                icon = 3;
            }

            Image image = iconTransform.GetComponent<Image>();
            if (icon >= 0 && commandIcons != null && icon < commandIcons.Length)
            {
                image.sprite = commandIcons[icon];
            }
            else
            {
                image.sprite = null;
            }

            // Якщо картинки немає - ховаємо іконку
            image.gameObject.SetActive(image.sprite != null);
            image.color = label.color;
        }
    }
}
