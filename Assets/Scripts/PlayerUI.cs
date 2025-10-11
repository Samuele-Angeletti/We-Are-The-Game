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

    [Header("Transition")]
    public Transform staminaTransitionTarget;
    private Vector3 staminaTransitionOriginal;

    [Header("State")]
    public bool staminaVisible;

    [Header("Visual")]
    [Range(0f, 1f)]
    public float staminaLowThreshold = 0.25f;
    public Color staminaNormalColor = Color.green;
    public Color staminaLowColor = Color.red;
    public float staminaTransitionTime = 0.35f;

    void Start()
    {
        // sanity checks
        if (staminaSlider == null) Debug.LogWarning("PlayerUI: staminaSlider non assegnato");
        if (staminaGroup == null) Debug.LogWarning("PlayerUI: staminaGroup non assegnato");
        if (staminaTransitionTarget == null) Debug.LogWarning("PlayerUI: staminaTransitionTarget non assegnato");

        // salva posizione originale locale
        if (staminaSlider != null)
            staminaTransitionOriginal = staminaSlider.transform.localPosition;
        else
            staminaTransitionOriginal = Vector3.zero;

        // inizializza visibilità (se la stamina è piena di default nascondiamo)
        bool startVisible = false;
        if (player != null && player.stats != null)
            startVisible = player.stats.Stamina < player.maxStamina;

        staminaVisible = startVisible;

        if (staminaSlider != null && staminaTransitionTarget != null)
            staminaSlider.gameObject.transform.localPosition = staminaVisible ? staminaTransitionOriginal : staminaTransitionTarget.localPosition;

        if (staminaGroup != null)
            staminaGroup.alpha = staminaVisible ? 1f : 0f;
    }

    void Update()
    {
        if (player == null || player.stats == null) return;

        // Aggiorna testo medicine
        if (medicineCount != null)
            medicineCount.text = player.stats.Medicines.ToString();

        // Calcola valore normalizzato della stamina
        float staminaCap = Mathf.Max(0.0001f, player.maxStamina);
        float staminaValue = Mathf.Clamp01(player.stats.Stamina / staminaCap);

        if (staminaSlider != null)
            staminaSlider.value = staminaValue;

        // Colore fill
        if (staminaFill != null)
        {
            staminaFill.color = (staminaValue > staminaLowThreshold) ? staminaNormalColor : staminaLowColor;
        }

        // Logica di show/hide
        bool shouldBeVisible = player.stats.Stamina < player.maxStamina;
        if (shouldBeVisible != staminaVisible)
        {
            staminaVisible = shouldBeVisible;
            ToggleStaminaUI(staminaVisible);
        }
    }

    private void ToggleStaminaUI(bool show)
    {
        if (staminaSlider == null || staminaGroup == null || staminaTransitionTarget == null) return;

        // muove e sfuma con LeanTween (se presente). Se LeanTween non è disponibile,
        // si può semplicemente settare immediatamente la posizione e alpha.
        if (show)
        {
            LeanTween.moveLocal(staminaSlider.gameObject, staminaTransitionOriginal, staminaTransitionTime).setEaseOutCirc();
            LeanTween.alphaCanvas(staminaGroup, 1f, staminaTransitionTime).setEaseOutCirc();
        }
        else
        {
            LeanTween.moveLocal(staminaSlider.gameObject, staminaTransitionTarget.localPosition, staminaTransitionTime).setEaseInCirc();
            LeanTween.alphaCanvas(staminaGroup, 0f, staminaTransitionTime).setEaseInCirc();
        }
    }

    // Se vuoi modificare l'alpha del fill in modo sicuro:
    private void SetImageTransparency(float alpha)
    {
        if (staminaFill == null) return;
        Color c = staminaFill.color;
        staminaFill.color = ChangeTransparency(c, alpha);
    }

    // ATTENZIONE: ordine RGBA corretto
    private Color ChangeTransparency(Color color, float alpha)
    {
        return new Color(color.r, color.g, color.b, alpha);
    }
}
