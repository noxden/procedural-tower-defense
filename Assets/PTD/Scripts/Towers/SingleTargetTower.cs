using UnityEngine;

public class SingleTargetTower : TowerObject
{
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private Transform projectileSpawnPoint;

    public override void Shoot(EnemyObject targetEnemy)
    {
        base.Shoot(targetEnemy);
        Projectile projectile = Instantiate(projectileObject, projectileSpawnPoint.position, Quaternion.identity, null).GetComponent<Projectile>();
        projectile.SetDamage(tower.damage);
        projectile.SetTarget(targetEnemy);
    }
}
