using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BarsLogic : MonoBehaviour
{
    public Slider healthBar;
    public Slider medicinesBar;
    public TMP_Text healthText;
    public TMP_Text medicineText;
    public void UpdateHealthBar(float currentValue, float max)
    {
        healthText.text = $"{currentValue}/{max}";
        healthBar.value = currentValue / max;
    }
    public void UpdateMedicinesBar(float currentValue, float max)
    {
        medicineText.text = $"{currentValue}/{max}";
        medicinesBar.value = currentValue / max;
    }
}
