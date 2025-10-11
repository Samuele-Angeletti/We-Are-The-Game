public class OpenEventUIMessage : IPublisherMessage
{
    public GameEvent GameEvent { get; }
    public OpenEventUIMessage(GameEvent gameEvent)
    {
        GameEvent = gameEvent;
    }
}