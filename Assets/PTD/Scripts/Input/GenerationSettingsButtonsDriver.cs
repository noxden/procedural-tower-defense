using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[DefaultExecutionOrder(3)]
public class GenerationSettingsButtonsDriver : MonoBehaviour
{
    [Header("Grid Size Fields")] [SerializeField]
    private TMP_InputField gridSizeXField;

    [SerializeField] private TMP_InputField gridSizeYField;

    [Header("Start Position Fields")] [SerializeField]
    private SettingsSliderGroup startPosX;

    [SerializeField] private SettingsSliderGroup startPosY;

    [Header("End Position Fields")] [SerializeField]
    private SettingsSliderGroup endPosX;

    [SerializeField] private SettingsSliderGroup endPosY;

    [Header("Other Fields")] [SerializeField]
    private SettingsSliderGroup pathLengthSliderGroup;

    [SerializeField] private Toggle generateInstantlyToggle;


    private void OnEnable()
    {
        gridSizeXField.onEndEdit.AddListener(ChangeGridSizeX);
        gridSizeYField.onEndEdit.AddListener(ChangeGridSizeY);

        startPosX.onSliderValueChanged.AddListener(ChangeStartPosX);
        startPosY.onSliderValueChanged.AddListener(ChangeStartPosY);

        endPosX.onSliderValueChanged.AddListener(ChangeEndPosX);
        endPosY.onSliderValueChanged.AddListener(ChangeEndPosY);

        pathLengthSliderGroup.onSliderValueChanged.AddListener(ChangePathLength);
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

        startPosX.onSliderValueChanged.RemoveListener(ChangeStartPosX);
        startPosY.onSliderValueChanged.RemoveListener(ChangeStartPosY);

        endPosX.onSliderValueChanged.RemoveListener(ChangeEndPosX);
        endPosY.onSliderValueChanged.RemoveListener(ChangeEndPosY);

        pathLengthSliderGroup.onSliderValueChanged.RemoveListener(ChangePathLength);
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

        //> Set starting values
        ChangeGridSizeX("10"); //< Set starting size
        ChangeGridSizeY("10");
        ChangeStartPosX(1);
        ChangeStartPosY(1);
        ChangeEndPosX(GenerationHandler.instance.gridSize.x); //< So that you can just press the "Generate Map" button right after game start 
        ChangeEndPosY(GenerationHandler.instance.gridSize.y); //  and the path start and end positions are already set to be able to generate a path.
        ChangeGenerateInstantlyValue(false);
    }

    //#> Slider to GenerationHandler 
    private void ChangeGridSizeX(string inputText)
    {
        if (!int.TryParse(inputText, out int value)) return;
        GenerationHandler.instance.gridSize = new Vector2Int(Mathf.Max(value, 2), GenerationHandler.instance.gridSize.y);
        startPosX.SetSliderMinMax(1, GenerationHandler.instance.gridSize.x);
        endPosX.SetSliderMinMax(1, GenerationHandler.instance.gridSize.x);

        NodeManager.instance.ResetGrid();
    }

    private void ChangeGridSizeY(string inputText)
    {
        if (!int.TryParse(inputText, out int value)) return;
        GenerationHandler.instance.gridSize = new Vector2Int(GenerationHandler.instance.gridSize.x, Mathf.Max(value, 2));
        startPosY.SetSliderMinMax(1, GenerationHandler.instance.gridSize.y);
        endPosY.SetSliderMinMax(1, GenerationHandler.instance.gridSize.y);

        NodeManager.instance.ResetGrid();
    }

    private static void ChangeStartPosX(float value) =>
        GenerationHandler.instance.startPositionIndex = new Vector2Int(Mathf.FloorToInt(value) - 1, GenerationHandler.instance.startPositionIndex.y);

    private static void ChangeStartPosY(float value) =>
        GenerationHandler.instance.startPositionIndex = new Vector2Int(GenerationHandler.instance.startPositionIndex.x, Mathf.FloorToInt(value) - 1);

    private static void ChangeEndPosX(float value) =>
        GenerationHandler.instance.endPositionIndex = new Vector2Int(Mathf.FloorToInt(value) - 1, GenerationHandler.instance.endPositionIndex.y);

    private static void ChangeEndPosY(float value) =>
        GenerationHandler.instance.endPositionIndex = new Vector2Int(GenerationHandler.instance.endPositionIndex.x, Mathf.FloorToInt(value) - 1);

    private static void ChangePathLength(float value) => GenerationHandler.instance.pathLength = Mathf.FloorToInt(value);

    private static void ChangeGenerateInstantlyValue(bool value) => GenerationHandler.instance.generateInstantly = value;

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

    private void UpdatePathLength(float value) => pathLengthSliderGroup.DisplayCurrentValue(value);

    private void RecalculatePathSliderMin(Vector2Int value)
    {
        int minLength = GenerationHandler.instance.GetPathMinMax().x;
        pathLengthSliderGroup.SetSliderMin(minLength);
    }

    private void RecalculatePathSliderMax(Vector2Int newGridSize)
    {
        int maxLength = GenerationHandler.instance.GetPathMinMax().y;
        pathLengthSliderGroup.SetSliderMax(maxLength);
    }
}