using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] protected float speed = 10f;
    protected float damage = 0;
    protected EnemyObject target;

    public virtual void SetValues(float speed, float damage, EnemyObject target)
    {
        this.speed = speed;
        this.damage = damage;
        this.target = target;
    }

    protected virtual void Update()
    {
        if (target == null)
        {
            DestroyProjectile();
            return;
        }

        MoveProjectile();
    }

    protected virtual void MoveProjectile()
    {
        Vector3 direction = target.transform.position - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (direction.magnitude <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    protected virtual void HitTarget()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        DestroyProjectile();
    }

    protected void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}