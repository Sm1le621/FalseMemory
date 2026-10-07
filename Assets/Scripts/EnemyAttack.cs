using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 10;
    public float attackCooldown = 1f;
    private float nextAttackTime = 0f;

    // Срабатывает ТОЛЬКО при физическом касании тел (CircleCollider2D с Is Trigger = false)
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= nextAttackTime)
            {
                Health playerHealth = collision.gameObject.GetComponent<Health>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    nextAttackTime = Time.time + attackCooldown;
                    Debug.Log($"Враг ударил игрока! Урон: {damage}");
                }
            }
        }
    }
}