using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NpcCivilController : MonoBehaviour
{
    [SerializeField] float timeAwaitInCity = 2f;
    [SerializeField] float maxStamina = 5;
    [SerializeField] float staminaLoseSpeed = 1f;
    public GameEventsStats stats;
    NavMeshAgent agent;
    City _ownCity;
    City _destinationCity;
    bool directionIsDestinationCity;
    int medicinesRequested = 0;
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public void Initialize(City ownCity, int medicinesRequested)
    {
        _ownCity = ownCity;
        stats.Medicines = medicinesRequested;
        this.medicinesRequested = medicinesRequested;
        stats.Stamina = maxStamina;
    }

    public void SetStats(GameEventsStats stats)
    {
        this.stats.Medicines = stats.Medicines;
    }

    public void SetCityDestination(City destinationCity)
    {
        _destinationCity = destinationCity;
        directionIsDestinationCity = true;
        agent.SetDestination(_destinationCity.transform.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<City>(out var city))
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

        if (agent.velocity.magnitude > 0)
        {
            stats.Stamina -= Time.deltaTime * staminaLoseSpeed;

            if (stats.Stamina <= 0)
            {
                stats.Stamina = 0;
                directionIsDestinationCity = false;
                transform.position = _ownCity.transform.position;
                StartCoroutine(AwaitAndContinue());
            }
        }
    }

    private IEnumerator AwaitAndContinue()
    {
        agent.isStopped = true;
        yield return new WaitForSeconds(timeAwaitInCity);
        stats.Stamina = maxStamina;
        agent.SetDestination(directionIsDestinationCity ? _destinationCity.transform.position : _ownCity.transform.position);
    }
}
