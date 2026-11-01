using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Presentation only: targets are read from the original Health/BattleUnit.
[DisallowMultipleComponent]
public sealed class BattleHUD : MonoBehaviour
{
    public Slider hpBar, mpBar, enemyHPBar;
    public TextMeshProUGUI resourceText, lowHealthText, menuTitle;
    public RectTransform content;
    public CanvasGroup menuGroup;
    public CanvasGroup screenGroup, resultGroup;
    public Sprite[] commandIcons;
    [Range(0f,1f)] public float revealDuration = 0.22f;
    [Min(0f)] public float barDuration = 0.22f;
    [Min(0f)] public float menuDuration = 0.12f;
    [Range(0f, 1f)] public float lowHealthThreshold = 0.25f;
    private BattleUnit player, enemy;
    private bool initialized;
    private Coroutine menuRoutine;
    public void Bind(BattleUnit hero, BattleUnit foe)
    {
        player = hero; enemy = foe; initialized = false;
        Refresh();
    }

    private void Update()
    {
        Refresh();
        if (content != null)
        {
            var canvasRect = content.parent as RectTransform;
            float scale = canvasRect != null ? Mathf.Min(1f, canvasRect.rect.width / 960f) : 1f;
            content.localScale = Vector3.one * Mathf.Max(0.1f, scale);
        }
    }

    private void Refresh()
    {
        if (player == null || enemy == null) return;
        SetBar(hpBar, player.Health.currentHealth, player.Health.maxHealth);
        SetBar(mpBar, player.CurrentMP, player.MaxMP);
        SetBar(enemyHPBar, enemy.Health.currentHealth, enemy.Health.maxHealth);
        if (resourceText != null)
            resourceText.text = (player.Inventory != null ? player.Inventory.Summary + "   |   " : "") +
                (player.IsDefending ? "DEFENDING" : "");
        bool low = player.IsAlive && player.Health.currentHealth <= player.Health.maxHealth * lowHealthThreshold;
        if (lowHealthText != null)
        {
            lowHealthText.gameObject.SetActive(low);
            lowHealthText.text = "LOW HP";
        }
        if (hpBar != null && hpBar.fillRect != null)
            hpBar.fillRect.GetComponent<Image>().color = low ? new Color32(222, 96, 89, 255) : new Color32(86, 168, 121, 255);
        initialized = true;
    }

    private void SetBar(Slider bar, int value, int maximum)
    {
        if (bar == null) return;
        float target = maximum > 0 ? Mathf.Clamp01((float)value / maximum) : 0f;
        float current = initialized && barDuration > 0f ? Mathf.MoveTowards(bar.value, target, Time.unscaledDeltaTime / barDuration) : target;
        bar.SetValueWithoutNotify(current);
    }

    public void MenuChanged(bool submenu)
    {
        if (menuTitle != null) menuTitle.text = submenu ? "SPELLS" : "COMMANDS";
        if (menuGroup == null || !isActiveAndEnabled) return;
        if (menuRoutine != null) StopCoroutine(menuRoutine);
        menuRoutine = StartCoroutine(RevealMenu());
    }

    public void HideForEntrance() { if(isActiveAndEnabled && screenGroup!=null) screenGroup.alpha=0f; }
    public IEnumerator RevealScreen(float pace)
    {
        if(screenGroup==null || !isActiveAndEnabled) yield break;
        float duration=Mathf.Clamp(revealDuration,0,1)*Mathf.Clamp(pace/0.8f,0.05f,3f);
        for(float t=0;t<duration;t+=Time.unscaledDeltaTime){screenGroup.alpha=Mathf.SmoothStep(0,1,t/duration);yield return null;}
        screenGroup.alpha=1f;
    }
    public void RevealResult(){if(resultGroup!=null && isActiveAndEnabled)StartCoroutine(ResultFade());}
    private IEnumerator ResultFade()
    {
        float duration=Mathf.Clamp(revealDuration,0,1);
        for(float t=0;t<duration;t+=Time.unscaledDeltaTime){resultGroup.alpha=Mathf.SmoothStep(0,1,t/duration);yield return null;}
        resultGroup.alpha=1f;
    }
    private void OnDisable(){if(screenGroup!=null)screenGroup.alpha=1f;if(resultGroup!=null)resultGroup.alpha=1f;}

    private IEnumerator RevealMenu()
    {
        float duration = Mathf.Clamp(menuDuration, 0f, 0.5f);
        for (float t = 0f; t < duration; t += Time.unscaledDeltaTime)
        {
            menuGroup.alpha = Mathf.Lerp(0.55f, 1f, t / duration);
            yield return null;
        }
        menuGroup.alpha = 1f; menuRoutine = null;
    }

    public void StyleCommand(Button button, TextMeshProUGUI label, bool selected, bool enabled)
    {
        if (button == null || label == null) return;
        var colors = button.colors;
        colors.normalColor = enabled ? (selected ? new Color32(72, 58, 77, 255) : new Color32(32, 29, 41, 255)) : new Color32(24, 22, 30, 255);
        colors.highlightedColor = new Color32(82, 68, 89, 255);
        colors.pressedColor = new Color32(106, 84, 96, 255);
        colors.disabledColor = new Color32(24, 22, 30, 255); colors.fadeDuration = 0.08f;
        button.colors = colors;
        label.color = enabled ? (selected ? new Color32(255, 226, 158, 255) : new Color32(224, 216, 204, 255)) : new Color32(119, 111, 123, 255);
        var iconTransform=button.transform.Find("CommandIcon");
        if(iconTransform!=null)
        {
            string caption=label.text.TrimStart(' ','>');
            int icon=caption.StartsWith("Attack")?0:caption.StartsWith("Magic")||caption.StartsWith("Fire")?1:caption.StartsWith("Defend")?3:caption.StartsWith("Flee")?4:-1;
            var image=iconTransform.GetComponent<Image>();image.sprite=icon>=0 && commandIcons!=null && icon<commandIcons.Length?commandIcons[icon]:null;
            image.gameObject.SetActive(image.sprite!=null);image.color=label.color;
        }
    }

}
