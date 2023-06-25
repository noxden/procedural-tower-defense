using UnityEngine;

public class SingleTargetTower : Tower
{
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private Transform projectileSpawnPoint;

    public override void Shoot(Enemy targetEnemy)
    {
        base.Shoot(targetEnemy);
        Projectile projectile = Instantiate(projectileObject, projectileSpawnPoint.position, Quaternion.identity, null).GetComponent<Projectile>();
        projectile.SetDamage(damage);
        projectile.SetTarget(targetEnemy);
    }
}
