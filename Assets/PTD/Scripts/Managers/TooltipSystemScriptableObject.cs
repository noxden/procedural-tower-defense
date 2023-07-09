using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Tooltip System", menuName = "ScriptableObjects/Managers/Tooltip System")]
public class TooltipSystemScriptableObject : ScriptableObject
{
    [System.NonSerialized] public UnityEvent<string, string> showTooltipEvent;
    [System.NonSerialized] public UnityEvent hideTooltipEvent;

    private void OnEnable()
    {
        if (showTooltipEvent == null)
            showTooltipEvent = new UnityEvent<string, string>();
        if(hideTooltipEvent == null)
            hideTooltipEvent = new UnityEvent();
    }

    public void Show(string content, string header = "")
    {
        showTooltipEvent.Invoke(content, header);
    }

    public void Hide()
    {
        hideTooltipEvent.Invoke();
    }
}
