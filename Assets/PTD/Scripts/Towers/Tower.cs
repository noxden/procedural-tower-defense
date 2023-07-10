using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Tower", menuName = "ScriptableObjects/Tower")]
public class Tower : ScriptableObject
{
    [Header("Tower Stats")]
    public string towerName;
    public float cost;
    public float damage;
    public float range;
    public float secondsPerAttack;
    public float projectileSpeed;
    public List<PtdEnums.TileType> buildableHeights = new List<PtdEnums.TileType>();
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
