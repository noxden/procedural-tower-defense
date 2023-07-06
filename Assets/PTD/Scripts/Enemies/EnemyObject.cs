using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class EnemyObject : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private CastleManagerScriptableObject castleManager;
    [SerializeField] private LayerMask castleLayer;
    [SerializeField] private Enemy enemy;
    private NavMeshAgent agent;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        if(waveManager.goal != null)
            agent.SetDestination(waveManager.goal.position);
    }

    public void TakeDamage(float damage)
    {
        enemy.currentHealth -= damage;

        if(enemy.currentHealth <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
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
