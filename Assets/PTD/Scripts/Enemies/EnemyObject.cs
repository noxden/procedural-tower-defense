//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyObject : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private CastleManagerScriptableObject castleManager;
    [SerializeField] private FinanceManagerScriptableObject financeManager;
    [SerializeField] private LayerMask castleLayer;
    [SerializeField] private Enemy enemy;
    private float originalSpeed;
    private float AGENT_DISTANCE_TO_WAYPOINT = 0.1f;
    private Vector3 WAYPOINT_HEIGHT_DIFFERENCE = new Vector3(0, 4, 0);
    private NavMeshAgent agent;
    private int currentWaypoint = 0;

    private void Awake()
    {
        enemy = Instantiate(enemy);
    }

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = enemy.speed;
        originalSpeed = enemy.speed;

        if (waveManager.navigationPath != null)
            SetNextDestination();
    }

    private void SetNextDestination()
    {
        if(currentWaypoint == waveManager.navigationPath.Count - 1)
        {
            Debug.LogWarning("this is the last waypoint");
            return;
        }
        currentWaypoint++;
        agent.SetDestination(NextWaypointPosition());
    }

    private Vector3 NextWaypointPosition()
    {
        if(currentWaypoint == waveManager.navigationPath.Count - 1)
        {
            return waveManager.navigationPath[currentWaypoint].position;
        }
        else
        {
            return waveManager.navigationPath[currentWaypoint].position + WAYPOINT_HEIGHT_DIFFERENCE;
        }
    }

    private void Update()
    {
        if(agent.remainingDistance <= AGENT_DISTANCE_TO_WAYPOINT)
        {
            SetNextDestination();
        }
    }

    private void OnDrawGizmos()
    {
        if (waveManager.navigationPath == null)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(NextWaypointPosition(), 0.5f);
    }

    public void TakeDamage(float damage)
    {
        enemy.currentHealth -= damage;

        if(enemy.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Slow(float percentAmount, float seconds)
    {
        agent.speed = (100 - percentAmount) / 100 * originalSpeed;
        StopCoroutine(Defrost(seconds));
        StartCoroutine(Defrost(seconds));
    }

    private IEnumerator Defrost(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        agent.speed = originalSpeed;
    }

    public void Die()
    {
        financeManager.ChangeMoney(enemy.goldOnDeath);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if((castleLayer.value & (1 << other.gameObject.layer)) > 0)
        {
            castleManager.TakeDamage(enemy.damage);
            Destroy(gameObject);
        }
    }
}
