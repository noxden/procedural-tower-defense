using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private int numberOfEnemiesToSpawn;
    [SerializeField] private float secondsBetweenSpawns = 1;

    private void OnEnable()
    {
        waveManager.spawnWaveEvent.AddListener(SpawnWave);
    }

    private void OnDisable()
    {
        waveManager.spawnWaveEvent.RemoveListener(SpawnWave);
    }

    private void SpawnWave()
    {
        StartCoroutine(SpawnWaveEnumerator());
    }

    private IEnumerator SpawnWaveEnumerator()
    {
        for (int i = 0; i < numberOfEnemiesToSpawn; i++)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
            yield return new WaitForSeconds(secondsBetweenSpawns);
        }
    }
}
