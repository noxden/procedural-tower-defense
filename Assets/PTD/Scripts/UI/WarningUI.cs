//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using TMPro;
using UnityEngine;
using System.Collections;

public class WarningUI : MonoBehaviour
{
    [SerializeField] private WarningManagerScriptableObject warningManager;
    [SerializeField] private GameObject warningBackground;
    [SerializeField] private TextMeshProUGUI warningText;
    [SerializeField] private float fadeOutTime;

    private void OnEnable()
    {
        warningManager.warningEvent.AddListener(UpdateWarningUI);
    }

    private void OnDisable()
    {
        warningManager.warningEvent.RemoveListener(UpdateWarningUI);
    }

    public void UpdateWarningUI(string warning)
    {
        warningText.text = warning;
        warningBackground.SetActive(true);
        StopAllCoroutines();
        StartCoroutine(FadeOutWarning());
    }

    private IEnumerator FadeOutWarning()
    {
        yield return new WaitForSeconds(fadeOutTime);
        warningBackground.SetActive(false);
    }
}
