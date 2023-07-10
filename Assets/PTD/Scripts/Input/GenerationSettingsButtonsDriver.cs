using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(2)]
public class GenerationSettingsButtonsDriver : MonoBehaviour
{
    [Header("Grid Size Fields")]
    [SerializeField] private TMP_InputField gridSizeXField;
    [SerializeField] private TMP_InputField gridSizeYField;

    [Header("Start Position Fields")]
    [SerializeField] private SettingsSliderGroup startPosX;
    [SerializeField] private SettingsSliderGroup startPosY;

    [Header("End Position Fields")]
    [SerializeField] private SettingsSliderGroup endPosX;
    [SerializeField] private SettingsSliderGroup endPosY;

    [Header("Other Fields")]
    [SerializeField] private SettingsSliderGroup pathLengthSliderGroup;
    [SerializeField] private Toggle generateInstantlyToggle;


    private void OnEnable()
    {
        gridSizeXField.onEndEdit.AddListener(ChangeGridSizeX);
        gridSizeYField.onEndEdit.AddListener(ChangeGridSizeY);

        startPosX.OnSliderValueChanged.AddListener(ChangeStartPosX);
        startPosY.OnSliderValueChanged.AddListener(ChangeStartPosY);

        endPosX.OnSliderValueChanged.AddListener(ChangeEndPosX);
        endPosY.OnSliderValueChanged.AddListener(ChangeEndPosY);

        pathLengthSliderGroup.OnSliderValueChanged.AddListener(ChangePathLength);
        generateInstantlyToggle.onValueChanged.AddListener(ChangeGenerateInstantlyValue);

        GenerationHandler.OnGridSizeChanged.AddListener(RecalculatePathSliderMax);
        GenerationHandler.OnGridSizeChanged.AddListener(UpdateGridCurrent);

        GenerationHandler.OnStartPositionChanged.AddListener(UpdateStartPosCurrent);
        GenerationHandler.OnStartPositionChanged.AddListener(RecalculatePathSliderMin);

        GenerationHandler.OnEndPositionChanged.AddListener(UpdateEndPosCurrent);
        GenerationHandler.OnEndPositionChanged.AddListener(RecalculatePathSliderMin);

        GenerationHandler.OnPathLengthChanged.AddListener(UpdatePathLength);
    }
    private void OnDisable()
    {
        gridSizeXField.onEndEdit.RemoveListener(ChangeGridSizeX);
        gridSizeYField.onEndEdit.RemoveListener(ChangeGridSizeY);

        startPosX.OnSliderValueChanged.RemoveListener(ChangeStartPosX);
        startPosY.OnSliderValueChanged.RemoveListener(ChangeStartPosY);

        endPosX.OnSliderValueChanged.RemoveListener(ChangeEndPosX);
        endPosY.OnSliderValueChanged.RemoveListener(ChangeEndPosY);

        pathLengthSliderGroup.OnSliderValueChanged.RemoveListener(ChangePathLength);
        generateInstantlyToggle.onValueChanged.RemoveListener(ChangeGenerateInstantlyValue);

        GenerationHandler.OnGridSizeChanged.RemoveListener(RecalculatePathSliderMax);
        GenerationHandler.OnGridSizeChanged.RemoveListener(UpdateGridCurrent);

        GenerationHandler.OnStartPositionChanged.RemoveListener(UpdateStartPosCurrent);
        GenerationHandler.OnStartPositionChanged.RemoveListener(RecalculatePathSliderMin);

        GenerationHandler.OnEndPositionChanged.RemoveListener(UpdateEndPosCurrent);
        GenerationHandler.OnEndPositionChanged.RemoveListener(RecalculatePathSliderMin);

        GenerationHandler.OnPathLengthChanged.RemoveListener(UpdatePathLength);
    }

    private void Start()
    {
        gridSizeXField.characterLimit = 3;
        gridSizeYField.characterLimit = 3;

        ChangeGridSizeX("10");  //< Set starting size
        ChangeGridSizeY("10");
        ChangeGenerateInstantlyValue(false);
    }

