using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 3f;

    private Transform playerTransform;
    private bool isAggro = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Если игрок в радиусе агра — двигаемся к нему
        if (isAggro && playerTransform != null)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            rb.MovePosition(rb.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    // Срабатывает, когда объект входит в радиус триггера
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            isAggro = true;
        }
    }

    // Срабатывает, когда игрок выходит из радиуса триггера
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAggro = false;
            rb.linearVelocity = Vector2.zero; // Остановка (или rb.velocity для старых версий Unity)
        }
    }
}