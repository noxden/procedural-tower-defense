//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Game Event Manager", menuName = "ScriptableObjects/Managers/Game Event Manager")]
public class GameEventManagerScriptableObject : ScriptableObject
{
    [SerializeField] private TimeManagerScriptableObject timeManager;
    [System.NonSerialized] public UnityEvent startGameEvent;
    [System.NonSerialized] public UnityEvent generateMapEvent;
    [System.NonSerialized] public UnityEvent nodeGridGeneratedEvent;
    [System.NonSerialized] public UnityEvent loseGameEvent;
    [System.NonSerialized] public UnityEvent openMainMenuEvent;

    private void OnEnable()
    {
        if (startGameEvent == null)
            startGameEvent = new UnityEvent();
        if (generateMapEvent == null)
            generateMapEvent = new UnityEvent();
        if (nodeGridGeneratedEvent == null)
            nodeGridGeneratedEvent = new UnityEvent();
        if (loseGameEvent == null)
            loseGameEvent = new UnityEvent();
        if (openMainMenuEvent == null)
            openMainMenuEvent = new UnityEvent();

    }

    public void StartGame()
    {
        timeManager.ResumeGame();
        startGameEvent.Invoke();
    }

    public void GenerateMap()
    {
        generateMapEvent.Invoke();
    }

    public void NodeGridRegenerated()
    {
        nodeGridGeneratedEvent.Invoke();
    }

    public void LoseGame()
    {
        loseGameEvent.Invoke();
    }

    public void OpenMainMenu()
    {
        timeManager.PauseGame();
        openMainMenuEvent.Invoke();
    }
}
