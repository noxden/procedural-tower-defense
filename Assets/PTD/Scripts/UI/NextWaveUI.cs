using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextWaveUI : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private GameObject nextWaveButton;

    private void OnEnable()
    {
        waveManager.halfwayThroughWaveEvent.AddListener(ActivateNextWaveButton);
    }

    private void OnDisable()
    {
        waveManager.halfwayThroughWaveEvent.RemoveListener(ActivateNextWaveButton);
    }

    private void ActivateNextWaveButton()
    {
        if(!waveManager.IsLastWave())
            nextWaveButton.SetActive(true);
    }

    public void StartNextWave()
    {
        nextWaveButton.SetActive(false);

        if(waveManager.currentWave == 0)
        {
            waveManager.SpawnFirstWave();
        }
        else
        {
            waveManager.SpawnNextWaveEarly();
        }
    }

}
