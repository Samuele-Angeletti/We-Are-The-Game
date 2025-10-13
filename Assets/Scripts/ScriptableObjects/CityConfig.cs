using UnityEngine;

[CreateAssetMenu(fileName = "CityConfig", menuName = "ScriptableObjects/CityConfig")]
public class CityConfig : ScriptableObject
{
    [Header("Settings")]
    [SerializeField, Min(1)] private int minimumMedicineToSurvive = 10;
    public int MinimumMedicinesToSurvive => minimumMedicineToSurvive;

    [SerializeField, Min(0)] private int startingMedicine = 0;
    public int StartingMedicine => startingMedicine;

    [SerializeField, Min(0)] private int medicineCost = 1; // se vuoi un costo per inviare
    public int MedicineCost => medicineCost;

    [SerializeField, Min(0)] private int medicineCarried = 1; // quanti ne porta il player (opzionale)
    public int MedicineCarried => medicineCarried;

    [SerializeField, Min(0)] private int medicineCap = 100;
    public int MedicineCap => medicineCap;

    [Space]
    [SerializeField, Min(0)] private int medicineProductionRate = 1;
    public int MedicineProductionRate => medicineProductionRate;

    [SerializeField, Min(0.01f)] private float medicineProductionInterval = 5f;
    public float MedicineProductionInterval => medicineProductionInterval;

    [SerializeField, Min(1)] private int maxNpcs = 1;
    public int MaxNpcs => maxNpcs;

    [Space]
    [SerializeField, Min(0)] private float timeBeforeDecay = 60f; // in seconds
    public float TimeBeforeDecay => timeBeforeDecay;

    [Header("Visual")]
    [SerializeField] private Color normalColor = Color.white;
    public Color NormalColor => normalColor;

    [SerializeField] private Color savedColor = Color.green;
    public Color SavedColor => savedColor;

    [SerializeField] private Color destroyedColor = Color.red;
    public Color DestroyedColor => destroyedColor;
}
