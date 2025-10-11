using UnityEngine;

public class BombDrop : MonoBehaviour
{
    [SerializeField] GameEventsStats stats;
    [SerializeField] float destroyAfterTime = 1f;

    private void Awake()
    {
        Destroy(gameObject, destroyAfterTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            player.ApplyStatsDelta(stats);
        }
    }
}
