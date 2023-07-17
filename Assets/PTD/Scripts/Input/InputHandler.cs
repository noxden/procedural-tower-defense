using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;

public class InputHandler : MonoBehaviour
{
    public InputActionAsset actionAsset;

    private InputAction movementAction;
    private InputAction rotationAction;
    private InputAction zoomAction;

    public static UnityAction<Vector2> onMovementInput;
    public static UnityAction<Vector2> onRotationInput;
    public static UnityAction<Vector2> onZoomInput;

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
            onMovementInput.Invoke(movementInput);
        }

        if (rotationAction.inProgress)
        {
            Vector2 rotationInput = rotationAction.ReadValue<Vector2>();
            // Debug.Log($"[Conditional Update] rotationInput: {rotationInput}");
            onRotationInput.Invoke(rotationInput);
        }

        if (zoomAction.inProgress)
        {
            Vector2 zoomInput = zoomAction.ReadValue<Vector2>();
            // Debug.Log($"[Conditional Update] zoomInput: {zoomInput}");
            onZoomInput.Invoke(zoomInput);
        }
    }

    private void OnEnable() => EnableInput();
    private void OnDisable() => DisableInput();

    private void EnableInput() => actionAsset.Enable();
    private void DisableInput() => actionAsset.Disable();
}