    //#> Slider to GenerationHandler 
    private void ChangeGridSizeX(string inputText)
    {
        if (int.TryParse(inputText, out int value))
        {
            Debug.Log($"Changed grid size x to {value}.");
            GenerationHandler.instance.gridSize = new Vector2Int(Mathf.Max(value, 2), GenerationHandler.instance.gridSize.y);
            startPosX.SetSliderMinMax(1, GenerationHandler.instance.gridSize.x);
            endPosX.SetSliderMinMax(1, GenerationHandler.instance.gridSize.x);

            NodeManager.instance.Regenerate();
        }
    }
    private void ChangeGridSizeY(string inputText)
    {
        if (int.TryParse(inputText, out int value))
        {
            Debug.Log($"Changed grid size y to {value}.");
            GenerationHandler.instance.gridSize = new Vector2Int(GenerationHandler.instance.gridSize.x, Mathf.Max(value, 2));
            startPosY.SetSliderMinMax(1, GenerationHandler.instance.gridSize.y);
            endPosY.SetSliderMinMax(1, GenerationHandler.instance.gridSize.y);

            NodeManager.instance.Regenerate();
        }
    }

    private void ChangeStartPosX(float value)
    {
        GenerationHandler.instance.startPositionIndex = new Vector2Int(Mathf.FloorToInt(value) - 1, GenerationHandler.instance.startPositionIndex.y);
        // startPosX.SetCurrentValue(value);
    }
    private void ChangeStartPosY(float value)
    {
        GenerationHandler.instance.startPositionIndex = new Vector2Int(GenerationHandler.instance.startPositionIndex.x, Mathf.FloorToInt(value) - 1);
        // startPosY.SetCurrentValue(value);
    }
    private void ChangeEndPosX(float value)
    {
        GenerationHandler.instance.endPositionIndex = new Vector2Int(Mathf.FloorToInt(value) - 1, GenerationHandler.instance.endPositionIndex.y);
        // endPosX.SetCurrentValue(value);
    }
    private void ChangeEndPosY(float value)
    {
        GenerationHandler.instance.endPositionIndex = new Vector2Int(GenerationHandler.instance.endPositionIndex.x, Mathf.FloorToInt(value) - 1);
        // endPosY.SetCurrentValue(value);
    }
    private void ChangePathLength(float value)
    {
        GenerationHandler.instance.pathLength = Mathf.FloorToInt(value);
        // pathLengthSliderGroup.SetCurrentValue(value);
    }

    private void ChangeGenerateInstantlyValue(bool value)
    {
        GenerationHandler.instance.generateInstantly = value;
        // generateInstantlyToggle.isOn = value;
    }

    //#> GenerationHandler to Slider 
    private void UpdateGridCurrent(Vector2Int newGridSize)
    {
        gridSizeXField.text = newGridSize.x.ToString();
        gridSizeYField.text = newGridSize.y.ToString();
    }
    private void UpdateStartPosCurrent(Vector2Int value)
    {
        startPosX.DisplayCurrentValue(value.x + 1);
        startPosY.DisplayCurrentValue(value.y + 1);
    }
    private void UpdateEndPosCurrent(Vector2Int value)
    {
        endPosX.DisplayCurrentValue(value.x + 1);
        endPosY.DisplayCurrentValue(value.y + 1);
    }
    private void UpdatePathLength(float value)
    {
        pathLengthSliderGroup.DisplayCurrentValue(value);
    }

    private void RecalculatePathSliderMin(Vector2Int value)
    {
        int minLength = GenerationHandler.instance.GetPathMinMax().x;
        pathLengthSliderGroup.SetSliderMin(minLength);
    }
    private void RecalculatePathSliderMax(Vector2Int newGridSize)
    {
        pathLengthSliderGroup.SetSliderMax(newGridSize.x * newGridSize.y);
    }
}