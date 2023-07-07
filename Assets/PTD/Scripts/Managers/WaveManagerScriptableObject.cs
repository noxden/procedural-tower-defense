using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Wave Manager", menuName = "ScriptableObjects/Managers/Wave Manager")]
public class WaveManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent spawnWaveEvent = new UnityEvent();
    public List<Transform> navigationPath = new List<Transform>();

    private void OnEnable()
    {
        if(spawnWaveEvent == null)
            spawnWaveEvent = new UnityEvent();
    }

    public void SpawnWave()
    {
        spawnWaveEvent.Invoke();
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
