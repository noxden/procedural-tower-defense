using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Wave X", menuName = "ScriptableObjects/Enemy Wave")]
public class EnemyWave : ScriptableObject
{
    public List<GameObject> enemies;
    public float secondsBetweenSpawns;
    public float timeUntilNextWave;
    public float maxBonusGoldForSkippingWave;
}
