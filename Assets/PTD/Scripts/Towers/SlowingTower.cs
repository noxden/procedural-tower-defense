//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

public class SlowingTower : TowerObject
{
    [SerializeField] private GameObject slowingSphere;
    [Range(0, 100)]
    [SerializeField] private float slowPercentage = 50f;
    [SerializeField] private float slowDuration = 2f;

    public override void Shoot(EnemyObject targetEnemy)
    {
        base.Shoot(targetEnemy);

        GameObject slowSphere = Instantiate(slowingSphere, transform.position, Quaternion.identity, null);
        slowSphere.transform.localScale = Vector3.one * tower.range;

        Collider[] colliders = Physics.OverlapSphere(transform.position, tower.range, LayerMask.GetMask("Enemy"));
        foreach (Collider collider in colliders)
        {
            EnemyObject enemy = collider.GetComponent<EnemyObject>();
            enemy.Slow(slowPercentage, slowDuration);
        }
    }
}
