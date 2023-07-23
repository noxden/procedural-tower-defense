//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField] private GameEventManagerScriptableObject gameEventManager;
    [SerializeField] private GameObject uiParent;

    private void Start()
    {
        uiParent.SetActive(false);
    }

    private void OnEnable()
    {
        gameEventManager.startGameEvent.AddListener(EnableMenu);
        gameEventManager.openMainMenuEvent.AddListener(DisableMenu);
    }

    private void OnDisable()
    {
        gameEventManager.startGameEvent.RemoveListener(EnableMenu);
        gameEventManager.openMainMenuEvent.RemoveListener(DisableMenu);
    }

    public void DisableMenu()
    {
        uiParent.SetActive(false);
    }

    public void EnableMenu()
    {
        uiParent.SetActive(true);
    }
}
