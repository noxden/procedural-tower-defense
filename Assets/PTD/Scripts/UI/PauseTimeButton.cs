//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;
using UnityEngine.UI;

public class PauseTimeButton : MonoBehaviour
{
    [SerializeField] private TimeManagerScriptableObject timeManager;
    [SerializeField] private Image pauseImage;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite playSprite;
    private bool isPaused = false;

    public void SwitchPauseState()
    {
        if(isPaused)
        {
            timeManager.ResumeGame();
            pauseImage.sprite = pauseSprite;
            isPaused = false;
        }
        else
        {
            timeManager.PauseGame();
            pauseImage.sprite = playSprite;
            isPaused = true;
        }
    }
}
