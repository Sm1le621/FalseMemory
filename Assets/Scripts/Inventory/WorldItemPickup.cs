using UnityEngine;

public interface IWorldInteractable
{
    string Prompt {get;}
    bool Available {get;}
    bool TryInteract(BattleUnit player,out string message);
}

[RequireComponent(typeof(Collider2D))]
public sealed class WorldItemPickup : MonoBehaviour, IWorldInteractable
{
    public string worldId;
    public ItemData item;
    [Min(1)] public int quantity=1;
    public bool Collected {get;private set;}
    public bool Available => isActiveAndEnabled && !Collected && item!=null && quantity>0;
    public string Prompt => item!=null?"E — подобрать "+item.displayName+" ×"+quantity:"";
    public bool TryInteract(BattleUnit player,out string message)
    {
        message="";
        if(!Available || player==null || !player.IsAlive || player.Inventory==null)return false;
        var gate=player.GetComponent<WorldInputGate>();
        var interactor=player.GetComponent<PlayerInteractor>();
        float radius=interactor!=null?Mathf.Max(0.1f,interactor.interactionRadius):1.2f;
        if((gate!=null && gate.IsBlocked) ||
            ((Vector2)player.transform.position-(Vector2)transform.position).sqrMagnitude>radius*radius)return false;
        // Reserve before notifying inventory observers; reentrant/rapid requests cannot claim twice.
        Collected=true;
        if(!player.Inventory.TryAdd(item,quantity))
        {
            Collected=false;message="Нет места в инвентаре. Предмет остался на земле.";return false;
        }
        message="Получено: "+item.displayName+" ×"+quantity;
        gameObject.SetActive(false);return true;
    }
}
