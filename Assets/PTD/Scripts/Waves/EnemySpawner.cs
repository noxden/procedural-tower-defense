using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private Transform spawnPoint;
    private EnemyWave currentWave;
    private bool waveSpawningStarted = false;
    private float waveCooldownTimer = 0f;
    private bool currentWaveSpawned = false;
    private bool halfwayThroughCurrentWave = false;
    private int lastSkipGold = -1;

    private void OnEnable()
    {
        waveManager.spawnFirstWaveEvent.AddListener(StartWaveSpawning);
        waveManager.spawnNextWaveEarlyEvent.AddListener(SpawnNextWaveEarly);
    }

    private void OnDisable()
    {
        waveManager.spawnFirstWaveEvent.RemoveListener(StartWaveSpawning);
        waveManager.spawnNextWaveEarlyEvent.RemoveListener(SpawnNextWaveEarly);
    }

    private void StartWaveSpawning()
    {
        GetNextWave();
        waveSpawningStarted = true;
    }

    private void GetNextWave()
    {
        currentWave = waveManager.GetCurrentWave();
        if(currentWave == null)
        {
            return;
        }
        waveCooldownTimer = currentWave.timeUntilNextWave;
        currentWaveSpawned = false;
        halfwayThroughCurrentWave = false;
        waveManager.currentWave++;
    }

    public void SpawnNextWaveEarly()
    {
        waveCooldownTimer = 0;
    }

    private IEnumerator SpawnWaveEnumerator()
    {
        currentWaveSpawned = true;
        List<GameObject> enemyObjectList = currentWave.enemies;
        float secondsBetweenSpawns = currentWave.secondsBetweenSpawns;

        for (int i = 0; i < enemyObjectList.Count; i++)
        {
            GameObject enemy = Instantiate(enemyObjectList[i], spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }

    private void Update()
    {
        if (!waveSpawningStarted)
            return;

        if (!currentWaveSpawned)
        {
            StartCoroutine(SpawnWaveEnumerator());
            currentWaveSpawned = true;
            waveManager.NextWaveSpawned();
        }

        if(waveCooldownTimer <= 0)
        {
            GetNextWave();
        }
        else
        {
            if(waveCooldownTimer <= currentWave.timeUntilNextWave / 2)
            {
                int skipGold = (int)( (currentWave.maxBonusGoldForSkippingWave * waveCooldownTimer) / (currentWave.timeUntilNextWave / 2));

                if(skipGold != lastSkipGold)
                {
                    waveManager.UpdateSkipGold(skipGold);
                    lastSkipGold = skipGold;
                }

                if (!halfwayThroughCurrentWave)
                {
                    waveManager.HalfwayThroughWave();
                    halfwayThroughCurrentWave = true;
                }             
            }
            waveCooldownTimer -= Time.deltaTime;
        }
    }
}
