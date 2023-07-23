//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

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
