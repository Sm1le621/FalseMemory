using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Анімації портретів у бою: замах, удар, відскок, смерть, втеча.
// Тут тільки картинка - HP, мана і предмети тут НЕ змінюються.
[DisallowMultipleComponent]
public sealed class BattlePresentation : MonoBehaviour
{
    public BattleUI ui;
    // Значок щита біля гравця і ворога (коли захищаються)
    public Image playerGuard;
    public Image enemyGuard;

    // Тривалість анімації атаки
    [Range(0f, 1f)]
    public float attackDuration = 0.4f;
    // Тривалість реакції на удар
    [Range(0f, 1f)]
    public float hitDuration = 0.18f;
    // На скільки пікселів портрет "стрибає" вперед при атаці
    [Min(0f)]
    public float lungeDistance = 42f;
    // На скільки відлітає ціль від удару
    [Min(0f)]
    public float reactionDistance = 8f;
    // Тривалість анімації кінця бою
    [Range(0f, 2f)]
    public float outcomeDuration = 0.35f;
    // Пауза, щоб прочитати повідомлення
    [Range(0f, 2f)]
    public float messageDuration = 0.75f;
    // Скільки ворог "думає"
    [Range(0f, 2f)]
    public float enemyThinkDuration = 0.5f;
    // Чи можна прискорити анімації пробілом
    public bool allowFastForward = true;
    // У скільки разів прискорюємо
    [Range(1f, 6f)]
    public float fastForwardMultiplier = 3f;

    // Початкові позиції і розміри портретів
    private Vector2 playerHome;
    private Vector2 enemyHome;
    private Vector3 playerScale;
    private Vector3 enemyScale;

    private BattleUnit player;
    private BattleUnit enemy;
    // true - вже запам'ятали позиції портретів
    private bool bound;
    // Хто зараз атакує (щоб показувати кадри атаки)
    private bool playerAttacking;
    private bool enemyAttacking;

    // Скільки разів програвався ефект влучання (для тестів)
    public int HitEffectsPlayed { get; private set; }
    // Чи йде зараз анімація
    public bool IsAnimating { get; private set; }

    // Час кадру для анімацій. Якщо тримати пробіл - анімації швидші
    public float VisualDelta
    {
        get
        {
            float multiplier = 1f;
            if (allowFastForward && Keyboard.current != null && Keyboard.current.spaceKey.isPressed)
            {
                multiplier = Mathf.Clamp(fastForwardMultiplier, 1f, 6f);
            }
            return Time.unscaledDeltaTime * multiplier;
        }
    }

    // Пауза для читання повідомлення (або "ворог думає")
    public IEnumerator ReadPause(bool enemyThinking, float pace)
    {
        float baseDuration;
        if (enemyThinking)
        {
            baseDuration = enemyThinkDuration;
        }
        else
        {
            baseDuration = messageDuration;
        }

        float duration = Mathf.Clamp(baseDuration, 0f, 2f) * Pace(pace);

        for (float t = 0; t < duration; t = t + VisualDelta)
        {
            yield return null;
        }
    }

    // Прив'язати до учасників і запам'ятати, де стоять портрети
    public void Bind(BattleUnit hero, BattleUnit foe)
    {
        player = hero;
        enemy = foe;

        if (ui == null || ui.playerPortrait == null || ui.enemyPortrait == null)
        {
            return;
        }

        playerHome = ui.playerPortrait.rectTransform.anchoredPosition;
        enemyHome = ui.enemyPortrait.rectTransform.anchoredPosition;
        playerScale = ui.playerPortrait.rectTransform.localScale;
        enemyScale = ui.enemyPortrait.rectTransform.localScale;
        bound = true;
    }

    private void LateUpdate()
    {
        // Коли анімації немає - показуємо звичайні статичні кадри
        if (bound && !IsAnimating)
        {
            StaticFrame(true);
            StaticFrame(false);
        }

        // Значки щита видно тільки коли персонаж живий і захищається
        if (playerGuard != null)
        {
            bool showPlayerGuard = player != null && player.IsAlive && player.IsDefending;
            playerGuard.gameObject.SetActive(showPlayerGuard);
        }
        if (enemyGuard != null)
        {
            bool showEnemyGuard = enemy != null && enemy.IsAlive && enemy.IsDefending;
            enemyGuard.gameObject.SetActive(showEnemyGuard);
        }
    }

    // Повертає учасника (гравця або ворога)
    private BattleUnit GetUnit(bool onPlayer)
    {
        if (onPlayer)
        {
            return player;
        }
        return enemy;
    }

    // Повертає картинку-портрет (гравця або ворога)
    private Image GetPortrait(bool onPlayer)
    {
        if (ui == null)
        {
            return null;
        }
        if (onPlayer)
        {
            return ui.playerPortrait;
        }
        return ui.enemyPortrait;
    }

    // Коли персонаж стоїть - один статичний кадр (анімації "стоїть" немає)
    private void StaticFrame(bool onPlayer)
    {
        BattleUnit unit = GetUnit(onPlayer);
        Image portrait = GetPortrait(onPlayer);

        if (unit != null && unit.IsAlive && portrait != null)
        {
            portrait.sprite = unit.battleSprite;
        }
    }

