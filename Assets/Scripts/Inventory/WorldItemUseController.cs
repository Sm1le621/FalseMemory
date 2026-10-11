using UnityEngine;

[RequireComponent(typeof(BattleUnit),typeof(InventoryWindow))]
public sealed class WorldItemUseController : MonoBehaviour
{
    private BattleUnit unit;
    private InventoryWindow window;
    private int lastUseFrame=-1;
    private void Awake(){unit=GetComponent<BattleUnit>();window=GetComponent<InventoryWindow>();}
    private void OnEnable()
    {
        if(window==null)return;window.UseRequested+=HandleUse;window.CanUseSelection=CanUse;
    }
    private void OnDisable()
    {
        if(window==null)return;window.UseRequested-=HandleUse;
        if(window.CanUseSelection==CanUse)window.CanUseSelection=null;
    }
    public bool CanUse(int index)
    {
        if(!isActiveAndEnabled || unit==null || window==null || !window.IsOpen || index<0 || index>=30 ||
            (BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive))return false;
        var item=unit.Inventory.State.GetSlot(index).Item as BattleConsumable;
        return BattleAbilities.CanUseItem(unit,unit,item,ItemUseLocation.Exploration);
    }
    public bool TryUse(int index)
    {
        if(lastUseFrame==Time.frameCount || !CanUse(index))return false;
        var item=(BattleConsumable)unit.Inventory.State.GetSlot(index).Item;
        lastUseFrame=Time.frameCount;
        bool ok=BattleAbilities.TryUseItem(unit,unit,item,out int amount,ItemUseLocation.Exploration);
        window.ShowMessage(ok?item.displayName+": +"+amount+(item.manaRestoration>0?" MP":" HP"):"Предмет недоступен.");window.Refresh();return ok;
    }
    private void HandleUse(int index)=>TryUse(index);
}
