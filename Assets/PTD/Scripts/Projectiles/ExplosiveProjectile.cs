//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class ExplosiveProjectile : Projectile
{
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private GameObject explosionObject;
    Vector3 targetPosition;

    public override void SetValues(float speed, float damage, EnemyObject target)
    {
        base.SetValues(speed, damage, target);
        targetPosition = target.transform.position;
    }

    protected override void MoveProjectile()
    {
        Vector3 direction = targetPosition - transform.position;
        float distanceThisFrame = speed * Time.deltaTime;

        if (Vector3.Distance(transform.position, targetPosition) <= distanceThisFrame)
        {
            HitTarget();
            return;
        }

        transform.Translate(direction.normalized * distanceThisFrame, Space.World);
    }

    protected override void HitTarget()
    {
        GameObject explosion = Instantiate(explosionObject, transform.position, Quaternion.identity, null);
        explosion.transform.localScale = Vector3.one * explosionRadius;

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, LayerMask.GetMask("Enemy"));
        foreach (Collider collider in colliders)
        {
            EnemyObject enemy = collider.GetComponent<EnemyObject>();
            enemy.TakeDamage(damage);
        }
        DestroyProjectile();
    }
}