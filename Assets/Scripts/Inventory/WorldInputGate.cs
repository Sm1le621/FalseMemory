using System.Collections.Generic;
using UnityEngine;

// "Замок" на керування у світі.
// Якщо хоча б одне вікно (наприклад, інвентар) "взяло" замок - гравець не рухається.
// Час гри (timeScale) і компоненти при цьому не вимикаються.
[DisallowMultipleComponent]
public sealed class WorldInputGate : MonoBehaviour
{
    // Хто зараз тримає замок
    private readonly HashSet<object> owners = new HashSet<object>();

    // Заблоковано, якщо є хоч один власник
    public bool IsBlocked
    {
        get { return owners.Count > 0; }
    }

    // Взяти замок
    public void Acquire(object owner)
    {
        if (owner != null)
        {
            owners.Add(owner);
        }
    }

    // Віддати замок
    public void Release(object owner)
    {
        if (owner != null)
        {
            owners.Remove(owner);
        }
    }

    private void OnDisable()
    {
        owners.Clear();
    }
}
