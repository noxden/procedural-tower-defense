//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

public class SingleTargetTower : TowerObject
{
    [SerializeField] private GameObject projectileObject;
    [SerializeField] private Transform projectileSpawnPoint;

    public override void Shoot(EnemyObject targetEnemy)
    {
        base.Shoot(targetEnemy);

        Projectile projectile = Instantiate(projectileObject, projectileSpawnPoint.position, Quaternion.identity, null).GetComponent<Projectile>();
        projectile.SetValues(tower.projectileSpeed, tower.damage, targetEnemy);
    }
}
