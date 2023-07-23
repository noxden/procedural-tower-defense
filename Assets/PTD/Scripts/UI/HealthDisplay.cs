//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private CastleManagerScriptableObject castleManager;
    [SerializeField] private TextMeshProUGUI healthText;

    private void OnEnable()
    {
        castleManager.healthChangedEvent.AddListener(UpdateHealth);
    }

    private void OnDisable()
    {
        castleManager.healthChangedEvent.RemoveListener(UpdateHealth);
    }

    private void UpdateHealth()
    {
        healthText.text = castleManager.CurrentHealth.ToString();
    }
}
