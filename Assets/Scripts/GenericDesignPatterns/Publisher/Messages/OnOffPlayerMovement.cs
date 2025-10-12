using UnityEngine;

public class OnOffPlayerMovement : IPublisherMessage
{
    public bool CanMove { get; }
    public OnOffPlayerMovement(bool _canMove)
    {
        CanMove = _canMove;
    }
}
