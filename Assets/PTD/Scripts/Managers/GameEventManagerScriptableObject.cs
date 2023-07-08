using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Game Event Manager", menuName = "ScriptableObjects/Managers/Game Event Manager")]
public class GameEventManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent startGameEvent;
    [System.NonSerialized] public UnityEvent generateMapEvent;
    [System.NonSerialized] public UnityEvent nodeGridRegeneratedEvent;
    [System.NonSerialized] public UnityEvent loseGameEvent;

    private void OnEnable()
    {
        if(startGameEvent == null)
            startGameEvent = new UnityEvent();
        if(generateMapEvent == null)
            generateMapEvent = new UnityEvent();
        if(nodeGridRegeneratedEvent == null)
            nodeGridRegeneratedEvent = new UnityEvent();
        if(loseGameEvent == null)
            loseGameEvent = new UnityEvent();
    }

    public void StartGame()
    {
        startGameEvent.Invoke();
    }

    public void GenerateMap()
    {
          generateMapEvent.Invoke();
    }

    public void NodeGridRegenerated()
    {
        nodeGridRegeneratedEvent.Invoke();
    }

    public void LoseGame()
    {
        loseGameEvent.Invoke();
    }
}
