using UnityEngine;

// Стара атака "при дотику": ворог б'є гравця, коли вони торкаються.
// Зараз для ворогів з покроковим боєм вона вимкнена (див. перевірку нижче).
public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    // Скільки шкоди за один удар
    public int damage = 10;
    // Пауза між ударами в секундах
    public float attackCooldown = 1f;
    // Час, коли ворог зможе вдарити знову
    private float nextAttackTime = 0f;

    // Викликається кожен кадр, поки тіла фізично торкаються
    // (CircleCollider2D з вимкненим Is Trigger)
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Якщо на ворогу є BattleEncounter, то замість цієї атаки починається покроковий бій.
        // Unity може викликати цей метод навіть на вимкненому компоненті,
        // тому перевіряємо явно.
        if (GetComponent<BattleEncounter>() != null)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            // Чи пройшов уже час перезарядки
            if (Time.time >= nextAttackTime)
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    nextAttackTime = Time.time + attackCooldown;
                    Debug.Log("Enemy hit the player for " + damage + " damage.");
                }
            }
        }
    }
}
