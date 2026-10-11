using UnityEngine;

[RequireComponent(typeof(BattleUnit))]
public sealed class BattleEncounter : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision) => TryBegin(collision);
    private void OnCollisionStay2D(Collision2D collision) => TryBegin(collision);

    private void TryBegin(Collision2D collision)
    {
        // Detection trigger does not start combat: physical body contact does.
        var player = collision.gameObject.GetComponent<BattleUnit>();
        if (player == null || !collision.gameObject.CompareTag("Player")) return;
        var transition = BattleTransition.Instance;
        if (transition != null) transition.TryBegin(player, GetComponent<BattleUnit>());
    }
}
