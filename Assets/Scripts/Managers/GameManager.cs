using DesignPatterns.Generics;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Player _player;
    public Player Player => _player;
    public override void Awake()
    {
        base.Awake();
        _player = FindFirstObjectByType<Player>(FindObjectsInactive.Exclude);
    }
}
