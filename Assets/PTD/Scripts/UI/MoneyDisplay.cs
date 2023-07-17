//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using TMPro;
using UnityEngine;

public class MoneyDisplay : MonoBehaviour
{
    [SerializeField] private FinanceManagerScriptableObject financeManager;
    [SerializeField] private TextMeshProUGUI moneyText;

    private void OnEnable()
    {
        financeManager.chageMoneyEvent.AddListener(ChangeMoney);
    }

    private void OnDisable()
    {
        financeManager.chageMoneyEvent.RemoveListener(ChangeMoney);
    }

    private void Start()
    {
        moneyText.text = financeManager.GetCurrentMoney().ToString();
    }

    public void ChangeMoney(float amount)
    {
        moneyText.text = financeManager.GetCurrentMoney().ToString();
    }
}
