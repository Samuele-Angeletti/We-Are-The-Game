using UnityEngine;
[CreateAssetMenu(fileName ="New Game Event", menuName ="NewGameEvent")]
public class GameEvent : ScriptableObject
{
    public EGameEventType GameEventType;
    public EGameEventEffect GameEventEffect;

    public Sprite SpriteGraphics;

    [SerializeField, Range(0, 1f)] float chanceSuccess;

    public float ChanceSuccessPercentage => chanceSuccess * 100;
    public float ChanceFailurePercentage => (1f - chanceSuccess) * 100;

    public bool TryChanceSuccess()
    {
        return Random.Range(0, 1f) <= chanceSuccess;
    }
}
