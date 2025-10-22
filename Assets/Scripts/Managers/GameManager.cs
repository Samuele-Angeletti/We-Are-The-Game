using DesignPatterns.Generics;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private Player _player;
    public Player Player => _player;

    [Header("Cities Vars")]
    [SerializeField] int totalCities = 10;
    [SerializeField] int currentCitiesSaved = 0;
    [SerializeField] int currentCitiesDestroyed = 0;
    [SerializeField] int citiesSavedWinCondition = 7;

    public override void Awake()
    {
        base.Awake();
        _player = FindFirstObjectByType<Player>(FindObjectsInactive.Exclude);
    }
    //chiamato dalla città quando salvata
    public void CitySaved()
    {
        currentCitiesSaved++;
        CheckWin();
    }
    //chiamato dalla città quando distrutta
    public void CityDestroyed()
    {
        currentCitiesDestroyed++;
        CheckWin();
    }
    public void CheckWin()
    {
        if(totalCities - currentCitiesDestroyed >= citiesSavedWinCondition)
        {
            if(currentCitiesSaved == citiesSavedWinCondition)
            {
                Debug.Log("hai salvato tutte le città, fine");
            }
            else
                Debug.Log($"puoi salvare ancora {totalCities - (currentCitiesDestroyed + currentCitiesSaved)} città da salvare");
        }
        else
        {
            Debug.Log("sono state distrutte troppe città, non le hai salvate in tempo");
        }
    }
}
