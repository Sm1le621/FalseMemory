using UnityEngine;

// Як рухаються частинки: піднімаються вгору, крутяться по колу або розлітаються
public enum BattleVFXShape { Rising, Orbit, Burst }

// Налаштування одного візуального ефекту (вогонь...)
[CreateAssetMenu(menuName = "FalseMemory/Battle VFX Profile")]
public sealed class BattleVFXProfile : ScriptableObject
{
    public BattleVFXShape shape;
    public Color color = Color.white;

    // Картинка однієї частинки
    public Sprite particleSprite;

    // Контролер з .anim кліпом кадрів ефекту
    public RuntimeAnimatorController effectController;
    // Скільки кадрів у кліпі (для CurrentFrame у тестах)
    public int frameCount = 8;

    // Розмір кадру анімації
    [Range(32f, 256f)]
    public float frameSize = 128f;

    // Скільки секунд триває ефект
    [Range(0f, 2f)]
    public float duration = 0.45f;

    // Кількість частинок
    [Range(4, 24)]
    public int particleCount = 12;

    // Розмір частинки
    [Range(1f, 24f)]
    public float particleSize = 8f;

    // Радіус розльоту частинок
    [Range(0f, 100f)]
    public float radius = 42f;
}
