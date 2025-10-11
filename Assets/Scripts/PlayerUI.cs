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
    public CanvasGroup staminaGroup;
    public Transform staminaTransitionTarget;
    private Vector2 staminaTransitionOriginal;
    [Header("State")]
    public bool staminaVisible;
    [Header("Visual")]
    [Range(0f, 1f)]
    public float staminaLowThreshold;
    public Color staminaNormalColor;
    public Color staminaLowColor;
    public float staminaTransitionTime;

    void Start()
    {
        staminaTransitionOriginal = staminaSlider.transform.localPosition;

        staminaVisible = false;
        staminaSlider.gameObject.transform.localPosition = staminaTransitionTarget.localPosition;
        staminaGroup.alpha = 0;
    }

    void Update()
    {
        if (player.stamina >= player.staminaCap && staminaVisible == true)
        {
            staminaVisible = false;

            LeanTween.moveLocal(staminaSlider.gameObject, staminaTransitionTarget.localPosition, staminaTransitionTime).setEaseInCirc();
            LeanTween.alphaCanvas(staminaGroup, 0, staminaTransitionTime).setEaseInCirc();
        }
        if (player.stamina < player.staminaCap && staminaVisible == false)
        {
            staminaVisible = true;

            LeanTween.moveLocal(staminaSlider.gameObject, staminaTransitionOriginal, staminaTransitionTime).setEaseOutCirc();
            LeanTween.alphaCanvas(staminaGroup, 1, staminaTransitionTime).setEaseOutCirc();
        }

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

    private void SetImageTransparency(float value)
    {
        staminaFill.color = ChangeTransparency(staminaFill.color, value);
    }

    private Color ChangeTransparency(Color color, float value)
    {
        return new Color(color.r, color.b, color.g, value);
    }
}
