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

    [Header("Sprites")]
    [SerializeField] Sprite north;
    [SerializeField] Sprite south;
    [SerializeField] Sprite east;
    [SerializeField] Sprite southEast;
    [SerializeField] Sprite northWest;

    SpriteRenderer _graphics;
    Vector3 _cityPosition;
    private bool _canMove = true;

    // track last shown sprite/flip to avoid redundant sets
    private Sprite _lastSprite = null;
    private bool _lastFlipX = false;

    // threshold below which we consider the player "idle" (no sprite change)
    private const float SPRITE_CHANGE_SPEED_THRESHOLD = 0.1f;

    void Awake()
    {
        _graphics = GetComponentInChildren<SpriteRenderer>();
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

        // --- NUOVO: aggiorna la sprite in base alla direzione della velocit�
        HandleSpriteByDirection();
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
    /// Aggiorna la sprite del camion in base alla direzione di rb.linearVelocity.
    /// Usa flipX per coprire West, NorthEast e SouthWest quando necessario.
    /// Non cambia la sprite se la velocit� � sotto una soglia (idle).
    /// </summary>
    private void HandleSpriteByDirection()
    {
        if (_graphics == null || rb == null) return;

        Vector2 v = rb.linearVelocity;
        if (v.sqrMagnitude < SPRITE_CHANGE_SPEED_THRESHOLD * SPRITE_CHANGE_SPEED_THRESHOLD)
        {
            // se vuoi, qui si potrebbe mettere una sprite "idle".
            // Al momento non cambiamo nulla quando il veicolo � fermo.
            return;
        }

        // calcola angolo in gradi (0 = +x East, aumenti in senso antiorario)
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        // mappa l'angolo ai 8 settori: ciascuno 45°, centri a 0,45,90,...
        // index 0 = East, 1 = NE, 2 = North, 3 = NW, 4 = West, 5 = SW, 6 = South, 7 = SE
        int sector = Mathf.RoundToInt(angle / 45f) % 8;

        Sprite chosen = null;
        bool flipX = false;

        switch (sector)
        {
            case 0: // East
                chosen = east;
                flipX = false;
                break;
            case 1: // NorthEast -> usa northWest + flipX (northWest flipped -> northEast)
                chosen = northWest;
                flipX = true;
                break;
            case 2: // North
                chosen = north;
                flipX = false;
                break;
            case 3: // NorthWest
                chosen = northWest;
                flipX = false;
                break;
            case 4: // West -> usa east + flipX
                chosen = east;
                flipX = true;
                break;
            case 5: // SouthWest -> usa southEast + flipX
                chosen = southEast;
                flipX = true;
                break;
            case 6: // South
                chosen = south;
                flipX = false;
                break;
            case 7: // SouthEast
                chosen = southEast;
                flipX = false;
                break;
            default:
                chosen = east;
                flipX = false;
                break;
        }

        // se la sprite scelta non � assegnata, non facciamo nulla (evitiamo nullref)
        if (chosen == null)
            return;

        // applica solo se � cambiato (minimo lavoro in runtime)
        if (_lastSprite != chosen || _lastFlipX != flipX)
        {
            _graphics.sprite = chosen;
            _graphics.flipX = flipX;
            _lastSprite = chosen;
            _lastFlipX = flipX;
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

    internal int TakeMedicines()
    {
        var med = stats.Medicines;
        stats.Medicines = 0;
        return med;
    }
}
