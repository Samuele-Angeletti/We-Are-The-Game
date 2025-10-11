using UnityEngine;
using TMPro;

public class CityUI : MonoBehaviour
{
    [Header("Source")]
    public City city;
    [Header("Binding")]
    public TMP_Text medicineCount;
    [Header("Visual")]
    public Color savedColor;
    public Color riskColor;

    void Start()
    {

    }

    void Update()
    {
        medicineCount.gameObject.SetActive(true);

        medicineCount.text = city.CurrentMedicine.ToString();

        if (city.IsSaved)
        {
            medicineCount.color = savedColor;
        }
        else
        {
            medicineCount.color = riskColor;
        }
    }
}
