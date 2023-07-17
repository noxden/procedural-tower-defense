//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

public class ResultUI : MonoBehaviour
{
    [SerializeField] private GameEventManagerScriptableObject gameEventManager;
    [SerializeField] private TimeManagerScriptableObject timeManager;
    [SerializeField] private GameObject winScreen;
    [SerializeField] private GameObject loseScreen;

    private void Start()
    {
        HideScreens();
    }

    private void OnEnable()
    {
        gameEventManager.loseGameEvent.AddListener(ShowLoseScreen);
        gameEventManager.openMainMenuEvent.AddListener(HideScreens);
    }

    private void OnDisable()
    {
        gameEventManager.loseGameEvent.RemoveListener(ShowLoseScreen);
        gameEventManager.openMainMenuEvent.RemoveListener(HideScreens);
    }

    public void HideScreens()
    {
        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public void ShowLoseScreen()
    {
        timeManager.PauseGame();
        loseScreen.SetActive(true);
    }
}
