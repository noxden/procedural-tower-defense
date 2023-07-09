using System.Collections;
using System.Collections.Generic;
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
