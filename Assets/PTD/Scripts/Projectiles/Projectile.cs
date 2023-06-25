using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float speed = 10f;
    private float damage = 0;
    private Enemy target;

    public void SetTarget(Enemy target)
    {
        this.target = target;
    }

    public void SetDamage(float damage)
    {
        this.damage = damage;
    }

    private void Update()
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

    private void HitTarget()
    {
        if (target != null)
        {
            target.TakeDamage(damage);
        }

        DestroyProjectile();
    }

    private void DestroyProjectile()
    {
        Destroy(gameObject);
    }
}