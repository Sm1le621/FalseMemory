using UnityEngine;

// Анімація ворога у СВІТІ (не в бою): ходьба, біль, смерть.
// Тут тільки картинка: фізика, AI і бій працюють в інших скриптах.
[RequireComponent(typeof(SpriteRenderer), typeof(BattleUnit))]
[DisallowMultipleComponent]
public sealed class EnemySpriteAnimation : MonoBehaviour
{
    // Мінімальний зсув, щоб вважати, що ворог рухається
    [Min(0f)]
    public float movementThreshold = 0.001f;
    // Скільки секунд показуємо анімацію болю
    [Range(0.05f, 0.5f)]
    public float hurtDuration = 0.16f;
    // Скільки секунд триває анімація смерті
    [Range(0.1f, 1f)]
    public float deathDuration = 0.4f;
    // Повертати спрайт у бік руху
    public bool faceMovement = true;

    private SpriteRenderer visual;
    private BattleUnit unit;
    // Де ворог був у минулому кадрі
    private Vector3 previousPosition;
    // Скільки HP було в минулому кадрі
    private int previousHP;
    // Свій "годинник" (секунди з моменту вмикання)
    private float clock;
    // До якого часу показувати біль
    private float hurtUntil;
    // Коли почалась смерть (-1 = ще не почалась)
    private float deathStarted = -1f;

    private void Awake()
    {
        visual = GetComponent<SpriteRenderer>();
        unit = GetComponent<BattleUnit>();
    }

    private void OnEnable()
    {
        // Скидаємо все при вмиканні
        previousPosition = transform.position;
        if (unit != null)
        {
            previousHP = unit.Health.currentHealth;
        }
        else
        {
            previousHP = 0;
        }
        clock = 0;
        hurtUntil = 0;
        deathStarted = -1;
    }

    private void LateUpdate()
    {
        if (unit == null || visual == null || unit.visualAnimations == null)
        {
            return;
        }

        // На скільки ворог зрушив за цей кадр
        Vector3 displacement = transform.position - previousPosition;
        previousPosition = transform.position;
        clock = clock + Time.deltaTime;

        // Якщо HP зменшилось - починаємо анімацію болю
        int hp = unit.Health.currentHealth;
        if (hp < previousHP)
        {
            hurtUntil = clock + hurtDuration;
        }
        previousHP = hp;

        SpriteAnimationSet set = unit.visualAnimations;

        // 1) Ворог мертвий - показуємо смерть
        if (hp <= 0)
        {
            if (deathStarted < 0)
            {
                deathStarted = clock;
            }
            float deathProgress = (clock - deathStarted) / Mathf.Max(0.1f, deathDuration);
            visual.sprite = set.At(SpritePose.Death, deathProgress, unit.battleSprite);
            return;
        }

        deathStarted = -1;

        // 2) Ворогу боляче - показуємо біль
        if (clock < hurtUntil)
        {
            float hurtProgress = 1 - (hurtUntil - clock) / Mathf.Max(0.05f, hurtDuration);
            visual.sprite = set.At(SpritePose.Hurt, hurtProgress, unit.battleSprite);
            return;
        }

        // 3) Рухається чи стоїть
        bool moving = displacement.sqrMagnitude > movementThreshold * movementThreshold;

        if (faceMovement && moving && Mathf.Abs(displacement.x) > movementThreshold)
        {
            // Йде вліво - віддзеркалюємо
            visual.flipX = displacement.x < 0;
        }

        // Коли стоїть - один статичний кадр (анімації "стоїть" немає)
        if (moving)
        {
            visual.sprite = set.Loop(SpritePose.Move, clock, unit.battleSprite);
        }
        else
        {
            visual.sprite = unit.battleSprite;
        }
    }
}
