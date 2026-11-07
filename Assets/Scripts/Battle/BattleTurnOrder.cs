using System;
using System.Collections.Generic;

// Черга ходів у раунді. Хто швидший - ходить першим.
// Тут немає нічого від Unity, тому клас легко тестувати.
// Працює з будь-якою кількістю учасників.
public sealed class BattleTurnOrder<T> where T : class, IBattleParticipant
{
    // Учасники в порядку ходів
    private readonly List<T> order = new List<T>();
    // Номер того, хто ходить наступним
    private int cursor;

    // Почати новий раунд: розставити всіх живих за швидкістю
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

            // Шукаємо перше місце, де стоїть хтось ПОВІЛЬНІШИЙ.
            // Якщо швидкість однакова - той, хто був раніше в списку, залишається першим.
            int index = -1;
            for (int i = 0; i < order.Count; i++)
            {
                if (order[i].Speed < participant.Speed)
                {
                    index = i;
                    break;
                }
            }

            if (index < 0)
            {
                // Повільніших немає - ставимо в кінець
                order.Add(participant);
            }
            else
            {
                order.Insert(index, participant);
            }
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
