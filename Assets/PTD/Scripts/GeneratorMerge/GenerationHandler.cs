using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

// TODO: Implement different generation calls as async -> Await cast and so on
public class GenerationHandler : MonoBehaviour
{
    public static GenerationHandler instance { get; private set; }

    [SerializeField] private GameEventManagerScriptableObject gameEventManager; //< Added by Jan


    #region Variables and Properties

    [Header("Generation Settings")] public bool generateInstantly;

    [FormerlySerializedAs("m_gridSize")] [SerializeField]
    private Vector2Int mGridSize = Vector2Int.zero;

    public Vector2Int gridSize
    {
        get => mGridSize;
        set
        {
            mGridSize = new Vector2Int(Mathf.Clamp(value.x, 1, 999), Mathf.Clamp(value.y, 1, 999));
            ClampStartEndPos(mGridSize);

            OnGridSizeChanged?.Invoke(gridSize);
        }
    }

    [FormerlySerializedAs("m_startPositionIndex")] [SerializeField]
    private Vector2Int mStartPositionIndex;

    public Vector2Int startPositionIndex
    {
        get => mStartPositionIndex;
        set
        {
            mStartPositionIndex = value;
            OnStartPositionChanged?.Invoke(value);
        }
    }

    [FormerlySerializedAs("m_endPositionIndex")] [SerializeField]
    private Vector2Int mEndPositionIndex;

    public Vector2Int endPositionIndex
    {
        get => mEndPositionIndex;
        set
        {
            mEndPositionIndex = value;
            OnEndPositionChanged?.Invoke(value);
        }
    }

    private int mPathLength;

    public int pathLength
    {
        get => mPathLength;
        set
        {
            int clampedPathLength = ClampPathLength(value);
            mPathLength = clampedPathLength;
            OnPathLengthChanged?.Invoke(clampedPathLength);
        }
    }

    private bool mIsLevelGenerated;

    public bool isLevelGenerated
    {
        get => mIsLevelGenerated;
        set
        {
            mIsLevelGenerated = value;
            OnLevelGeneratedStateChanged?.Invoke(value);
        }
    }

    private bool isRegenerating;

    #endregion

    #region References to the Generator instances

    //> Are required to be public to allow access to GenerationHandlerEditor
    public NodeManager nodeManager { get; private set; }
    public PathGenerator pathGenerator { get; private set; }
    public WaveFunctionSolver waveFunctionSolver { get; private set; }

    #endregion

    #region Generation Events

    public static readonly UnityEvent<bool> OnLevelGeneratedStateChanged = new UnityEvent<bool>();
    public static readonly UnityEvent<Vector2Int> OnGridSizeChanged = new UnityEvent<Vector2Int>();
    public static readonly UnityEvent<Vector2Int> OnStartPositionChanged = new UnityEvent<Vector2Int>();
    public static readonly UnityEvent<Vector2Int> OnEndPositionChanged = new UnityEvent<Vector2Int>();
    public static readonly UnityEvent<float> OnPathLengthChanged = new UnityEvent<float>();

    #endregion

    #region Monobehaviour Methods

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        nodeManager = NodeManager.instance;
        pathGenerator = FindObjectOfType<PathGenerator>();
        waveFunctionSolver = FindObjectOfType<WaveFunctionSolver>();

        isLevelGenerated = false;
    }

    private void OnEnable() => gameEventManager.generateMapEvent.AddListener(RegenerateLevel);

    private void OnDisable() => gameEventManager.generateMapEvent.RemoveListener(RegenerateLevel);

    #endregion

    #region Generation Methods

    private void RegenerateLevel()
    {
        if (isRegenerating)
            return;

        isRegenerating = true;
        StartCoroutine(RegenerateNextLevel());
    }

    private IEnumerator RegenerateNextLevel()
    {
        isLevelGenerated = false;
        nodeManager.ResetGrid();
        //> Requires a brief wait time here to work properly, that's why this method is called as an IEnumerator. I dunno why that's necessary. It's probably
        //  due to one of the methods in ResetGrid() taking longer than expected and thereby preventing variables from being assigned properly if not waited for.
        //> I could not figure out how else to implement this without using async tasks, and my attempt at implementing async tasks did not work out at all :(
        yield return new WaitForSecondsRealtime(0.0001f);

        GenerateLevel();
    }

    public void GenerateLevel() //< Only public to allow GenerationHandlerEditor access (for custom inspector button)
    {
        GeneratePath();
        PathGenerator.onPathGenerated.AddListener(GenerateTilemap); //< So that if the path generates successfully, its event calls GenerateTilemap.
        //  This way the landscape is only generated if the path generated successfully.
    }

    private void GeneratePath()
    {
        Debug.Log($"[Generator] Starting path generation.");

        pathGenerator.Generate(); //< Always generate path instantly, as showing it stepwise can lead to the path generator taking ages.
        isRegenerating = false; //< If path generator fails, GenerateTilemap is never called, hence isRegenerating needs to be set to false here already.
    }

    public void GenerateTilemap() //< Only public to allow GenerationHandlerEditor access (for custom inspector button)
    {
        Debug.Log($"[Generator] Starting tilemap generation.");
        waveFunctionSolver.StartSolve(generateInstantly);

        isLevelGenerated = true;
        PathGenerator.onPathGenerated.RemoveListener(GenerateTilemap);
    }

    #endregion

    #region Methods for clamping values

    private void ClampStartEndPos(Vector2Int newGridSize) //< This method has been introduced to clean up the gridSize property field.
    {
        //> Ensure positions are within the gridSize
        mStartPositionIndex.x = Mathf.Clamp(startPositionIndex.x, 0, Mathf.Max(0, newGridSize.x - 1));
        mStartPositionIndex.y = Mathf.Clamp(startPositionIndex.y, 0, Mathf.Max(0, newGridSize.y - 1));
        mEndPositionIndex.x = Mathf.Clamp(endPositionIndex.x, 0, Mathf.Max(0, newGridSize.x - 1));
        mEndPositionIndex.y = Mathf.Clamp(endPositionIndex.y, 0, Mathf.Max(0, newGridSize.y - 1));
    }

    private int ClampPathLength(int newLength)
    {
        Vector2Int pathMinMax = GetPathMinMax();
        int minLength = pathMinMax.x; //< Just for better readability, I could've also just 
        int maxLength = pathMinMax.y; //  used the pathMinMax variable directly in the code below.

        if (newLength < minLength)
        {
            newLength = minLength;
            Debug.LogWarning("Path length is too short, setting to shortest distance");
        }

        if (newLength > maxLength)
        {
            newLength = maxLength;
            Debug.LogWarning("Path length is too long, setting to max length");
        }

        if (newLength % 2 == minLength % 2) return newLength;
        if (newLength == maxLength)
            newLength--;
        else
            newLength++;
        Debug.LogWarning("Path cannot end, setting it to an " + (minLength % 2 == 0 ? "even" : "uneven") + " number");

        return newLength;
    }

    #endregion

    public Vector2Int GetPathMinMax()
    {
        int pathMin = (Mathf.Abs(endPositionIndex.x - startPositionIndex.x) + Mathf.Abs(endPositionIndex.y - startPositionIndex.y)) + 1;
        int pathMax = gridSize.x * gridSize.y;
        return new Vector2Int(pathMin, pathMax);
    }

    private void OnValidate()
    {
        //> Modifying the values in the editor does not access the property fields to do that, thereby foregoing the validity-checks 
        //  built into them. Hence, the code below needs to be run to call those checks "manually".
        ClampStartEndPos(mGridSize);
        mPathLength = ClampPathLength(mPathLength);
    }
}