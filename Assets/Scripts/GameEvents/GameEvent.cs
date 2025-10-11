using System;
using UnityEngine;
[CreateAssetMenu(fileName = "New Game Event", menuName = "NewGameEvent")]
public class GameEvent : ScriptableObject
{
    public EGameEventType GameEventType;
    public EGameEventEffect GameEventEffect;
    public bool ShowIcon = true;
    public Sprite SpriteIcon;
    [Header("Only for bomb drop")]
    public GameObject BombPrefab;
    [Header("Success percentage (not for bombs)")]
    [SerializeField, Range(0, 1f)] float chanceSuccess;

    [Header("Win options")]
    public GameEventsStats StatsOnSuccess; // to show UI
    [Header("Lose options")]
    public GameEventsStats StatsOnFailure; // to show UI

    public float ChanceSuccessPercentage => chanceSuccess * 100; // to show UI
    public float ChanceFailurePercentage => (1f - chanceSuccess) * 100; // to show UI

    public GameEventsStats ExecuteEventAndGetStats() // to use on selection
    {
        return UnityEngine.Random.Range(0, 1f) <= chanceSuccess ? StatsOnSuccess : StatsOnFailure;
    }

}

[Serializable]
public class GameEventsStats
{
    public float Stamina;
    public int Medicines;
}