using System;
using System.Collections.Generic;

// Черга ходів у раунді. Учасники ходять у тому порядку,
// в якому їх передали (у бою першим завжди передається гравець).
// Тут немає нічого від Unity, тому клас легко тестувати.
// Працює з будь-якою кількістю учасників.
public sealed class BattleTurnOrder<T> where T : class, IBattleParticipant
{
    // Учасники в порядку ходів
    private readonly List<T> order = new List<T>();
    // Номер того, хто ходить наступним
    private int cursor;

    // Почати новий раунд: поставити всіх живих у черзі в тому ж порядку
    public void BeginRound(IEnumerable<T> participants)
    {
        order.Clear();
        cursor = 0;

        foreach (T participant in participants)
        {
            // Пропускаємо пустих, мертвих і тих, хто вже є в черзі
            if (participant == null)
            {
                continue;
            }
            if (!participant.IsAlive)
            {
                continue;
            }
            if (order.Contains(participant))
            {
                continue;
            }

            // Ставимо в кінець черги
            order.Add(participant);
        }
    }

    // Взяти наступного живого учасника. Якщо нікого не лишилось - false
    public bool TryNext(out T participant)
    {
        while (cursor < order.Count)
        {
            participant = order[cursor];
            cursor = cursor + 1;

            // Якщо він помер під час раунду - пропускаємо
            if (participant.IsAlive)
            {
                return true;
            }
        }

        participant = null;
        return false;
    }
}

// Які дії можна вибрати в бою
public enum BattleAction { Attack, Defend, Magic, Flee }

// Стан бою
public enum BattleState { Starting, Resolving, AwaitingPlayer, Victory, Defeat, Escaped }

// "Ворота" для команди гравця: команду можна прийняти лише ОДИН раз,
// навіть якщо клік мишкою і Enter прийшли одночасно.
public sealed class BattleCommandGate
{
    public bool IsOpen { get; private set; }

    public void Open()
    {
        IsOpen = true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    // Якщо ворота відкриті - закриваємо їх і повертаємо true
    public bool TryConsume()
    {
        if (!IsOpen)
        {
            return false;
        }

        IsOpen = false;
        return true;
    }
}
