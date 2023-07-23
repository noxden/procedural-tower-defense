//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;
using UnityEngine.UI;

/// <summary> Handles the "Start Game" button, turning it off while no level is generated and turning it back on once a level is generated. </summary>
[RequireComponent(typeof(Button))]
public class ButtonHandler_StartGame : MonoBehaviour
{
    private Button button;

    private void OnEnable() => GenerationHandler.OnLevelGeneratedStateChanged.AddListener(UpdateButtonState);
    private void OnDisable() => GenerationHandler.OnLevelGeneratedStateChanged.RemoveListener(UpdateButtonState);
    private void Awake() => button = GetComponent<Button>();

    private void UpdateButtonState(bool newState) => button.interactable = newState;
}