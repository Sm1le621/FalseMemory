using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target; // Ссылка на игрока

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f; // Скорость сглаживания
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f); // Смещение по Z, чтобы камера не «въехала» в сцену

    private void LateUpdate()
    {
        if (target == null) return;

        // Определяем желаемую позицию камеры
        Vector3 desiredPosition = target.position + offset;

        // Плавно перемещаем камеру из текущей позиции в желаемую
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);

        transform.position = smoothedPosition;
    }
}