using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/Enemy")]
public class Enemy : ScriptableObject
{
    public float maxHealth;
    [HideInInspector] public float currentHealth;
    public float damage;

    private void OnEnable()
    {
        currentHealth = maxHealth;
    }
}
