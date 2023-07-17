//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

public class SettingsSliderGroup : MonoBehaviour
{
    public TextMeshProUGUI minValueField;
    public TextMeshProUGUI maxValueField;
    public TextMeshProUGUI currentValueField;
    public Slider slider;
    public UnityEvent<float> OnSliderValueChanged = new UnityEvent<float>();

    private void OnEnable()
    {
        slider.onValueChanged.AddListener(ReceiveSliderValueChange);
    }
    private void OnDisable()
    {
        slider.onValueChanged.RemoveListener(ReceiveSliderValueChange);
    }

    private void Start()
    {
        slider.wholeNumbers = true;
    }

    private void ReceiveSliderValueChange(float value)
    {
        if (currentValueField != null)
            currentValueField.text = value.ToString();
        OnSliderValueChanged?.Invoke(value);
    }

    public void DisplayCurrentValue(float newValue)
    {
        slider.value = newValue;    //< So that moving the slider of the GenerationHandler's custom inspector also moves the in-game slider.
        currentValueField.text = newValue.ToString();
    }

    public void SetSliderMin(int value)
    {
        slider.minValue = value;
        if (minValueField != null)
            minValueField.text = value.ToString();
    }

    public void SetSliderMax(int value)
    {
        slider.maxValue = value;
        if (maxValueField != null)
            maxValueField.text = value.ToString();
    }

    public void SetSliderMinMax(int min, int max)
    {
        SetSliderMin(min);
        SetSliderMax(max);
    }

    public void SetSliderMinMax(Vector2Int composite)
    {
        SetSliderMinMax(composite.x, composite.y);
    }
}
