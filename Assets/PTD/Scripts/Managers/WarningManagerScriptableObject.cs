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
