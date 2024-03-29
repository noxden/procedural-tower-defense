//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameEventManagerScriptableObject gameEventManager;
    [SerializeField] private GameObject menuParent;

    private void Start()
    {
        menuParent.SetActive(true);
    }

    private void OnEnable()
    {
        gameEventManager.startGameEvent.AddListener(DisableMenu);
        gameEventManager.openMainMenuEvent.AddListener(EnableMenu);
    }

    private void OnDisable()
    {
        gameEventManager.startGameEvent.RemoveListener(DisableMenu);
        gameEventManager.openMainMenuEvent.RemoveListener(EnableMenu);
    }

    public void EnableMenu()
    {
        menuParent.SetActive(true);
    }

    public void DisableMenu()
    {
        menuParent.SetActive(false);
    }
}
