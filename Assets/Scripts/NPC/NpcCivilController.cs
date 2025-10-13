using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcCivilController : Character
{
    [SerializeField] float timeAwaitInCity = 2f;
    //[SerializeField] float maxStamina = 5;
    [SerializeField] float staminaLoseSpeed = 1f;
    //public GameEventsStats stats;

    [Header("Sprites (direzione)")]
    [SerializeField] Sprite north;
    [SerializeField] Sprite south;
    [SerializeField] Sprite east;
    [SerializeField] Sprite southEast;
    [SerializeField] Sprite northWest;

    NavMeshAgent agent;
    City _ownCity;
    City _destinationCity;
    bool directionIsDestinationCity;
    int medicinesRequested = 0;

    // Sprite renderer & caching per evitare set non necessari
    SpriteRenderer _graphics;
    private Sprite _lastSprite = null;
    private bool _lastFlipX = false;

    // threshold sotto cui consideriamo "fermo" e non cambiamo sprite
    private const float SPRITE_CHANGE_SPEED_THRESHOLD = 0.1f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        _graphics = GetComponentInChildren<SpriteRenderer>();
    }

    public void Initialize(City ownCity, int medicinesRequested)
    {
        _ownCity = ownCity;
        this.medicinesRequested = medicinesRequested;
        SetStats(new GameEventsStats() { Stamina = maxStamina, Medicines = medicinesRequested });
    }

    public void SetStats(GameEventsStats stats)
    {
        this.stats.Medicines = stats.Medicines;
        this.stats.Stamina = stats.Stamina;
    }

    public void SetCityDestination(City destinationCity)
    {
        _destinationCity = destinationCity;
        directionIsDestinationCity = true;
        agent.SetDestination(_destinationCity.transform.position);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<City>(out var city))
        {
            if (city == _destinationCity && directionIsDestinationCity)
            {
                city.AddMedicine(stats.Medicines);
                directionIsDestinationCity = false;
                StartCoroutine(AwaitAndContinue());
            }
            else if (city == _ownCity && !directionIsDestinationCity)
            {
                stats.Medicines = city.TakeMedicine(medicinesRequested);
                directionIsDestinationCity = true;
                StartCoroutine(AwaitAndContinue());
            }
        }
    }

    private void Update()
    {
        if (stats.Stamina <= 0) return;

        // riduzione stamina se si muove
        if (agent.velocity.magnitude > 0f)
        {
            stats.Stamina -= Time.deltaTime * staminaLoseSpeed;

            if (stats.Stamina <= 0)
            {
                stats.Stamina = 0;
                directionIsDestinationCity = true;
                transform.position = _ownCity.transform.position;
                StartCoroutine(AwaitAndContinue());
            }
        }

        // Aggiorna sprite in base alla direzione dell'agent
        HandleSpriteByDirection();
    }

    private IEnumerator AwaitAndContinue()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(timeAwaitInCity);
        stats.Stamina = maxStamina;
        // se _destinationCity o _ownCity sono null, proteggiamo con null-check
        if (directionIsDestinationCity && _destinationCity != null)
        {
            stats.Medicines = _ownCity.TakeMedicine(medicinesRequested);
            agent.SetDestination(_destinationCity.transform.position);
        }
        else if (!directionIsDestinationCity && _ownCity != null)
        {
            _destinationCity.AddMedicine(stats.Medicines);
            stats.Medicines = 0;
            agent.SetDestination(_ownCity.transform.position);
        }

        agent.isStopped = false;
    }

    /// <summary>
    /// Aggiorna la sprite del NPC in base alla direzione della velocity dell'agent.
    /// Usa flipX per coprire West, NorthEast e SouthWest quando necessario.
    /// Non cambia la sprite se la velocità è sotto una soglia (idle).
    /// </summary>
    private void HandleSpriteByDirection()
    {
        if (_graphics == null || agent == null) return;

        Vector3 v3 = agent.velocity;
        Vector2 v = new Vector2(v3.x, v3.y); // attenzione: NavMeshAgent usa x,z per piano orizzontale
        // Se il tuo NavMeshAgent è 2D e muove su x,y, usa (v3.x,v3.y). Io assumo agente 3D standard.
        // Se all'interno del tuo progetto usi NavMesh in 2D, cambia la riga sopra a: Vector2 v = new Vector2(v3.x, v3.y);

        if (v.sqrMagnitude < SPRITE_CHANGE_SPEED_THRESHOLD * SPRITE_CHANGE_SPEED_THRESHOLD)
        {
            // fermo: non cambiamo sprite (se vuoi impostare un idle specifico, fallo qui)
            return;
        }

        // angolo in gradi: 0 = +x East, aumenta antiorario
        float angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        if (angle < 0f) angle += 360f;

        int sector = Mathf.RoundToInt(angle / 45f) % 8;

        Sprite chosen = null;
        bool flipX = false;

        switch (sector)
        {
            case 0: // East
                chosen = east;
                flipX = false;
                break;
            case 1: // NorthEast -> usa northWest + flipX
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

        if (chosen == null)
            return;

        if (_lastSprite != chosen || _lastFlipX != flipX)
        {
            _graphics.sprite = chosen;
            _graphics.flipX = flipX;
            _lastSprite = chosen;
            _lastFlipX = flipX;
        }
    }
}
