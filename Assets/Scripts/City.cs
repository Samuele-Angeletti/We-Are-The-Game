using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class City : MonoBehaviour
{
    [Header("Config (ScriptableObject)")]
    [SerializeField] private CityConfig config;

    [Header("Runtime state (do not edit at runtime)")]
    [SerializeField]
    private int currentMedicine;
    [SerializeField]
    private float currentTimeBeforeDecay;
    [SerializeField]
    private bool isSaved = false;
    [SerializeField]
    private bool isDestroyed = false;
    [SerializeField]
    private bool isProducing = false;

    [SerializeField] private bool isShowingCurrentCityOptions = false;

    [Header("References")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    public BarsLogic barsLogic;
    [SerializeField] private GameObject showCityOptionsButton;

    [Header("Civils")]
    [SerializeField] NpcCivilController civilControllerPrefab;
    List<NpcCivilController> npcCivilControllers;

    // coroutines
    private Coroutine productionCoroutine;
    private Coroutine decayCoroutine;

    void Awake()
    {
        if (config == null)
            Debug.LogError($"City '{name}' has no CityConfig assigned.");

        if (spriteRenderer == null)
            spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }

    void Start()
    {
        InitializeState();
        StartDecay();
    }

    private void InitializeState()
    {
        currentMedicine = config != null ? config.StartingMedicine : 0;
        currentTimeBeforeDecay = config != null ? config.TimeBeforeDecay : 0f;
        isSaved = false;
        isDestroyed = false;
        isProducing = false;
        showCityOptionsButton.SetActive(false);

        if (spriteRenderer != null && config != null)
            spriteRenderer.color = config.NormalColor;
    }

    #region Save / Destroy / Medicine API

    /// <summary>
    /// Called when the player 'saves' this city by delivering medicine.
    /// Makes it a base and starts production.
    /// </summary>
    public void SaveCity()
    {
        if (isDestroyed || isSaved) return;

        isSaved = true;
        StopDecay(); // ora non decadere più
        if (spriteRenderer != null)
            spriteRenderer.color = config.SavedColor;

        StartMedicineProduction();
    }

    public void DestroyCity()
    {
        if (isDestroyed) return;
        isDestroyed = true;
        isSaved = false;
        StopDecay();
        StopMedicineProduction();

        if (spriteRenderer != null)
            spriteRenderer.color = config.DestroyedColor;
    }

    public void AddMedicine(int amount)
    {
        if (isDestroyed) return;
        currentMedicine = Mathf.Clamp(currentMedicine + amount, 0, config.MedicineCap);
    }

    /// <summary>
    /// Remove medicine, returning the actual removed amount (clamped).
    /// </summary>
    public int TakeMedicine(int amount)
    {
        if (isDestroyed) return 0;
        int removed = Mathf.Min(amount, currentMedicine);
        currentMedicine -= removed;
        return removed;
    }

    public int CurrentMedicine => currentMedicine;
    public bool IsSaved => isSaved;
    public bool IsDestroyed => isDestroyed;

    #endregion

    #region Production

    private void StartMedicineProduction()
    {
        if (isProducing) return;
        productionCoroutine = StartCoroutine(MedicineProductionCoroutine());
        isProducing = true;
    }

    private void StopMedicineProduction()
    {
        if (!isProducing) return;
        if (productionCoroutine != null)
            StopCoroutine(productionCoroutine);

        productionCoroutine = null;
        isProducing = false;
    }

    private IEnumerator MedicineProductionCoroutine()
    {
        // produce immediatamente? qui aspettiamo l'intervallo (opzionale)
        while (true)
        {
            yield return new WaitForSeconds(config.MedicineProductionInterval);
            AddMedicine(config.MedicineProductionRate);
            // TODO: notificare UI/manager (evento) che la città ha cambiato medicine

            barsLogic.UpdateMedicinesBar(currentMedicine, config.MedicineCap);
            //if(isShowingCurrentCityOptions)
        }
    }

    #endregion

    #region Decay

    private void StartDecay()
    {
        // Solo se la città non è già saved/distrutta e il tempo è > 0
        if (isSaved || isDestroyed || config == null || config.TimeBeforeDecay <= 0f) return;
        decayCoroutine = StartCoroutine(DecayCoroutine());
    }

    private void StopDecay()
    {
        if (decayCoroutine != null)
            StopCoroutine(decayCoroutine);
        decayCoroutine = null;
    }

    private IEnumerator DecayCoroutine()
    {
        // decresce il timer ogni secondo
        while (currentTimeBeforeDecay > 0f)
        {
            yield return new WaitForSeconds(1f);
            currentTimeBeforeDecay -= 1f;
            // eventualmente notificare UI/manager con un evento
            // Debug.Log($"City {name} decay: {currentTimeBeforeDecay}/{config.TimeBeforeDecay}");

            barsLogic.UpdateHealthBar(currentTimeBeforeDecay, config.TimeBeforeDecay);
        }

        // tempo scaduto
        DestroyCity();
    }

    #endregion

    #region Player interaction (trigger)

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // cerca il Player (assumo ha componente Player)
        var player = collision.GetComponent<Player>();
        if (player == null) return;

        // Quando il player entra:
        // 1) Se ha medicine -> trasferiscile alla città
        // 2) Marca la città come salvata (se ha ricevuto medicine almeno una volta)
        // 3) Ripristina la stamina del player (diventa un campo base)

        int playerMedicines = player.stats != null ? player.stats.Medicines : 0;
        if (playerMedicines > 0)
        {
            int accepted = Mathf.Min(playerMedicines, config.MedicineCap - currentMedicine);
            // se vuoi trasferire tutto indipendentemente dal cap, usa playerMedicines

            // rimuovi dal player
            player.stats.Medicines -= accepted;
            // aggiungi alla città
            AddMedicine(accepted);
        }

        // se ora la città ha medicine > 0 e non è distrutta -> è considerata salvata
        if (!isDestroyed && currentMedicine > 0)
        {
            SaveCity();
        }

        // la città funge da campo base: ripristina stamina del player
        player.RestoreFullStamina();

        // eventualmente notifica un manager di gioco (es. aumentare produzione globale)


        showCityOptionsButton.SetActive(true);
        UIManager.Instance.currentCity = this;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        showCityOptionsButton.SetActive(false);
        UIManager.Instance.currentCity = null;
    }

    #endregion

    public void StartCivilToDestinationCity(City destination, int medicineToCarry)
    {
        npcCivilControllers ??= new();

        var npc = Instantiate(civilControllerPrefab, transform.position, Quaternion.identity);
        npcCivilControllers.Add(npc);

        npc.Initialize(this, medicineToCarry);
        npc.SetCityDestination(destination);
    }

#if DEBUG
    public void DEBUG_SendToCity(City city)
    {
        StartCivilToDestinationCity(city, 1);
    }
#endif
}
