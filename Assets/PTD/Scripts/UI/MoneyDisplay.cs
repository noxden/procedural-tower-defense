using System.Collections;
using System.Collections.Generic;
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
