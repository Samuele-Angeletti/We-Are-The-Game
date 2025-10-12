using UnityEngine;

public class AddStatsPlayerMessage : IPublisherMessage
{
    public GameEventsStats PlayerStats { get; }
    public AddStatsPlayerMessage(GameEventsStats _playerStats)
    {
        PlayerStats = _playerStats;
    }
}
