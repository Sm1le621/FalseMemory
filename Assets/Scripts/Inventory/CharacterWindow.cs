using System;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[DisallowMultipleComponent,RequireComponent(typeof(BattleUnit),typeof(WorldInputGate))]
public sealed class CharacterWindow : MonoBehaviour
{
    public RectTransform panel,tooltipPanel;
    public Image backdrop;
    public TextMeshProUGUI header,points,attributeDetail,combatStats,combatWeapon,abilityDetail,equipmentDetail,tooltipText,learningPoints;
    public Button closeButton,learnButton;
    public Button[] tabs,abilityButtons,equipmentButtons;
    public RectTransform[] pages;
    public CharacterAttributeView[] attributeRows;
    private BattleUnit unit;
    private WorldInputGate gate;
    private InventoryWindow inventory;
    private int selectedAbility,selectedEquipment,selectedTab,lastAllocationFrame=-1,lastLearningFrame=-1;
    private CharacterAttribute selectedAttribute;
    private int shownHP=-1,shownMP=-1;
    private Vector2 tooltipPosition;
    public bool IsOpen=>panel!=null && panel.gameObject.activeSelf;
    public int SelectedTab=>selectedTab;
    public BattleUnit Unit=>unit;
    private void Awake()
    {
        unit=GetComponent<BattleUnit>();gate=GetComponent<WorldInputGate>();inventory=GetComponent<InventoryWindow>();
        closeButton.onClick.AddListener(Close);learnButton.onClick.AddListener(LearnSelected);
        for(int i=0;i<tabs.Length;i++){int index=i;tabs[i].onClick.AddListener(()=>SelectTab(index));}
        foreach(var row in attributeRows){var stat=row.attribute;row.increase.onClick.AddListener(()=>Allocate(stat));}
        for(int i=0;i<abilityButtons.Length;i++){int index=i;abilityButtons[i].onClick.AddListener(()=>SelectAbility(index));}
        for(int i=0;i<equipmentButtons.Length;i++){int index=i;equipmentButtons[i].onClick.AddListener(()=>{selectedEquipment=index;Refresh();});}
        Close();
    }
    private void OnEnable(){if(unit==null)return;unit.StatsChanged+=Refresh;unit.ProgressChanged+=Refresh;if(unit.Learning!=null)unit.Learning.Changed+=Refresh;}
    private void OnDisable(){if(unit!=null){unit.StatsChanged-=Refresh;unit.ProgressChanged-=Refresh;if(unit.Learning!=null)unit.Learning.Changed-=Refresh;}Close();}
    private void OnDestroy()
    {
        // The buttons belong exclusively to this authored window.
        if(closeButton!=null)closeButton.onClick.RemoveAllListeners();if(learnButton!=null)learnButton.onClick.RemoveAllListeners();
        foreach(var b in tabs ?? Array.Empty<Button>())if(b!=null)b.onClick.RemoveAllListeners();
        foreach(var b in abilityButtons ?? Array.Empty<Button>())if(b!=null)b.onClick.RemoveAllListeners();
        foreach(var b in equipmentButtons ?? Array.Empty<Button>())if(b!=null)b.onClick.RemoveAllListeners();
        foreach(var row in attributeRows ?? Array.Empty<CharacterAttributeView>())if(row!=null && row.increase!=null)row.increase.onClick.RemoveAllListeners();
    }
    private void Update()
    {
        var keyboard=Keyboard.current;
        if(keyboard!=null && keyboard.cKey.wasPressedThisFrame){if(IsOpen)Close();else Open();return;}
        if(!IsOpen)return;
        if(keyboard!=null && (keyboard.escapeKey.wasPressedThisFrame || keyboard.iKey.wasPressedThisFrame)){Close();return;}
        if(!unit.IsAlive || BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive){Close();return;}
        if(shownHP!=unit.Health.currentHealth || shownMP!=unit.CurrentMP)RefreshHeader();
    }
    public bool Open()
    {
        if(panel==null || !isActiveAndEnabled || unit==null || !unit.IsAlive || !unit.UsesPrimaryAttributes || unit.Learning==null ||
           BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive)return false;
        if(inventory!=null && inventory.IsOpen)inventory.Close();
        if(gate.IsBlocked && !IsOpen)return false;
        backdrop.gameObject.SetActive(true);panel.gameObject.SetActive(true);panel.SetAsLastSibling();tooltipPanel.SetAsLastSibling();
        gate.Acquire(this);SelectTab(selectedTab);return true;
    }
    public void Close(){HideTooltip();if(panel!=null)panel.gameObject.SetActive(false);if(backdrop!=null)backdrop.gameObject.SetActive(false);if(gate!=null)gate.Release(this);}
    public void SelectTab(int index)
    {
        if(index<0 || index>=pages.Length)return;selectedTab=index;HideTooltip();
        for(int i=0;i<pages.Length;i++){pages[i].gameObject.SetActive(i==index);tabs[i].image.color=i==index?new Color32(100,77,47,255):new Color32(37,37,44,255);}Refresh();
    }
    private void RefreshHeader()
    {
        shownHP=unit.Health.currentHealth;shownMP=unit.CurrentMP;
        header.text=GameText.Get("character.window_header",unit.LocalizedName,unit.level,unit.CurrentExperience,
            unit.AtMaximumLevel?GameText.Get("character.max_level"):GameText.Get("character.next_experience",unit.ExperienceToNextLevel),shownHP,unit.Health.maxHealth,shownMP,unit.MaxMP);
        points.text=GameText.Get("character.free_points",unit.Attributes.UnspentPoints);
    }
    public void Refresh()
    {
        if(!IsOpen || unit==null)return;RefreshHeader();var data=unit.Attributes;var effective=unit.PrimaryValues;var gear=unit.EquipmentAttributes;
        foreach(var row in attributeRows)
        {
            var stat=row.attribute;row.label.text=GameText.Get("character.base_row",GameText.Get("attribute."+stat.ToString().ToLowerInvariant()+".abbr"),
                CharacterPresentation.AttributeName(stat),data.BaseValues.Get(stat),gear.Get(stat),effective.Get(stat));
            row.increase.interactable=data.UnspentPoints>0 && data.BaseValues.Get(stat)<99;
        }
        attributeDetail.text=CharacterPresentation.AttributeDescription(unit,selectedAttribute);
        combatStats.text=CharacterPresentation.CombatStats(unit);
        var weapon=unit.Equipment!=null?unit.Equipment.State.GetSlot(EquipmentSlot.Weapon):null;
        combatWeapon.text=weapon!=null?ItemPresentation.Description(weapon,1,unit):GameText.Get("character.unarmed");
        learningPoints.text=GameText.Get("ability.learning_points",unit.Learning.Points);
        for(int i=0;i<abilityButtons.Length;i++)
        {
            var spell=i<unit.Learning.catalog.Length?unit.Learning.catalog[i]:null;abilityButtons[i].gameObject.SetActive(spell!=null);
            if(spell!=null)abilityButtons[i].GetComponentInChildren<TextMeshProUGUI>(true).text=GameText.Get("character.ability_row",spell.LocalizedName,CharacterPresentation.AbilityState(unit.Learning.Availability(spell)));
        }
        var selected=selectedAbility<unit.Learning.catalog.Length?unit.Learning.catalog[selectedAbility]:null;
        abilityDetail.text=CharacterPresentation.AbilityDescription(unit,selected);learnButton.interactable=selected!=null && unit.Learning.Availability(selected)==AbilityAvailability.Available;
        learnButton.GetComponentInChildren<TextMeshProUGUI>(true).text=GameText.Get("ability.learn",selected!=null?selected.learningCost:0);
        for(int i=0;i<equipmentButtons.Length;i++)
        {
            var item=unit.Equipment!=null?unit.Equipment.State.GetSlot((EquipmentSlot)i):null;
            equipmentButtons[i].GetComponentInChildren<TextMeshProUGUI>(true).text=GameText.Get("character.equipment_row",ItemPresentation.SlotName((EquipmentSlot)i),item!=null?item.LocalizedName:GameText.Get("character.empty_slot"));
        }
        var worn=unit.Equipment!=null?unit.Equipment.State.GetSlot((EquipmentSlot)selectedEquipment):null;
        equipmentDetail.text=worn!=null?ItemPresentation.Description(worn,1,unit):GameText.Get("character.equipment_hint");
    }
    public bool Allocate(CharacterAttribute stat)
    {
        if(!IsOpen || lastAllocationFrame==Time.frameCount)return false;lastAllocationFrame=Time.frameCount;
        bool changed=unit.Attributes.TryIncreaseBase(stat);selectedAttribute=stat;HideTooltip();Refresh();return changed;
    }
    public void SelectAbility(int index){if(index<0 || index>=unit.Learning.catalog.Length)return;selectedAbility=index;Refresh();}
    public void LearnSelected()
    {
        if(!IsOpen || lastLearningFrame==Time.frameCount || selectedAbility>=unit.Learning.catalog.Length)return;
        lastLearningFrame=Time.frameCount;unit.Learning.TryLearn(unit.Learning.catalog[selectedAbility]);Refresh();
    }
    public void ShowAttribute(CharacterAttribute stat,Vector2 position,bool tooltip)
    {
        if(!IsOpen)return;selectedAttribute=stat;attributeDetail.text=CharacterPresentation.AttributeDescription(unit,stat);
        if(!tooltip)return;tooltipText.text=attributeDetail.text;tooltipPosition=position;tooltipPanel.gameObject.SetActive(true);tooltipPanel.SetAsLastSibling();PositionTooltip();
    }
    private void LateUpdate(){if(tooltipPanel!=null && tooltipPanel.gameObject.activeSelf)PositionTooltip();}
    private void PositionTooltip()
    {
        var parent=tooltipPanel.parent as RectTransform;float width=Mathf.Min(400,parent.rect.width-24);
        tooltipPanel.sizeDelta=new Vector2(width,Mathf.Min(tooltipText.GetPreferredValues(tooltipText.text,width-32,0).y+32,parent.rect.height-24));
        Vector2 local;RectTransformUtility.ScreenPointToLocalPointInRectangle(parent,tooltipPosition,null,out local);local+=new Vector2(20,-20);
        local.x=Mathf.Clamp(local.x,parent.rect.xMin+8,parent.rect.xMax-tooltipPanel.rect.width-8);
        local.y=Mathf.Clamp(local.y,parent.rect.yMin+tooltipPanel.rect.height+8,parent.rect.yMax-8);
        tooltipPanel.anchoredPosition=local;tooltipPanel.gameObject.SetActive(true);tooltipPanel.SetAsLastSibling();
    }
    public void HideTooltip(){if(tooltipPanel!=null)tooltipPanel.gameObject.SetActive(false);}
}
