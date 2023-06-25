using UnityEngine;

public class Tower : MonoBehaviour
{
    [SerializeField] protected float damage;
    [SerializeField] protected float range;
    [SerializeField] protected float attackSpeed;
    private float attackCooldown;

    private enum TargetingPriority
    {
        Closest,
        Furthest,
        First,
        Last
    }
    [SerializeField] private TargetingPriority targetingPriority = TargetingPriority.Closest;

    private void Awake()
    {
        attackCooldown = attackSpeed;
    }

    private void Update()
    {
        if(attackCooldown <= 0)
        {
            Enemy targetEnemy = EnemyToAttack();
            if (targetEnemy != null)
            {
                Shoot(targetEnemy);
            }
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public virtual void Shoot(Enemy targetEnemy)
    {
        attackCooldown = attackSpeed;
    }

    public virtual Enemy GetClosestEnemy()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, LayerMask.GetMask("Enemy"));

        if (colliders.Length == 0)
            return null;

        Enemy closestEnemy = null;
        float closestDistance = range;

        for (int i = 0; i < colliders.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, colliders[i].transform.position);
            if (distance < closestDistance)
            {
                closestEnemy = colliders[i].GetComponent<Enemy>();
                closestDistance = distance;
            }
        }

        return closestEnemy;
    }

    public virtual Enemy EnemyToAttack()
    {
        Enemy prioritisedEnemy;
        switch (targetingPriority)
        {
            case TargetingPriority.Closest:
                prioritisedEnemy = GetClosestEnemy();
                break;
            case TargetingPriority.Furthest:
                prioritisedEnemy = null;
                break;
            case TargetingPriority.First:
                prioritisedEnemy = null;
                break;
            case TargetingPriority.Last:
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
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
