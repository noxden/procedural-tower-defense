//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

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
