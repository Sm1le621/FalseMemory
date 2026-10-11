using System;
using System.Collections.Generic;

// No Unity lifecycle dependencies: independently testable and supports N units.
public sealed class BattleTurnOrder<T> where T : class, IBattleParticipant
{
    private readonly List<T> order = new List<T>();
    private int cursor;

    public void BeginRound(IEnumerable<T> participants)
    {
        order.Clear();
        cursor = 0;
        foreach (T participant in participants)
        {
            if (participant == null || !participant.IsAlive || order.Contains(participant)) continue;
            // Stable ties: the initial party/enemy order wins, consistently.
            int index = order.FindIndex(other => other.Speed < participant.Speed);
            if (index < 0) order.Add(participant); else order.Insert(index, participant);
        }
    }

    public bool TryNext(out T participant)
    {
        while (cursor < order.Count)
        {
            participant = order[cursor++];
            if (participant.IsAlive) return true;
        }
        participant = null;
        return false;
    }
}

public enum BattleAction { Attack, Defend, Magic, Items, Flee }
public enum BattleState { Starting, Resolving, AwaitingPlayer, Victory, Defeat, Escaped }

// A command can be consumed once, even when click and keyboard arrive together.
public sealed class BattleCommandGate
{
    public bool IsOpen { get; private set; }
    public void Open() => IsOpen = true;
    public void Close() => IsOpen = false;
    public bool TryConsume()
    {
        if (!IsOpen) return false;
        IsOpen = false;
        return true;
    }
}
