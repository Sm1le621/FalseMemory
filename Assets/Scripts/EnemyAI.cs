using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Настройки движения")]
    public float moveSpeed = 3f;
    [Min(0.01f)] public float acceleration = 48f;
    private float currentSpeed;

    private Transform playerTransform;
    private bool isAggro = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // Если игрок в радиусе агра — двигаемся к нему
        if (rb != null && rb.simulated && isAggro && playerTransform != null)
        {
            Vector2 delta = (Vector2)playerTransform.position - rb.position;
            currentSpeed = Mathf.MoveTowards(currentSpeed, Mathf.Max(0f, moveSpeed),
                Mathf.Max(.01f, acceleration) * Time.fixedDeltaTime);
            rb.MovePosition(rb.position + Vector2.ClampMagnitude(delta, currentSpeed * Time.fixedDeltaTime));
        }
        else currentSpeed = 0f;
    }

    private void OnDisable()
    {
        currentSpeed = 0f;
        if (rb != null) rb.linearVelocity = Vector2.zero;
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
            currentSpeed = 0f;
            if (rb != null) rb.linearVelocity = Vector2.zero;
        }
    }
}
