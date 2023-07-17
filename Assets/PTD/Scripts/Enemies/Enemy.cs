//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class Enemy : ScriptableObject
{
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    public float damage;
    public float goldOnDeath;
    public float speed = 5;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }
}
