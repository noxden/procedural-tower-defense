using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObject : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

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
}
