using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Wave Manager", menuName = "ScriptableObjects/Managers/Wave Manager")]
public class WaveManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent spawnWaveEvent = new UnityEvent();
    public Transform goal;

    private void OnEnable()
    {
        goal = null;
        if(spawnWaveEvent == null)
            spawnWaveEvent = new UnityEvent();
    }

    public void SpawnWave()
    {
        spawnWaveEvent.Invoke();
    }
}
