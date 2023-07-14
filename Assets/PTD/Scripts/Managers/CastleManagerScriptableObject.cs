//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Castle Manager", menuName = "ScriptableObjects/Managers/Castle Manager")]
public class CastleManagerScriptableObject : ScriptableObject
{
    [SerializeField] private GameEventManagerScriptableObject gameEventManager;
    [System.NonSerialized] public UnityEvent healthChangedEvent = new UnityEvent();
    [SerializeField] private float maxHealth = 100;
    public float CurrentHealth { get { return currentHealth; } }
    private float currentHealth;

    private void OnEnable()
    {
        currentHealth = maxHealth;
        if(healthChangedEvent == null)
            healthChangedEvent = new UnityEvent();
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0)
        {
            currentHealth = 0;
            gameEventManager.LoseGame();
        }
        healthChangedEvent.Invoke();
    }

}
