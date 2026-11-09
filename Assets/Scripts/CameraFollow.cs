using UnityEngine;

// Камера, яка плавно рухається слідом за гравцем.
public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    // За ким стежить камера (зазвичай це гравець)
    [SerializeField] private Transform target;

    [Header("Settings")]
    // Наскільки швидко камера наздоганяє ціль
    [SerializeField] private float smoothSpeed = 5f;
    // Зсув камери. По Z стоїть -10, щоб камера була "перед" сценою і все бачила
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    // Змінити ціль камери (потрібно після перенесення гравця в іншу сцену)
    public void SetTarget(Transform newTarget)
    {
        target = newTarget;
    }

    // Одразу поставити камеру на ціль, без плавного руху
    public void SnapToTarget()
    {
        if (target == null)
        {
            return;
        }

        transform.position = target.position + offset;
    }

    // LateUpdate викликається після всіх Update,
    // тобто гравець уже встиг рухнутися в цьому кадрі
    private void LateUpdate()
    {
        // Якщо цілі немає, то нічого не робимо
        if (target == null)
        {
            return;
        }

        // Рахуємо, де камера МАЄ бути
        Vector3 desiredPosition = target.position + offset;

        // Плавно рухаємо камеру з поточної позиції до потрібної
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}
