using UnityEngine;

public class City : MonoBehaviour
{
    [Header("Data")]
    public int medicineCost;
    public int medicineCarried;
    public int medicineCap;
    [Header("State")]
    public bool isSaved;
    public bool isDestroyed;
    [Header("Visual")]
    public Color normalColor;
    public Color savedColor;
    public Color destroyedColor;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer.color = normalColor;
    }

    void Update()
    {
        if (isSaved) spriteRenderer.color = savedColor;
        if (isDestroyed) spriteRenderer.color = destroyedColor;
    }
}
