using UnityEngine;

public class GameEventTrigger : MonoBehaviour
{
    [SerializeField] GameEvent gameEvent;
    SpriteRenderer spriteRenderer;
    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        if (gameEvent.ShowIcon)
            spriteRenderer.sprite = gameEvent.SpriteIcon;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            if (gameEvent.GameEventEffect == EGameEventEffect.BombDrop)
            {
                Instantiate(gameEvent.BombPrefab, transform.position, Quaternion.identity);
                gameObject.SetActive(false);
                return;
            }

            Publisher.Publish(new OpenEventUIMessage(gameEvent));
            gameObject.SetActive(false);
        }
    }
}
