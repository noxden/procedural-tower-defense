using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCastle : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;
    [SerializeField] private Transform goalPoint;


    // Start is called before the first frame update
    void Start()
    {
        waveManager.SetGoal(goalPoint);
    }

}
