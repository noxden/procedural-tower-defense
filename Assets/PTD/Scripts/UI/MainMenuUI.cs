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
    }

    private void OnDisable()
    {
        gameEventManager.startGameEvent.RemoveListener(DisableMenu);
    }

    public void DisableMenu()
    {
        menuParent.SetActive(false);
    }
}
