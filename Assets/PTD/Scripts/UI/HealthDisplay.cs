using System.Collections;
using System.Collections.Generic;
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
