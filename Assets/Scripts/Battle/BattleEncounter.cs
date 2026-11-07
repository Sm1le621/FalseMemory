using UnityEngine;

// Вішається на ворога. Коли гравець фізично торкається ворога - починається бій.
[RequireComponent(typeof(BattleUnit))]
public sealed class BattleEncounter : MonoBehaviour
{
    // Перший момент дотику
    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryBegin(collision);
    }

    // Поки тіла торкаються (кожен кадр фізики)
    private void OnCollisionStay2D(Collision2D collision)
    {
        TryBegin(collision);
    }

    private void TryBegin(Collision2D collision)
    {
        // Бій починається тільки від дотику тіл, а не від тригера "помітив гравця"
        BattleUnit player = collision.gameObject.GetComponent<BattleUnit>();
        if (player == null)
        {
            return;
        }
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        BattleTransition transition = BattleTransition.Instance;
        if (transition != null)
        {
            // TryBegin сам перевірить, чи можна зараз почати бій
            transition.TryBegin(player, GetComponent<BattleUnit>());
        }
    }
}
