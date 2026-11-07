using UnityEngine;

// Види анімацій. Анімації "стоїть" (Idle) навмисно немає
public enum SpritePose { Move, Attack, Hurt, Death }

// Набір кадрів для всіх анімацій персонажа
[CreateAssetMenu(menuName = "FalseMemory/Sprite Animation Set")]
public sealed class SpriteAnimationSet : ScriptableObject
{
    public Sprite[] move = System.Array.Empty<Sprite>();
    public Sprite[] attack = System.Array.Empty<Sprite>();
    public Sprite[] hurt = System.Array.Empty<Sprite>();
    public Sprite[] death = System.Array.Empty<Sprite>();

    // Швидкість анімації (кадрів за секунду)
    [Range(1f, 24f)]
    public float framesPerSecond = 8f;

    // Повертає масив кадрів для потрібної анімації
    public Sprite[] Frames(SpritePose pose)
    {
        switch (pose)
        {
            case SpritePose.Attack:
                return attack;
            case SpritePose.Hurt:
                return hurt;
            case SpritePose.Death:
                return death;
            default:
                return move;
        }
    }

    // Кадр за прогресом від 0 до 1 (для анімацій, що грають один раз).
    // Якщо кадрів немає - повертаємо fallback.
    public Sprite At(SpritePose pose, float progress, Sprite fallback = null)
    {
        Sprite[] frames = Frames(pose);
        if (frames == null || frames.Length == 0)
        {
            return fallback;
        }

        // Наприклад, 4 кадри і progress 0.5 -> кадр номер 2
        int index = Mathf.FloorToInt(Mathf.Clamp01(progress) * frames.Length);
        // Щоб при progress = 1 не вийти за межі масиву
        if (index > frames.Length - 1)
        {
            index = frames.Length - 1;
        }

        Sprite frame = frames[index];
        // ReferenceEquals - перевірка "чи порожній елемент масиву" без особливої
        // логіки Unity для знищених об'єктів (так працював оператор ?? в оригіналі)
        if (ReferenceEquals(frame, null))
        {
            return fallback;
        }
        return frame;
    }

    // Кадр для анімації, яка крутиться по колу (наприклад, ходьба)
    public Sprite Loop(SpritePose pose, float seconds, Sprite fallback = null)
    {
        Sprite[] frames = Frames(pose);
        if (frames == null || frames.Length == 0)
        {
            return fallback;
        }

        // Скільки кадрів пройшло з початку, і остача від ділення на кількість кадрів
        double safeSeconds = System.Math.Max(0d, seconds);
        float fps = Mathf.Clamp(framesPerSecond, 1, 24);
        int index = (int)(safeSeconds * fps % frames.Length);

        Sprite frame = frames[index];
        if (ReferenceEquals(frame, null))
        {
            return fallback;
        }
        return frame;
    }
}
