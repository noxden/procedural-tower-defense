//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

/// <summary> Handles input of the assigned InputActionAsset. Intended for handling the inputs used to move, rotate and zoom the camera. </summary>
public class InputHandler : MonoBehaviour
{
    public InputActionAsset actionAsset;
    
    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction zoomAction;

    public static UnityAction<Vector2> OnMovementInput;
    public static UnityAction<Vector2> OnRotationInput;
    public static UnityAction<Vector2> OnZoomInput;

    private void Awake()
    {
        movementAction = actionAsset.FindAction("Move");
        rotationAction = actionAsset.FindAction("Rotate");
        zoomAction = actionAsset.FindAction("Zoom");
    }

    private void Update()
    {
        if (movementAction.inProgress)
        {
            Vector2 movementInput = movementAction.ReadValue<Vector2>();
            // Debug.Log($"[Conditional Update] movementInput: {movementInput}");
            OnMovementInput.Invoke(movementInput);
        }
        if (rotationAction.inProgress)
        {
            Vector2 rotationInput = rotationAction.ReadValue<Vector2>();
            // Debug.Log($"[Conditional Update] rotationInput: {rotationInput}");
            OnRotationInput.Invoke(rotationInput);
        }
        if (zoomAction.inProgress)
        {
            Vector2 zoomInput = zoomAction.ReadValue<Vector2>();
            // Debug.Log($"[Conditional Update] zoomInput: {zoomInput}");
            OnZoomInput.Invoke(zoomInput);
        }
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();

    public void EnableInput() => actionAsset?.Enable();
    public void DisableInput() => actionAsset?.Disable();
}
