using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    [SerializeField] GameEvent gameEvent;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = gameEvent.SpriteIcon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            Publisher.Publish(new OpenEventUIMessage(gameEvent));
        }
    }
}
