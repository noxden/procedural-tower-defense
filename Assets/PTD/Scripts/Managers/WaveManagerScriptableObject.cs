using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Wave Manager", menuName = "ScriptableObjects/Managers/Wave Manager")]
public class WaveManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent spawnFirstWaveEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent halfwayThroughWaveEvent = new UnityEvent();
    [System.NonSerialized] public UnityEvent spawnNextWaveEarlyEvent = new UnityEvent();

    public List<Transform> navigationPath = new List<Transform>();
    [SerializeField] private List<EnemyWave> enemyWaves = new List<EnemyWave>();
    [HideInInspector] public int currentWave = 0;

    private void OnEnable()
    {
        currentWave = 0;
        if(spawnFirstWaveEvent == null)
            spawnFirstWaveEvent = new UnityEvent();
        if (halfwayThroughWaveEvent == null)
            halfwayThroughWaveEvent = new UnityEvent();
        if (spawnNextWaveEarlyEvent == null)
            spawnNextWaveEarlyEvent = new UnityEvent();
    }

    public void SpawnFirstWave()
    {
        spawnFirstWaveEvent.Invoke();
    }

    public void HalfwayThroughWave()
    {
        halfwayThroughWaveEvent.Invoke();
    }

    public bool IsLastWave()
    {
          return currentWave == enemyWaves.Count - 1;
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
