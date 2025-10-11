using System;
using UnityEngine;

[CreateAssetMenu(fileName = "CityData", menuName = "ScriptableObjects"
    + "/" + "CityData"
)]

public class CityData : ScriptableObject
{
    [Header("Settings")]

    [SerializeField, Min(0)] private int startingMedicine;
    public int StartingMedicine => startingMedicine;

    [SerializeField, Min(0)] private int medicineCost;
    public int MedicineCost => medicineCost;

    [SerializeField, Min(0)] private int medicineCarried;
    public int MedicineCarried => medicineCarried;

    [SerializeField, Min(0)] private int medicineCap;
    public int MedicineCap => medicineCap;


    [Space]
    [SerializeField, Min(0)] private int medicineProductionRate;
    public int MedicineProductionRate => medicineProductionRate;

    [SerializeField, Min(0)] private float medicineProductionInterval;
    public float MedicineProductionInterval => medicineProductionInterval;


    [Space]
    [SerializeField, Min(0), Tooltip("In seconds")] private int timeBeforeDecay;
    public int TimeBeforeDecay => timeBeforeDecay;

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.white;
    public Color NormalColor => normalColor;

    [SerializeField] private Color savedColor = Color.green;
    public Color SavedColor => savedColor;

    [SerializeField] private Color destroyedColor = Color.red;
    public Color DestroyedColor => destroyedColor;


    private int currentMedicine;

    public int CurrentMedicine
    {
        get { return currentMedicine; }
        set { currentMedicine = Mathf.Clamp(value, 0, medicineCap); }
    }

    private float currentTimeBeforeDecay;

    public float CurrentTimeBeforeDecay
    {
        get { return currentTimeBeforeDecay; }
        set { currentTimeBeforeDecay = Mathf.Clamp(value, 0, timeBeforeDecay); }
    }

    private bool isSendingMedicine;
    public bool IsSendingMedicine
    {
        get { return isSendingMedicine; }
        set { isSendingMedicine = value; }
    }

    private bool isSaved;

    public bool IsSaved
    {
        get { return isSaved; }
        set { isSaved = value; }
    }

    private bool isDestroyed;

    public bool IsDestroyed
    {
        get { return isDestroyed; }
        set { isDestroyed = value; }
    }
}
