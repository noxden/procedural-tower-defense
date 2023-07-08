using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/Tower")]
public class Tower : ScriptableObject
{
    [Header("Tower Stats")]
    public float cost;
    public float damage;
    public float range;
    public float secondsPerAttack;
    [HideInInspector] public float attackCooldown;

    public enum TargetingPriority
    {
        Closest,
        Furthest,
        First,
        Last
    }
    public TargetingPriority targetingPriority = TargetingPriority.Closest;

    [Header("Tower Visuals")]
    public Sprite towerIcon;

}
