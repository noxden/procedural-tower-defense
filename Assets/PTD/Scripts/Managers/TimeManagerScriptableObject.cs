//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

[CreateAssetMenu(fileName = "Time Manager", menuName = "ScriptableObjects/Managers/Time Manager")]
public class TimeManagerScriptableObject : ScriptableObject
{
    public float timeScale = 1;
    private bool isPaused = false;

    private void OnEnable()
    {
        timeScale = 1;
        isPaused = false;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        isPaused = true;
    }

    public bool IsPaused()
    {
        return isPaused;
    }

    public void ResumeGame()
    {
        Time.timeScale = timeScale;
        isPaused = false;
    }

    public void SetTimeScale(float timeScale)
    {
        this.timeScale = timeScale;
        Time.timeScale = timeScale;
    }
}
