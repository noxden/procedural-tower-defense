using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHandler_StartGame : MonoBehaviour
{
    private Button button;

    private void OnEnable() => GenerationHandler.OnLevelGeneratedStateChanged.AddListener(UpdateButtonState);
    private void OnDisable() => GenerationHandler.OnLevelGeneratedStateChanged.RemoveListener(UpdateButtonState);
    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void UpdateButtonState(bool newState)
    {
        button.interactable = newState;
    }
}
