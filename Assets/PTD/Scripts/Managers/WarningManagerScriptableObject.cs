//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine.Events;
using UnityEngine;

[CreateAssetMenu(fileName = "Warning Manager", menuName = "ScriptableObjects/Managers/Warning")]
public class WarningManagerScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent<string> warningEvent;

    private void OnEnable()
    {
        if (warningEvent == null)
            warningEvent = new UnityEvent<string>();
    }

    public void ResetScriptableObject()
    {
        warningEvent = new UnityEvent<string>();
    }

    public void UpdateWarningUI(string warning)
    {
        warningEvent.Invoke(warning);
    }
}
