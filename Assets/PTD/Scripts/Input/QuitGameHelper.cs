//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;
using UnityEditor;

/// <summary> To allow for UI button press to close the game by subscribing the Quit method to its ButtonPressed event. </summary>
public class QuitGameHelper : MonoBehaviour
{
    /// <summary> If method is run while in editor playmode, end playmode. Otherwise, quit the application. </summary>
    public void Quit()
    {
#if UNITY_EDITOR
        EditorApplication.isPlaying = false;
#elif UNITY_STANDALONE
        Application.Quit();
#endif
    }
}