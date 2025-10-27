using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.PlayerSettings;

/// <summary>
/// spawno un evento in un'area predefinita verso la direzione del player, non spawno se non sono uscito dalla città
/// </summary>
[RequireComponent(typeof(Player))]
public class EventsSpawner : MonoBehaviour
{
    //per chiamate veloci senza messaggi
    public static EventsSpawner Instance;
    Coroutine eventSpawn;

    [Header("Event Spawn Area")]
    [SerializeField] private Vector2 eventSpawnSize = new Vector2(1f, 3f);
    [SerializeField] private float checkDistance = 2f;
    private Player player;
    [SerializeField] private Vector2 direction;

    [Header("Event Vars")]
    private bool spawningEnabled = false;
    [SerializeField] private GameEventTrigger eventTriggerPrefab;
    [SerializeField] private GameEvent[] eventsList;
    [SerializeField] private float minTimeSpawn = 2;
    [SerializeField] private float maxTimeSpawn = 6;
    [SerializeField] private float timeNextSpawn = 3;

    [Header("Debug")]
    [SerializeField] private float curretTime;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        if (eventTriggerPrefab == null)
            Debug.Log("piazza un event trigger nella scena");
        eventTriggerPrefab.gameObject.SetActive(false);
    }
    private void Start()
    {
        player = GetComponent<Player>();
    }
    private void Update()
    {
        if(player.MovementInput != Vector2.zero)
        {
            direction = player.MovementInput;
        }
    }
    public void StartSpawnEvent()
    {
        //inizio a spawnare eventi ogni tot tempo

        //if (spawningEnabled) return;
        spawningEnabled = true;

        if (eventSpawn != null)
        {
            StopCoroutine(eventSpawn);
        }
        eventSpawn = StartCoroutine(SpawnEventInTime());
    }
    public void StopSpawnEvent()
    {
        //smetto di spawnare eventi
        spawningEnabled = false;

        if (eventSpawn != null)
        {
            StopCoroutine(eventSpawn);
            eventSpawn = null;
        }
        eventTriggerPrefab.gameObject.SetActive(false);
    }
    IEnumerator SpawnEventInTime()
    {
        //faccio un rand tra minTimeSpawn e maxTimeSpawn e aspetto quel float di tempo, dopo di che spawno nell'area
        while (spawningEnabled)
        {
            float waitTime = Random.Range(minTimeSpawn, maxTimeSpawn);
            curretTime = waitTime;
            while (curretTime > 0 && spawningEnabled)
            {
                curretTime -= player.MovementInput != Vector2.zero ? Time.deltaTime : 0;
                yield return null;
            }

            if (!spawningEnabled)
                yield break;

            SpawnEvent();
            curretTime = timeNextSpawn;
            while (curretTime > 0 && spawningEnabled)
            {
                curretTime -= player.MovementInput != Vector2.zero ? Time.deltaTime : 0;
                yield return null;
            }
        }

    }
    private void SpawnEvent()
    {
        if (eventsList.Length == 0)
            return;

        Vector2 dir = (direction == Vector2.zero) ? Vector2.right : direction.normalized;
        Vector2 boxCenter = (Vector2)transform.position + dir * checkDistance;

        Vector2 randomLocalPos = new Vector2(
        Random.Range(-eventSpawnSize.x / 2f, eventSpawnSize.x / 2f),
        Random.Range(-eventSpawnSize.y / 2f, eventSpawnSize.y / 2f)
        );
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.Euler(0, 0, angle);

        Vector2 randomWorldPos = boxCenter + (Vector2)(rotation * randomLocalPos);

        //se il punto di spawn non è dentro la navmesh allora ritorno e basta, da fixare
        //if (!NavMesh.SamplePosition(randomWorldPos, out NavMeshHit hit, 0.01f, NavMesh.AllAreas))
        //{
        //    Debug.Log("punto di spawn fuori dal navmesh");
        //    return;
        //}

        eventTriggerPrefab.transform.position = randomWorldPos;
        eventTriggerPrefab.gameObject.SetActive(true);

        GameEvent chosenEvent = eventsList[Random.Range(0, eventsList.Length)];
        eventTriggerPrefab.gameEvent = chosenEvent;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector2 dir = direction.normalized;
        Vector2 boxCenter = (Vector2)transform.position + dir * checkDistance;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boxCenter, Quaternion.Euler(0, 0, angle), Vector3.one);
        Gizmos.matrix = rotationMatrix;
        Gizmos.DrawWireCube(Vector3.zero, eventSpawnSize);
        Gizmos.matrix = Matrix4x4.identity;
    }
}
