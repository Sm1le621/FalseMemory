using System.Collections.Generic;
using UnityEngine;

// Shared modal input ownership, without changing time scale or component enabled state.
[DisallowMultipleComponent]
public sealed class WorldInputGate : MonoBehaviour
{
    private readonly HashSet<object> owners=new HashSet<object>();
    public bool IsBlocked => owners.Count>0;
    public void Acquire(object owner){if(owner!=null)owners.Add(owner);}
    public void Release(object owner){if(owner!=null)owners.Remove(owner);}
    private void OnDisable() => owners.Clear();
}
