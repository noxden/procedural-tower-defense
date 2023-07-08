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
    }

    private void OnDisable()
    {
        gameEventManager.startGameEvent.RemoveListener(EnableMenu);
    }

    public void EnableMenu()
    {
        uiParent.SetActive(true);
    }
}
