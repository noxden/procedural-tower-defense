//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeHandler : MonoBehaviour
{
    public static void Restart()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadSceneAsync(currentScene.buildIndex);
    }
}
