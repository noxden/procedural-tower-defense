//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;

public class PathGenFailedWarning : MonoBehaviour
{
    [SerializeField] private WarningManagerScriptableObject warningManager;

    private void OnEnable() => PathGenerator.onPathFailed.AddListener(DisplayWarning);
    private void OnDisable() => PathGenerator.onPathFailed.RemoveListener(DisplayWarning);

    private void DisplayWarning() => warningManager.UpdateWarningUI("Failed to generate path. Click the button to retry!");
}