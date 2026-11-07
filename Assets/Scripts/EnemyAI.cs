using UnityEngine;

// Простий ворог у світі: коли гравець заходить у зону тригера,
// ворог починає йти до нього.
public class EnemyAI : MonoBehaviour
{
    [Header("Настройки движения")]
    // Швидкість ворога
    public float moveSpeed = 3f;

    // Посилання на гравця (з'являється, коли гравець зайде в тригер)
    private Transform playerTransform;
    // true - ворог "помітив" гравця і йде до нього
    private bool isAggro = false;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // Якщо гравець у радіусі агро - рухаємось до нього
        if (isAggro == true && playerTransform != null)
        {
            // Напрямок від ворога до гравця (довжина 1)
            Vector2 direction = (playerTransform.position - transform.position).normalized;

            Vector2 newPosition = rb.position + direction * moveSpeed * Time.deltaTime;
            rb.MovePosition(newPosition);
        }
    }

    // Викликається, коли хтось заходить у тригер ворога
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerTransform = collision.transform;
            isAggro = true;
        }
    }

    // Викликається, коли хтось виходить з тригера ворога
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isAggro = false;
            // Зупиняємо ворога (у старих версіях Unity це було rb.velocity)
            rb.linearVelocity = Vector2.zero;
        }
    }
}
