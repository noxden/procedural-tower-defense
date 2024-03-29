//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Finance Manager", menuName = "ScriptableObjects/Managers/Finance Manager")]
public class FinanceManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent<float> chageMoneyEvent = new UnityEvent<float>();
    [SerializeField] private float startingMoney = 100;
    private float currentMoney;

    private void OnEnable()
    {
        currentMoney = startingMoney;

        if(chageMoneyEvent == null)
            chageMoneyEvent = new UnityEvent<float>();
    }

    public float GetCurrentMoney()
    {
        return currentMoney;
    }

    public void ChangeMoney(float amount)
    {
        currentMoney += amount;

        if(currentMoney < 0)
            currentMoney = 0;

        chageMoneyEvent.Invoke(amount);
    }

    public bool CanAfford(float amount)
    {
        return currentMoney >= amount;
    }
}
