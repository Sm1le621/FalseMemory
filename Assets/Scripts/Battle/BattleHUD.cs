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
                "EXP " + player.CurrentExperience + (player.IsDefending ? "   |   DEFEND" : "");
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
        if (menuTitle != null) menuTitle.text = submenu ? "SELECT / ESC - BACK" : "COMMANDS";
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
            int icon=caption.StartsWith("Attack")?0:caption.StartsWith("Magic")||caption.StartsWith("Fire")||caption.StartsWith("Heal")?1:caption.StartsWith("Items")||caption.StartsWith("Potion")?2:caption.StartsWith("Defend")?3:caption.StartsWith("Flee")?4:-1;
            var image=iconTransform.GetComponent<Image>();image.sprite=icon>=0 && commandIcons!=null && icon<commandIcons.Length?commandIcons[icon]:null;
            image.gameObject.SetActive(image.sprite!=null);image.color=label.color;
        }
    }

#if UNITY_EDITOR
    public void BuildCommandIcons(BattleUI ui)
    {
        foreach(var button in ui.commandButtons)
        {
            var icon=GetPanel(button.transform,"CommandIcon");Place(icon.rectTransform,12,5,16,16);icon.preserveAspect=true;
            var label=button.GetComponentInChildren<TextMeshProUGUI>();label.rectTransform.offsetMin=new Vector2(36,0);
        }
        UnityEditor.EditorUtility.SetDirty(this);
    }
    // Editor-only upgrade of existing objects, invoked through Unity MCP.
    public void BuildLayout(BattleUI ui)
    {
        var canvas = ui.messageText.GetComponentInParent<Canvas>();
        var root = canvas.transform;
        var scaler = canvas.GetComponent<CanvasScaler>();
        scaler.referenceResolution = new Vector2(960, 540); scaler.matchWidthOrHeight = 1f;
        canvas.pixelPerfect = true;
        var existing = root.Find("BattleContent");
        if (existing == null)
        {
            content = new GameObject("BattleContent", typeof(RectTransform)).GetComponent<RectTransform>();
            content.SetParent(root, false);
            var children = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in root) if (child != content && child.name != "Backdrop") children.Add(child);
            foreach (var child in children) child.SetParent(content, false);
        }
        else content = (RectTransform)existing;
        content.anchorMin = content.anchorMax = new Vector2(0.5f, 0.5f);
        content.pivot = new Vector2(0.5f, 0.5f); content.sizeDelta = new Vector2(960,540); content.anchoredPosition = Vector2.zero;
        var backdrop = root.Find("Backdrop").GetComponent<Image>(); backdrop.color = new Color32(12, 12, 20,255);
        Place(content.Find("Battlefield") as RectTransform, 24, 228, 912, 194);
        Frame(content.Find("Battlefield").GetComponent<Image>(), new Color32(23,23,33,255));
        Place(content.Find("StatusPanel") as RectTransform, 24, 24, 564, 188);
        Frame(content.Find("StatusPanel").GetComponent<Image>(), new Color32(24,22,33,255));
        Place(content.Find("CommandPanel") as RectTransform, 604, 24, 332, 188);
        Frame(content.Find("CommandPanel").GetComponent<Image>(), new Color32(24,22,33,255));
        var header = GetPanel(content, "EncounterPanel"); Place(header.rectTransform,24,438,912,78); Frame(header,new Color32(24,22,33,255)); header.transform.SetAsFirstSibling();
        Place(ui.enemyStats.rectTransform,44,474,650,25); Format(ui.enemyStats,16);
        Place(ui.messageText.rectTransform,44,443,832,27); Format(ui.messageText,17);
        Place(ui.roundText.rectTransform,780,480,132,22); Format(ui.roundText,12); ui.roundText.alignment = TextAlignmentOptions.MidlineRight;
        Place(ui.playerPortrait.rectTransform,662,242,144,166); Place(ui.enemyPortrait.rectTransform,154,242,144,166);
        ui.playerPortrait.rectTransform.pivot=ui.enemyPortrait.rectTransform.pivot=new Vector2(0.5f,0.5f);
        ui.playerPortrait.rectTransform.anchoredPosition=new Vector2(734,325);ui.enemyPortrait.rectTransform.anchoredPosition=new Vector2(226,325);
        ui.playerPortrait.preserveAspect = ui.enemyPortrait.preserveAspect = true;
        Place(ui.playerStats.rectTransform,44,80,420,114); Format(ui.playerStats,19); ui.playerStats.lineSpacing = 13;
        resourceText = GetLabel(content, "SessionResources"); Place(resourceText.rectTransform,44,40,520,25); Format(resourceText,13);
        lowHealthText = GetLabel(content,"LowHealth"); Place(lowHealthText.rectTransform,455,169,110,23); Format(lowHealthText,13); lowHealthText.color = new Color32(255,139,116,255); lowHealthText.gameObject.SetActive(false);
        hpBar = GetBar(content,"PlayerHPBar",new Color32(86,168,121,255)); Place(hpBar.GetComponent<RectTransform>(),285,133,275,12);
        mpBar = GetBar(content,"PlayerMPBar",new Color32(96,134,216,255)); Place(mpBar.GetComponent<RectTransform>(),285,94,275,12);
        enemyHPBar = GetBar(content,"EnemyHPBar",new Color32(185,99,111,255)); Place(enemyHPBar.GetComponent<RectTransform>(),710,450,202,8);
        menuTitle = GetLabel(content,"MenuTitle"); Place(menuTitle.rectTransform,622,183,296,18); Format(menuTitle,10); menuTitle.text = "COMMANDS";
        var menuRoot = content.Find("CommandRows");
        if (menuRoot == null) menuRoot = new GameObject("CommandRows",typeof(RectTransform),typeof(CanvasGroup)).transform;
        menuRoot.SetParent(content,false); Place((RectTransform)menuRoot,620,35,300,142); menuGroup=menuRoot.GetComponent<CanvasGroup>();
        for(int i=0;i<ui.commandButtons.Length;i++)
        {
            var button = ui.commandButtons[i]; button.transform.SetParent(menuRoot,false);
            Place(button.GetComponent<RectTransform>(),0,114-i*28,300,26);
            button.targetGraphic.color = Color.white;
            var label = ui.commandLabels[i]; Format(label,14); label.rectTransform.anchorMin=Vector2.zero; label.rectTransform.anchorMax=Vector2.one; label.rectTransform.offsetMin=new Vector2(12,0); label.rectTransform.offsetMax=new Vector2(-8,0);
            StyleCommand(button,label,i==0,true);
        }
        var controls = content.Find("Controls").GetComponent<TextMeshProUGUI>();
        Place(controls.rectTransform,30,4,900,16); Format(controls,10); controls.alignment=TextAlignmentOptions.Center; controls.text="WASD / стрелки • Enter — выбор • Esc — назад • Space — быстрее";
        Place(ui.resultPanel.rectTransform,310,238,340,178); Frame(ui.resultPanel,new Color32(29,25,40,248));
        Format(ui.resultText,22); ui.resultText.lineSpacing = 8;
        ui.resultPanel.transform.SetAsLastSibling(); ui.resultPanel.gameObject.SetActive(false);
        ui.feedbackGlow.transform.SetAsLastSibling(); ui.feedbackText.transform.SetAsLastSibling();
        foreach(var text in content.GetComponentsInChildren<TextMeshProUGUI>(true)) text.overflowMode=TextOverflowModes.Truncate;
        UnityEditor.EditorUtility.SetDirty(this); UnityEditor.EditorUtility.SetDirty(ui);
    }

    private static void Place(RectTransform rect,float x,float y,float width,float height)
    {
        rect.anchorMin=rect.anchorMax=Vector2.zero; rect.pivot=Vector2.zero; rect.anchoredPosition=new Vector2(x,y); rect.sizeDelta=new Vector2(width,height);
    }
    private static void Frame(Image image,Color color)
    {
        image.color=color;
        var outline=image.GetComponent<Outline>(); if(outline==null) outline=image.gameObject.AddComponent<Outline>();
        outline.effectColor=new Color32(132,114,88,255); outline.effectDistance=new Vector2(1,-1); outline.useGraphicAlpha=false;
    }
    private static Image GetPanel(Transform parent,string name)
    {
        var child=parent.Find(name); if(child!=null) return child.GetComponent<Image>();
        var obj=new GameObject(name,typeof(RectTransform),typeof(Image)); obj.transform.SetParent(parent,false);
        var image=obj.GetComponent<Image>(); image.raycastTarget=false; return image;
    }
    private static TextMeshProUGUI GetLabel(Transform parent,string name)
    {
        var child=parent.Find(name); if(child!=null) return child.GetComponent<TextMeshProUGUI>();
        var obj=new GameObject(name,typeof(RectTransform),typeof(TextMeshProUGUI)); obj.transform.SetParent(parent,false);
        var label=obj.GetComponent<TextMeshProUGUI>(); label.font=TMP_Settings.defaultFontAsset; label.color=new Color32(224,216,204,255); label.raycastTarget=false; return label;
    }
    private static void Format(TextMeshProUGUI label,int size)
    {
        label.fontSize=size; label.enableAutoSizing=true; label.fontSizeMin=size-2; label.fontSizeMax=size; label.raycastTarget=false;
    }
    private static Slider GetBar(Transform parent,string name,Color color)
    {
        var existing=parent.Find(name); if(existing!=null) return existing.GetComponent<Slider>();
        var track=GetPanel(parent,name); track.color=new Color32(12,12,20,255);
        var fill=GetPanel(track.transform,"Fill"); fill.color=color;
        fill.rectTransform.anchorMin=Vector2.zero; fill.rectTransform.anchorMax=Vector2.one; fill.rectTransform.offsetMin=fill.rectTransform.offsetMax=Vector2.zero;
        var slider=track.gameObject.AddComponent<Slider>(); slider.fillRect=fill.rectTransform; slider.minValue=0; slider.maxValue=1; slider.interactable=false;
        slider.navigation=new Navigation{mode=Navigation.Mode.None}; slider.transition=Selectable.Transition.None; return slider;
    }
#endif
}
