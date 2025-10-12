using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Player : MonoBehaviour, ISubscriber
{
    private GameInput inputActions;
    private new Rigidbody2D rb;

    [Header("Data")]
    public GameEventsStats stats;               // contiene Stamina, StaminaCap, Medicines, ecc.
    [Tooltip("Velocit� di movimento in units/s")]
    public float moveSpeed = 4f;
    [Tooltip("Quanta stamina viene consumata al secondo mentre ci si muove")]
    public float staminaDrainRate = 1f;
    public float maxStamina = 100;

    [Header("State")]
    public bool occupied = false;
    public City lastVisitedCity;

    [Header("Input (read-only)")]
    [SerializeField] private Vector2 movementInput = Vector2.zero;

    [Header("Audio")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip engineStartUp;
    [SerializeField] AudioClip engineLoop;

    Vector3 _cityPosition;
    private bool _canMove = true;
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        _cityPosition = transform.position;
        Publisher.Subscribe(this, typeof(AddStatsPlayerMessage));
        Publisher.Subscribe(this, typeof(OnOffPlayerMovement));
    }

    void OnEnable()
    {
        if (inputActions == null)
            inputActions = new GameInput();

        inputActions.Enable();
        inputActions.ActionMap.Movement.performed += OnMovementPerformed;
        inputActions.ActionMap.Movement.canceled += OnMovementCanceled;
    }

    void OnDisable()
    {
        if (inputActions != null)
        {
            inputActions.ActionMap.Movement.performed -= OnMovementPerformed;
            inputActions.ActionMap.Movement.canceled -= OnMovementCanceled;
            inputActions.Disable();
        }
    }

    private void OnMovementPerformed(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
        HandleAudio(movementInput);
    }

    private void OnMovementCanceled(InputAction.CallbackContext ctx)
    {
        movementInput = Vector2.zero;
        HandleAudio(movementInput);
    }

    void Update()
    {
        // Logica non-fisica: consumo stamina quando il giocatore si muove
        if (occupied || !_canMove) return;

        if (movementInput.sqrMagnitude > 0.001f)
        {
            // consuma stamina proporzionalmente al tempo
            if (stats != null)
            {
                stats.Stamina = Mathf.Max(0f, stats.Stamina - staminaDrainRate * Time.deltaTime);
                if (stats.Stamina <= 0)
                {
                    StartCoroutine(ReturnToLastCity());
                }
            }
        }
    }

    private IEnumerator ReturnToLastCity()
    {
        _canMove = false;
        yield return new WaitForSeconds(1);
        _canMove = true;
        transform.position = _cityPosition;
        stats.Stamina = maxStamina;
    }

    void FixedUpdate()
    {
        if (occupied || !_canMove)
        {
            // blocchiamo il movimento fisico se occupato
            // assicurati che occupied venga gestito esternamente
            rb.linearVelocity = Vector2.zero;
            return;
        }

        // calcola la velocity desiderata
        Vector2 desired = movementInput;
        if (desired.sqrMagnitude > 0.001f)
        {
            desired = desired.normalized * moveSpeed;
        }
        else
        {
            desired = Vector2.zero;
        }

        // Imposta la velocit� (uso linearVelocity come richiesto)
        rb.linearVelocity = desired;
    }

    private void HandleAudio(Vector2 movementInput)
    {
        if (occupied || !_canMove)
        {
            if (audioSource.isPlaying) 
                audioSource.Stop();
            return;
        }

        if (movementInput == Vector2.zero)
        {
            audioSource.Stop();
        }
        else
        {
            if (audioSource.isPlaying)
                return;

            if (stats.Stamina == maxStamina)
                audioSource.clip = engineStartUp;
            else
                audioSource.clip = engineLoop;

            audioSource.Play();
        }
    }

    /// <summary>
    /// Applica un delta di statistiche allo stats corrente del player.
    /// Esempio: passare un GameEventsStats con Medicines=+5 incrementer� stats.Medicines.
    /// </summary>
    /// <param name="delta">valori da sommare allo stats del player</param>
    public void ApplyStatsDelta(GameEventsStats delta)
    {
        if (delta == null || stats == null) return;

        stats.Stamina += delta.Stamina;
        stats.Medicines += delta.Medicines;

        // clampare la stamina al cap (se la struttura ha lo staminaCap)
        if (stats.Stamina > maxStamina)
            stats.Stamina = maxStamina;
    }

    /// <summary>
    /// Utility per ricostituire completamente la stamina (es. quando raggiungi una citt�).
    /// </summary>
    public void RestoreFullStamina()
    {
        if (stats == null) return;
        stats.Stamina = maxStamina;
    }

    public void OnPublish(IPublisherMessage message)
    {
        if (message is AddStatsPlayerMessage playerStatsMessage)
        {
            ApplyStatsDelta(playerStatsMessage.PlayerStats);
        }
        //else if (message is OnOffPlayerMovement playerMovement)
        //{
        //    occupied = playerMovement.CanMove;
        //}
    }

    public void OnDisableSubscriber()
    {
        Publisher.Unsubscribe(this, typeof(AddStatsPlayerMessage));
        //Publisher.Unsubscribe(this, typeof(OnOffPlayerMovement));
    }
    private void OnDestroy()
    {
        OnDisableSubscriber();
    }

    public void SetLastCity(City lastCity)
    {
        lastVisitedCity = lastCity;
        _cityPosition = lastCity.transform.position;
    }
}
