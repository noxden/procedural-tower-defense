using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Wave Manager", menuName = "ScriptableObjects/Managers/Wave Manager")]
public class WaveManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent spawnFirstWaveEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent nextWaveSpawnedEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent halfwayThroughWaveEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent spawnNextWaveEarlyEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent<int> updateSkipGoldEvent = new UnityEvent<int>();

    public List<Transform> navigationPath = new List<Transform>();
    [SerializeField] private List<EnemyWave> enemyWaves = new List<EnemyWave>();
    [HideInInspector] public int currentWave = 0;
    [HideInInspector] public int skipGold = 0;

    private void OnEnable()
    {
        currentWave = 0;
        skipGold = 0;

        if(spawnFirstWaveEvent == null)
            spawnFirstWaveEvent = new UnityEvent();
        if (halfwayThroughWaveEvent == null)
            halfwayThroughWaveEvent = new UnityEvent();
        if (spawnNextWaveEarlyEvent == null)
            spawnNextWaveEarlyEvent = new UnityEvent();
        if (nextWaveSpawnedEvent == null)
            nextWaveSpawnedEvent = new UnityEvent();
        if (updateSkipGoldEvent == null)
            updateSkipGoldEvent = new UnityEvent<int>();
    }

    public void SpawnFirstWave()
    {
        spawnFirstWaveEvent.Invoke();
    }

    public void HalfwayThroughWave()
    {
        halfwayThroughWaveEvent.Invoke();
    }

    public void NextWaveSpawned()
    {
        nextWaveSpawnedEvent.Invoke();
        skipGold = 0;
    }

    public bool IsLastWave()
    {
          return currentWave == enemyWaves.Count;
    }

    public void UpdateSkipGold(int skipGold)
    {
        this.skipGold = skipGold;
        updateSkipGoldEvent.Invoke(skipGold);
    }

    public void SpawnNextWaveEarly()
    {
        spawnNextWaveEarlyEvent.Invoke();
    }

    public EnemyWave GetCurrentWave()
    {
        if (enemyWaves.Count <= currentWave)
            return null;
        return enemyWaves[currentWave];
    }

    public void SetNavigationPath(List<Node> wholePath)
    {
        navigationPath = new List<Transform>();

        for(int i = 0; i < wholePath.Count; i++)
        {
            if (wholePath[i].IsCornerPiece())
            {
                navigationPath.Add(wholePath[i].transform);
            }
        }
    }

    public void SetGoal(Transform goal)
    {
        navigationPath.Add(goal);
    }
}
