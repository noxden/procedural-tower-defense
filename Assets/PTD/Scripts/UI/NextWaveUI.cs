using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NextWaveUI : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private FinanceManagerScriptableObject financeManager;

    [Header("Next Wave Button")]
    [SerializeField] private GameObject nextWaveButton;
    [SerializeField] private TextMeshProUGUI nextWaveText;
    [SerializeField] private string startWavesText = "Start wave ";
    [SerializeField] private string skipWaveText = "Skip to wave ";

    [Header("Bonus Gold UI")]
    [SerializeField] private GameObject bonusGoldUI;
    [SerializeField] private TextMeshProUGUI bonusGoldText;

    private void Start()
    {
        nextWaveText.text = startWavesText + (waveManager.currentWave + 1);
        bonusGoldUI.SetActive(false);
    }

    private void OnEnable()
    {
        waveManager.halfwayThroughWaveEvent.AddListener(ActivateNextWaveButton);
        waveManager.nextWaveSpawnedEvent.AddListener(DeactivateNextWaveButton);

        waveManager.updateSkipGoldEvent.AddListener(UpdateBonusGold);
    }

    private void OnDisable()
    {
        waveManager.halfwayThroughWaveEvent.RemoveListener(ActivateNextWaveButton);
        waveManager.nextWaveSpawnedEvent.RemoveListener(DeactivateNextWaveButton);

        waveManager.updateSkipGoldEvent.RemoveListener(UpdateBonusGold);
    }

    #region Next Wave Button
    private void ActivateNextWaveButton()
    {
        if (waveManager.IsLastWave())
            return;

        nextWaveButton.SetActive(true);
        nextWaveText.text = skipWaveText + (waveManager.currentWave + 1);
    }

    private void DeactivateNextWaveButton()
    {
        //if (waveManager.IsLastWave())
        //    return;
        
        nextWaveButton.SetActive(false);
        bonusGoldUI.SetActive(false);
    }

    public void StartNextWave()
    {
        if(waveManager.currentWave == 0)
        {
            waveManager.SpawnFirstWave();
        }
        else
        {
            waveManager.SpawnNextWaveEarly();
        }

        if(waveManager.skipGold > 0)
        {
            financeManager.ChangeMoney(waveManager.skipGold);
        }
    }
    #endregion

    #region Bonus Gold UI

    private void UpdateBonusGold(int bonusGold)
    {
        if(waveManager.IsLastWave())
        {
            //bonusGoldUI.SetActive(false);
            return;
        }

        bonusGoldUI.SetActive(true);

        bonusGoldText.text = "+" + bonusGold;
    }

    #endregion
}