    // Поставити кадр анімації pose з прогресом progress (від 0 до 1)
    private void PoseFrame(bool onPlayer, SpritePose pose, float progress)
    {
        BattleUnit unit = GetUnit(onPlayer);
        Image portrait = GetPortrait(onPlayer);

        if (unit != null && portrait != null && unit.visualAnimations != null)
        {
            portrait.sprite = unit.visualAnimations.At(pose, progress, unit.battleSprite);
        }
    }

    // Перша половина дії: замах / підготовка
    public IEnumerator Windup(bool onPlayer, BattleAction action, float pace)
    {
        if (!isActiveAndEnabled || !bound || ui == null)
        {
            yield break;
        }

        Image portrait;
        if (onPlayer)
        {
            portrait = ui.playerPortrait;
        }
        else
        {
            portrait = ui.enemyPortrait;
        }
        if (portrait == null)
        {
            yield break;
        }

        IsAnimating = true;

        if (onPlayer)
        {
            playerAttacking = action == BattleAction.Attack;
        }
        else
        {
            enemyAttacking = action == BattleAction.Attack;
        }

        float duration = Mathf.Clamp(attackDuration, 0f, 1f) * 0.5f * Pace(pace);

        Vector2 home;
        float direction;
        if (onPlayer)
        {
            home = playerHome;
            // Гравець стрибає вліво (до ворога)
            direction = -1f;
        }
        else
        {
            home = enemyHome;
            direction = 1f;
        }

        // При атаці портрет рухається вперед, при інших діях - стоїть
        float distance = 0f;
        if (action == BattleAction.Attack)
        {
            distance = Mathf.Clamp(lungeDistance, 0, 100);
        }
        Vector2 destination = home + Vector2.right * direction * distance;

        for (float t = 0; t < duration; t = t + VisualDelta)
        {
            // Портрет могли видалити
            if (portrait == null)
            {
                yield break;
            }

            if (action == BattleAction.Attack)
            {
                PoseFrame(onPlayer, SpritePose.Attack, t / duration);
            }

            portrait.rectTransform.anchoredPosition = Vector2.Lerp(home, destination, Mathf.SmoothStep(0, 1, t / duration));

            // Захист: портрет трохи "присідає"
            if (action == BattleAction.Defend)
            {
                Vector3 baseScale;
                if (onPlayer)
                {
                    baseScale = playerScale;
                }
                else
                {
                    baseScale = enemyScale;
                }
                float squash = 1f - 0.08f * Mathf.Sin(t / duration * Mathf.PI);
                portrait.rectTransform.localScale = Vector3.Scale(baseScale, new Vector3(1f, squash, 1f));
            }

            // Магія: портрет світиться фіолетовим
            if (action == BattleAction.Magic)
            {
                Color baseColor;
                if (onPlayer)
                {
                    baseColor = player.spriteColor;
                }
                else
                {
                    baseColor = enemy.spriteColor;
                }
                float glow = Mathf.Sin(t / duration * Mathf.PI) * 0.7f;
                portrait.color = Color.Lerp(baseColor, new Color(0.78f, 0.7f, 1f), glow);
            }

            yield return null;
        }

        if (action == BattleAction.Attack)
        {
            PoseFrame(onPlayer, SpritePose.Attack, 1f);
        }
        if (portrait != null)
        {
            portrait.rectTransform.anchoredPosition = destination;
        }
    }

