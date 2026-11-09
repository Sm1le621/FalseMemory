using System.Collections;
using UnityEngine;
using UnityEngine.UI;

// Програє візуальні ефекти (вогонь) поверх портретів.
// Кадри ефекту лежать у .anim кліпі, частинки рухаються кодом.
[DisallowMultipleComponent]
public sealed class BattleVFXController : MonoBehaviour
{
    public BattleUI ui;
    // Ефект за замовчуванням (якщо в заклинання немає свого)
    public BattleVFXProfile fireProfile;

    // Картинка для покадрової анімації
    [SerializeField] private Image frameImage;
    // Картинки-частинки
    [SerializeField] private Image[] particles = System.Array.Empty<Image>();

    // Animator картинки ефекту (додається у грі)
    private Animator frameAnimator;
    // Поточний ефект (корутина)
    private Coroutine effect;

    // Номер поточного кадру (-1 = анімації немає)
    public int CurrentFrame { get; private set; } = -1;

    // Чи показується зараз кадр анімації
    public bool HasAnimatedFrame
    {
        get { return frameImage != null && frameImage.isActiveAndEnabled && frameImage.sprite != null; }
    }

    // Скільки ефектів програно (для тестів)
    public int EffectsPlayed { get; private set; }
    // Скільки частинок зараз видно
    public int ActiveParticles { get; private set; }

    private void Awake()
    {
        if (frameImage != null)
        {
            frameAnimator = SpriteClipPlayer.GetAnimator(frameImage.gameObject);
        }
    }

    // Запустити ефект на гравці (onPlayer = true) або на ворогу
    public void Play(bool onPlayer, BattleVFXProfile profile, Color fallback)
    {
        if (!isActiveAndEnabled || ui == null)
        {
            return;
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
            return;
        }

        // Зупиняємо попередній ефект
        if (effect != null)
        {
            StopCoroutine(effect);
        }
        Clear();

        // Якщо профіль не передали - беремо стандартний
        if (profile == null)
        {
            profile = fireProfile;
        }

        effect = StartCoroutine(Animate(portrait, profile, fallback));
        EffectsPlayed = EffectsPlayed + 1;
    }

    private IEnumerator Animate(Image portrait, BattleVFXProfile profile, Color fallback)
    {
        // Скільки частинок є в сцені
        int available = 0;
        if (particles != null)
        {
            available = particles.Length;
        }

        // Беремо налаштування з профілю, а якщо його немає - стандартні значення
        int wantedCount = 12;
        float duration = 0.45f;
        float radius = 42f;
        float size = 8f;
        Color color = fallback;
        BattleVFXShape shape = BattleVFXShape.Rising;

        if (profile != null)
        {
            wantedCount = Mathf.Clamp(profile.particleCount, 4, 24);
            duration = Mathf.Clamp(profile.duration, 0, 2);
            radius = Mathf.Clamp(profile.radius, 0, 100);
            size = Mathf.Clamp(profile.particleSize, 1, 24);
            color = profile.color;
            shape = profile.shape;
        }

        // Не більше, ніж є частинок у сцені
        int count = Mathf.Min(wantedCount, available);

        // Чи є покадрова анімація
        bool animated = profile != null && profile.effectController != null && frameImage != null;
        if (animated)
        {
            frameImage.color = Color.white;
            frameImage.rectTransform.sizeDelta = Vector2.one * Mathf.Clamp(profile.frameSize, 32, 256);
            // Поверх усього
            frameImage.transform.SetAsLastSibling();
            frameImage.gameObject.SetActive(true);
        }

        // Центр портрета
        Vector2 center = portrait.rectTransform.anchoredPosition +
            Vector2.Scale(portrait.rectTransform.rect.size, new Vector2(0.5f, 0.5f) - portrait.rectTransform.pivot);

        // Вмикаємо потрібну кількість частинок
        for (int i = 0; i < count; i++)
        {
            if (profile != null)
            {
                particles[i].sprite = profile.particleSprite;
            }
            else
            {
                particles[i].sprite = null;
            }
            particles[i].gameObject.SetActive(true);
        }
        ActiveParticles = count;

        for (float t = 0; t < duration; t = t + Time.unscaledDeltaTime)
        {
            float progress = t / duration;

            // Покадрова анімація з .anim кліпа
            if (animated)
            {
                int frameCount = Mathf.Max(1, profile.frameCount);
                CurrentFrame = Mathf.Min(frameCount - 1, Mathf.FloorToInt(progress * frameCount));
                SpriteClipPlayer.Sample(frameAnimator, profile.effectController, "Effect", progress);
                frameImage.enabled = frameImage.sprite != null;
                frameImage.rectTransform.anchoredPosition = portrait.rectTransform.anchoredPosition +
                    Vector2.Scale(portrait.rectTransform.rect.size, Vector2.one * 0.5f - portrait.rectTransform.pivot);
            }

            // Рух частинок
            for (int i = 0; i < count; i++)
            {
                // Кут частинки на колі (при Orbit ще й крутиться)
                float spin = 0;
                if (shape == BattleVFXShape.Orbit)
                {
                    spin = 2.5f;
                }
                float angle = i * Mathf.PI * 2 / count + progress * spin;

                // При Burst частинки розлітаються від центру
                float spread = 1f;
                if (shape == BattleVFXShape.Burst)
                {
                    spread = progress;
                }
                Vector2 offset = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * radius * spread;

                // При Rising частинки стоять рядами і піднімаються вгору
                if (shape == BattleVFXShape.Rising)
                {
                    offset = new Vector2((i % 5 - 2) * radius / 2.5f, -30f + progress * (75f + i % 3 * 12f));
                }

                Image p = particles[i];
                p.rectTransform.anchoredPosition = center + offset;
                // Частинка росте і зменшується
                p.rectTransform.sizeDelta = Vector2.one * size * (0.5f + 0.5f * Mathf.Sin(progress * Mathf.PI));

                // При Orbit частинки повернуті на 45 градусів (ромбики)
                float rotation = 0f;
                if (shape == BattleVFXShape.Orbit)
                {
                    rotation = 45f;
                }
                p.rectTransform.localRotation = Quaternion.Euler(0, 0, rotation);

                // Плавна поява і зникнення
                Color current = color;
                current.a = Mathf.Sin(progress * Mathf.PI);
                p.color = current;
            }

            yield return null;
        }

        Clear();
        effect = null;
    }

    // Сховати всі частинки і кадр анімації
    private void Clear()
    {
        if (particles != null)
        {
            foreach (Image p in particles)
            {
                if (p != null)
                {
                    p.gameObject.SetActive(false);
                }
            }
        }

        if (frameImage != null)
        {
            SpriteClipPlayer.Stop(frameAnimator);
            frameImage.gameObject.SetActive(false);
        }

        ActiveParticles = 0;
        CurrentFrame = -1;
    }

    private void OnDisable()
    {
        if (effect != null)
        {
            StopCoroutine(effect);
        }
        effect = null;
        Clear();
    }
}
