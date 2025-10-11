using System.Collections;
using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] private CityData cityData;

    public float DecayUpdateInterval = 1f;

    [SerializeField] private SpriteRenderer spriteRenderer;

    void Start()
    {
        SetUp();
        StartDecay();
    }

    void Update()
    {
    }

    public void SetUp()
    {
        cityData.CurrentMedicine = cityData.StartingMedicine;
        cityData.CurrentTimeBeforeDecay = cityData.TimeBeforeDecay;
        cityData.IsSaved = false;
        cityData.IsDestroyed = false;

        spriteRenderer.color = cityData.NormalColor;
    }

    public void SaveCity()
    {
        StopDecay();
        cityData.IsSaved = true;
        spriteRenderer.color = cityData.SavedColor;
    }

    public void AddMedicine(int amount)
    {
        cityData.CurrentMedicine += amount;
    }

    public void TakeMedicine(int amount)
    {
        cityData.CurrentMedicine -= amount;
    }

    public void DestroyCity()
    {
        cityData.IsDestroyed = true;
        spriteRenderer.color = cityData.DestroyedColor;
    }

    // should subscribe to an event to know when sender is arrived to other city
    // and when he gets back
    public void SendMedicine(City targetCity, int amount)
    {
        if (!cityData.IsSaved || cityData.IsDestroyed || cityData.IsSendingMedicine) return;

        if (amount > cityData.CurrentMedicine)
        {
            Debug.LogWarning($"City {gameObject.name} does not have enough medicine to send {amount}. Sending: {cityData.CurrentMedicine}");
            amount = cityData.CurrentMedicine;
        }

        TakeMedicine(amount);

        // give medicine to sender and assign target city to it

        cityData.IsSendingMedicine = true;
    }

    public void ReceiveMedicine(int amount)
    {
        if (!cityData.IsSendingMedicine) return;
        AddMedicine(amount);
    }

    public void StartDecay()
    {
        InvokeRepeating(nameof(DecayCity), 0, DecayUpdateInterval);
    }

    private void DecayCity()
    {
        cityData.CurrentTimeBeforeDecay -= (int)DecayUpdateInterval;
        Debug.Log($"City {gameObject.name} time before decay: {cityData.CurrentTimeBeforeDecay} / {cityData.TimeBeforeDecay}");
        // Call UI update here

        if (cityData.CurrentTimeBeforeDecay <= 0)
        {
            StopDecay();
            DestroyCity();
        }
    }

    private void StopDecay()
    {
        CancelInvoke(nameof(DecayCity));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // trigger with sender
    }

}