    // Друга половина дії: атакуючий повертається, ціль реагує на удар
    public IEnumerator Recover(bool attackerIsPlayer, bool successfulHit, bool critical, float pace)
    {
        if (!isActiveAndEnabled || !bound || ui == null)
        {
            ResetPoses();
            yield break;
        }

        Image attacker;
        Image target;
        if (attackerIsPlayer)
        {
            attacker = ui.playerPortrait;
            target = ui.enemyPortrait;
        }
        else
        {
            attacker = ui.enemyPortrait;
            target = ui.playerPortrait;
        }

        if (attacker == null || target == null)
        {
            ResetPoses();
            yield break;
        }

        Vector2 attackerStart = attacker.rectTransform.anchoredPosition;
        Vector2 attackerHome;
        Vector2 targetHome;
        Color targetColor;
        if (attackerIsPlayer)
        {
            attackerHome = playerHome;
            targetHome = enemyHome;
            targetColor = enemy.spriteColor;
        }
        else
        {
            attackerHome = enemyHome;
            targetHome = playerHome;
            targetColor = player.spriteColor;
        }

        if (successfulHit)
        {
            HitEffectsPlayed = HitEffectsPlayed + 1;
        }

        float duration = Mathf.Max(Mathf.Clamp(attackDuration, 0, 1) * 0.5f, Mathf.Clamp(hitDuration, 0, 1)) * Pace(pace);

        for (float t = 0; t < duration; t = t + VisualDelta)
        {
            if (attacker == null || target == null)
            {
                yield break;
            }

            float progress = t / duration;

            // Чи атакував цей персонаж (тоді прокручуємо кадри атаки назад)
            bool wasAttacking;
            if (attackerIsPlayer)
            {
                wasAttacking = playerAttacking;
            }
            else
            {
                wasAttacking = enemyAttacking;
            }
            if (wasAttacking)
            {
                PoseFrame(attackerIsPlayer, SpritePose.Attack, 1f - progress);
            }

            // Атакуючий повертається додому
            attacker.rectTransform.anchoredPosition = Vector2.Lerp(attackerStart, attackerHome, progress);

            if (successfulHit)
            {
                // Ціль показує біль
                PoseFrame(!attackerIsPlayer, SpritePose.Hurt, progress);

                // Відскок від удару (при криті сильніший)
                float criticalBoost = 1f;
                if (critical)
                {
                    criticalBoost = 1.7f;
                }
                float recoil = Mathf.Sin(progress * Mathf.PI) * Mathf.Clamp(reactionDistance, 0, 30) * criticalBoost;

                float side = 1f;
                if (attackerIsPlayer)
                {
                    side = -1f;
                }
                target.rectTransform.anchoredPosition = targetHome + Vector2.right * side * recoil;

                // Перші 55% часу ціль "блимає" білим
                if (progress < 0.55f)
                {
                    target.color = Color.Lerp(targetColor, Color.white, 0.8f);
                }
                else
                {
                    target.color = targetColor;
                }
            }

            yield return null;
        }

        ResetPoses();
    }

    // Всі анімації підлаштовуються під темп бою (actionDelay).
    // 0.8 - стандартний темп, тоді множник = 1
    private static float Pace(float delay)
    {
        return Mathf.Clamp(delay / 0.8f, 0.05f, 3f);
    }

    // Анімація кінця бою: переможений зникає, або гравець тікає
    public IEnumerator Finish(BattleState outcome, float pace)
    {
        if (!isActiveAndEnabled || !bound || ui == null)
        {
            yield break;
        }

        // При перемозі зникає ворог, інакше - гравець
        Image portrait;
        if (outcome == BattleState.Victory)
        {
            portrait = ui.enemyPortrait;
        }
        else
        {
            portrait = ui.playerPortrait;
        }
        if (portrait == null)
        {
            yield break;
        }

        IsAnimating = true;

        Color color = portrait.color;
        float initialAlpha = color.a;
        Vector3 scale = portrait.rectTransform.localScale;
        Vector2 home = portrait.rectTransform.anchoredPosition;

        // Чий кадр показувати: при перемозі - ворога (onPlayer = false)
        bool onPlayer = outcome != BattleState.Victory;
        // При втечі - кадри ходьби, інакше - кадри смерті
        SpritePose pose;
        if (outcome == BattleState.Escaped)
        {
            pose = SpritePose.Move;
        }
        else
        {
            pose = SpritePose.Death;
        }

        float duration = Mathf.Clamp(outcomeDuration, 0, 1) * Pace(pace);

        for (float t = 0; t < duration; t = t + VisualDelta)
        {
            float progress = t / duration;

            // Поступово робимо прозорим
            color.a = initialAlpha * (1f - progress);
            portrait.color = color;

            PoseFrame(onPlayer, pose, progress);

            if (outcome == BattleState.Escaped)
            {
                // Тікає вправо
                portrait.rectTransform.anchoredPosition = home + Vector2.right * progress * 40f;
            }
            else
            {
                // "Осідає" вниз
                portrait.rectTransform.localScale = Vector3.Scale(scale, new Vector3(1f, 1f - progress * 0.15f, 1f));
            }

            yield return null;
        }

        color.a = 0f;
        portrait.color = color;
        PoseFrame(onPlayer, pose, 1f);
        IsAnimating = false;
    }

    // Повернути портрети в початковий стан
    public void ResetPoses()
    {
        IsAnimating = false;
        playerAttacking = false;
        enemyAttacking = false;
        StaticFrame(true);
        StaticFrame(false);

        if (!bound || ui == null)
        {
            return;
        }

        if (ui.playerPortrait != null)
        {
            ui.playerPortrait.rectTransform.anchoredPosition = playerHome;
            ui.playerPortrait.rectTransform.localScale = playerScale;
            if (player != null && player.IsAlive)
            {
                ui.playerPortrait.color = player.spriteColor;
            }
            else
            {
                // Мертвий - сірий і напівпрозорий
                ui.playerPortrait.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            }
        }

        if (ui.enemyPortrait != null)
        {
            ui.enemyPortrait.rectTransform.anchoredPosition = enemyHome;
            ui.enemyPortrait.rectTransform.localScale = enemyScale;
            if (enemy != null && enemy.IsAlive)
            {
                ui.enemyPortrait.color = enemy.spriteColor;
            }
            else
            {
                ui.enemyPortrait.color = new Color(0.3f, 0.3f, 0.3f, 0.3f);
            }
        }
    }

    private void OnDisable()
    {
        ResetPoses();
    }
}
