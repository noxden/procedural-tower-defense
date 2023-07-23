//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

public class TowerObject : MonoBehaviour
{
    public Tower tower;
    private bool isPlaced = false;

    private void Awake()
    {
        tower = Instantiate(tower);
        tower.attackCooldown = tower.secondsPerAttack;
    }

    private void Update()
    {
        if (!isPlaced)
            return;

        if(tower.attackCooldown <= 0)
        {
            EnemyObject targetEnemy = EnemyToAttack();
            if (targetEnemy != null)
            {
                Shoot(targetEnemy);
            }
        }
        else
        {
            tower.attackCooldown -= Time.deltaTime;
        }
    }

    public void Place()
    {
        Debug.Log("Tower placed");
        isPlaced = true;
    }

    public virtual void Shoot(EnemyObject targetEnemy)
    {
        tower.attackCooldown = tower.secondsPerAttack;
    }

    public virtual EnemyObject GetClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, tower.range, LayerMask.GetMask("Enemy"));

        if (colliders.Length == 0)
            return null;

        EnemyObject closestEnemy = null;
        float closestDistance = tower.range;

        for (int i = 0; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = colliders[i].GetComponent<EnemyObject>();
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    public virtual EnemyObject EnemyToAttack()
    {
        EnemyObject prioritisedEnemy;
        switch (tower.targetingPriority)
        {
            case Tower.TargetingPriority.Closest:
                prioritisedEnemy = GetClosestEnemy();
                break;
            case Tower.TargetingPriority.Furthest:
                prioritisedEnemy = null;
                break;
            case Tower.TargetingPriority.First:
                prioritisedEnemy = null;
                break;
            case Tower.TargetingPriority.Last:
                prioritisedEnemy = null;
                break;
            default:
                prioritisedEnemy = null;
                break;
        }
        return prioritisedEnemy;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, tower.range);
    }

}
