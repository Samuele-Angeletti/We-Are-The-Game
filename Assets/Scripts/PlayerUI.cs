using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    [Header("Source")]
    public Player player;
    [Header("Binding")]
    public TMP_Text medicineCount;
    public Slider staminaSlider;
    public Image staminaFill;
    [Header("Visual")]
    [Range(0f, 1f)]
    public float staminaLowThreshold;
    public Color staminaNormalColor;
    public Color staminaLowColor;

    void Start()
    {
        
    }

    void Update()
    {
        medicineCount.text = player.medicine.ToString();

        var staminaValue = player.stamina / player.staminaCap;
        staminaSlider.value = staminaValue;
        if (staminaValue > staminaLowThreshold)
        {
            staminaFill.color = staminaNormalColor;
        }
        else
        {
            staminaFill.color = staminaLowColor;
        }
    }
}
