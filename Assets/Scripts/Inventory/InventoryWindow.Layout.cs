using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public sealed partial class InventoryWindow
{
#if UNITY_EDITOR
    // Reuses every saved slot/button. Safe to repeat; never destroys scene objects.
    public void BuildFantasyLayout()
    {
        if(Application.isPlaying || panel==null || slots==null || slots.Length!=InventoryState.Capacity)
            throw new InvalidOperationException("Нужна существующая сцена с инвентарём вне Play Mode.");
        panel.sizeDelta=new Vector2(1140,720);panel.GetComponent<Image>().color=new Color32(18,19,24,255);
        var scaler=panel.GetComponentInParent<CanvasScaler>();scaler.referenceResolution=new Vector2(1200,780);scaler.screenMatchMode=CanvasScaler.ScreenMatchMode.Expand;
        Place(panel.Find("Title").GetComponent<RectTransform>(),32,662,400,32);
        panel.Find("Title").GetComponent<TextMeshProUGUI>().text="ИНВЕНТАРЬ";
        Place(resourcesLabel.rectTransform,490,662,390,32);resourcesLabel.fontSizeMax=18;
        Place(goldLabel.rectTransform,890,662,218,32);goldLabel.fontSizeMax=18;
        var character=Section("CharacterPanel",24,202,430,442);var bag=Section("BagPanel",474,202,642,442);
        character.SetAsFirstSibling();bag.SetAsFirstSibling();
        EnsureLabel("CharacterTitle","ПЕРСОНАЖ И ЭКИПИРОВКА",16,44,614,390,24);
        bagLabel=EnsureLabel("BagTitle","СУМКА   0 / 30",18,500,614,582,24);
        characterLabel=EnsureLabel("CharacterName","",16,146,550,190,54);characterLabel.alignment=TextAlignmentOptions.Center;
        var portraitFrame=Section("PortraitFrame",158,300,164,246);portraitFrame.GetComponent<Image>().color=new Color32(22,23,29,255);
        portraitFrame.SetSiblingIndex(2);
        characterPortrait=EnsureImage(panel,"CharacterPortrait");Place(characterPortrait.rectTransform,164,304,152,238);characterPortrait.preserveAspect=true;
        characterPortrait.rectTransform.pivot=new Vector2(.5f,.5f);characterPortrait.rectTransform.anchoredPosition=new Vector2(240,423);
        characterPortrait.sprite=GetComponent<BattleUnit>().battleSprite;characterPortrait.raycastTarget=false;
        EnsureLabel("PortraitCaption","FALSE MEMORY",12,170,278,138,22).alignment=TextAlignmentOptions.Center;
        Array.Resize(ref equipmentButtons,EquipmentState.SlotCount);
        // Layout order differs from enum order: serialized Accessory1/2 still remain 5/6.
        var order=new[]{EquipmentSlot.Weapon,EquipmentSlot.Head,EquipmentSlot.Chest,EquipmentSlot.Gloves,EquipmentSlot.Legs,EquipmentSlot.Accessory1,EquipmentSlot.Boots,EquipmentSlot.Accessory2};
        for(int j=0;j<order.Length;j++)
        {
            int index=(int)order[j];float x=j%2==0?64:338;float y=526-(j/2)*100;
            if(equipmentButtons[index]==null)equipmentButtons[index]=Button(panel,"Equipment"+order[j],"",x,y,68,68);
            var button=equipmentButtons[index];Place(button.GetComponent<RectTransform>(),x,y,68,68);button.transition=Selectable.Transition.None;
            var view=button.GetComponent<EquipmentSlotView>();if(view==null)view=button.gameObject.AddComponent<EquipmentSlotView>();
            view.owner=this;view.slot=order[j];view.background=button.GetComponent<Image>();view.rarityBorder=button.GetComponent<Outline>();
            view.slotLabel=button.GetComponentInChildren<TextMeshProUGUI>();Place(view.slotLabel.rectTransform,-22,-25,112,22);
            view.slotLabel.fontSizeMax=13;view.slotLabel.alignment=TextAlignmentOptions.Center;view.slotLabel.text=ItemPresentation.SlotName(order[j]);
            view.icon=EnsureImage(button.transform,"EquipmentIcon");Place(view.icon.rectTransform,10,10,48,48);view.icon.preserveAspect=true;
            view.icon.gameObject.SetActive(false);
        }
        for(int i=0;i<slots.Length;i++)
        {
            var view=slots[i];Place(view.GetComponent<RectTransform>(),514+(i%6)*96,524-(i/6)*76,68,68);
            view.rarityBorder=view.GetComponent<Outline>();view.background.color=ItemPresentation.CellColor(null,false);
            Place(view.icon.rectTransform,10,14,48,48);Place(view.quantity.rectTransform,4,3,60,16);view.quantity.fontSizeMax=13;
        }
        // Preserve old labels as useful parts of the new layout rather than deleting them.
        Place(panel.Find("EquipmentTitle").GetComponent<RectTransform>(),40,170,398,22);
        panel.Find("EquipmentTitle").GetComponent<TextMeshProUGUI>().text="ХАРАКТЕРИСТИКИ";
        Place(equipmentStats.rectTransform,40,96,398,66);equipmentStats.fontSizeMax=18;
        details.richText=false;Place(details.rectTransform,494,84,594,90);details.fontSizeMax=17;
        Place(message.rectTransform,32,54,1076,22);message.fontSizeMax=14;
        Place(panel.Find("Controls").GetComponent<RectTransform>(),32,20,1076,22);
        panel.Find("Controls").GetComponent<TextMeshProUGUI>().text="I / Esc — закрыть   •   Наведение — описание   •   Мышь — перенос   •   Стрелки — выбор";
        MoveButton(useButton,494,184,138);MoveButton(equipButton,642,184,138);MoveButton(splitButton,790,184,138);MoveButton(closeButton,938,184,150);
        if(tooltip==null)tooltip=panel.parent.GetComponent<InventoryTooltip>();
        if(tooltip==null)tooltip=panel.parent.gameObject.AddComponent<InventoryTooltip>();
        tooltip.owner=this;var tip=panel.parent.Find("InventoryTooltip");
        tooltip.panel=tip!=null?(RectTransform)tip:Panel(panel.parent,"InventoryTooltip",new Color32(22,22,28,255));
        tooltip.panel.anchorMin=tooltip.panel.anchorMax=new Vector2(0.5f,0.5f);tooltip.panel.pivot=new Vector2(0,1);tooltip.panel.sizeDelta=new Vector2(320,260);
        tooltip.panel.GetComponent<Image>().raycastTarget=false;tooltip.rarityBorder=tooltip.panel.GetComponent<Outline>();
        var content=tooltip.panel.Find("Content");tooltip.content=content!=null?content.GetComponent<TextMeshProUGUI>():Label(tooltip.panel,"Content","",16,16,16,288,228);
        tooltip.content.rectTransform.anchorMin=Vector2.zero;tooltip.content.rectTransform.anchorMax=Vector2.one;tooltip.content.rectTransform.pivot=new Vector2(.5f,.5f);
        tooltip.content.rectTransform.offsetMin=new Vector2(16,16);tooltip.content.rectTransform.offsetMax=new Vector2(-16,-16);
        tooltip.content.enableAutoSizing=true;tooltip.content.fontSizeMin=12;tooltip.content.fontSizeMax=16;tooltip.content.raycastTarget=false;
        tooltip.content.overflowMode=TextOverflowModes.Ellipsis;
        tooltip.Hide();dragIcon.transform.SetAsLastSibling();panel.gameObject.SetActive(false);
        UnityEditor.EditorUtility.SetDirty(this);UnityEditor.EditorUtility.SetDirty(tooltip);UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(gameObject.scene);
    }
    private RectTransform Section(string name,float x,float y,float w,float h)
    {
        var existing=panel.Find(name);var rect=existing!=null?(RectTransform)existing:Panel(panel,name,new Color32(24,25,30,255));
        Place(rect,x,y,w,h);rect.GetComponent<Image>().raycastTarget=false;return rect;
    }
    private TextMeshProUGUI EnsureLabel(string name,string text,int size,float x,float y,float w,float h)
    {
        var existing=panel.Find(name);var label=existing!=null?existing.GetComponent<TextMeshProUGUI>():Label(panel,name,text,size,x,y,w,h);
        Place(label.rectTransform,x,y,w,h);return label;
    }
    private static Image EnsureImage(Transform parent,string name)
    {
        var existing=parent.Find(name);var image=existing!=null?existing.GetComponent<Image>():Panel(parent,name,Color.white).GetComponent<Image>();image.raycastTarget=false;return image;
    }
    private static void MoveButton(Button button,float x,float y,float width)
    {
        Place(button.GetComponent<RectTransform>(),x,y,width,30);Place(button.GetComponentInChildren<TextMeshProUGUI>().rectTransform,4,2,width-8,26);
        button.GetComponent<Image>().color=new Color32(48,43,39,255);
    }
#endif
}
