using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BattleUnit),typeof(WorldInputGate))]
public sealed class PlayerInteractor : MonoBehaviour
{
    [Min(0.1f)] public float interactionRadius=1.2f;
    [Range(0.05f,0.5f)] public float scanInterval=0.15f;
    public TextMeshProUGUI prompt, notification;
    public IWorldInteractable Current {get;private set;}
    private BattleUnit unit;
    private WorldInputGate gate;
    private float nextScan, notificationUntil;
    private readonly List<Collider2D> nearby=new List<Collider2D>();
    private void Awake(){unit=GetComponent<BattleUnit>();gate=GetComponent<WorldInputGate>();}
    private void Update()
    {
        if(notification!=null && Time.unscaledTime>=notificationUntil)notification.text="";
        if(unit==null || !unit.IsAlive || gate.IsBlocked || (BattleTransition.Instance!=null && BattleTransition.Instance.EncounterActive))
        {Current=null;if(prompt!=null)prompt.text="";return;}
        if(Time.unscaledTime>=nextScan){Scan();nextScan=Time.unscaledTime+Mathf.Clamp(scanInterval,0.05f,0.5f);}
        if(prompt!=null)prompt.text=Current!=null && Current.Available?Current.Prompt:"";
        if(Keyboard.current!=null && Keyboard.current.eKey.wasPressedThisFrame && Current!=null)
        {
            Current.TryInteract(unit,out var text);ShowNotification(text);Current=null;nextScan=0;
            if(prompt!=null)prompt.text="";
        }
    }
    private void Scan()
    {
        Current=null;float nearest=float.MaxValue;
        var filter=new ContactFilter2D{useTriggers=true};
        Physics2D.OverlapCircle(transform.position,Mathf.Max(0.1f,interactionRadius),filter,nearby);
        foreach(var collider in nearby)
        {
            var target=collider.GetComponent<IWorldInteractable>();
            if(target==null || !target.Available)continue;
            float distance=((Vector2)collider.transform.position-(Vector2)transform.position).sqrMagnitude;
            float radius=Mathf.Max(0.1f,interactionRadius);
            if(distance<=radius*radius && distance<nearest){nearest=distance;Current=target;}
        }
    }
    public void ShowNotification(string text){if(notification!=null)notification.text=text;notificationUntil=Time.unscaledTime+2.5f;}
    private void OnDisable(){Current=null;nearby.Clear();if(prompt!=null)prompt.text="";if(notification!=null)notification.text="";}
}
