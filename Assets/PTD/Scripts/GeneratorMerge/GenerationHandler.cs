//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// TODO: Implement generation calls as async tasks
public class GenerationHandler : MonoBehaviour
{
    public static GenerationHandler instance { get; set; }

    [SerializeField] private GameEventManagerScriptableObject gameEventManager;  //< Added by Jan

    [Header("Generation Settings")]
    public bool generateInstantly = false;

    [SerializeField] private Vector2Int m_gridSize = Vector2Int.zero;
    public Vector2Int gridSize
    {
        get => m_gridSize;
        set
        {
            m_gridSize = value;

            m_startPositionIndex.x = Mathf.Clamp(startPositionIndex.x, 0, gridSize.x - 1);
            m_startPositionIndex.y = Mathf.Clamp(startPositionIndex.y, 0, gridSize.y - 1);
            m_endPositionIndex.x = Mathf.Clamp(endPositionIndex.x, 0, gridSize.x - 1);
            m_endPositionIndex.y = Mathf.Clamp(endPositionIndex.y, 0, gridSize.y - 1);

            OnGridSizeChanged?.Invoke(gridSize);
        }
    }

    [SerializeField] private Vector2Int m_startPositionIndex;
    public Vector2Int startPositionIndex
    {
        get => m_startPositionIndex;
        set
        {
            Vector2Int modifiedValue = value;

            // modifiedValue.x = Mathf.Clamp(modifiedValue.x, 0, gridSize.x - 1);
            // modifiedValue.y = Mathf.Clamp(modifiedValue.y, 0, gridSize.y - 1);

            m_startPositionIndex = value;
            OnStartPositionChanged?.Invoke(value);
        }
    }

    [SerializeField] private Vector2Int m_endPositionIndex;
    public Vector2Int endPositionIndex
    {
        get => m_endPositionIndex;
        set
        {
            Vector2Int modifiedValue = value;

            // modifiedValue.x = Mathf.Clamp(modifiedValue.x, 0, gridSize.x - 1);
            // modifiedValue.y = Mathf.Clamp(modifiedValue.y, 0, gridSize.y - 1);

            m_endPositionIndex = value;
            OnEndPositionChanged?.Invoke(value);
        }
    }

    private int m_pathLength;
    public int pathLength
    {
        get => m_pathLength;
        set
        {
            int modifiedValue = value;

            int shortestDistance = (Mathf.Abs(endPositionIndex.x - startPositionIndex.x) + Mathf.Abs(endPositionIndex.y - startPositionIndex.y)) + 1;
            if (modifiedValue < shortestDistance)
            {
                modifiedValue = shortestDistance;
                Debug.LogWarning("Path length is too short, setting to shortest distance");
            }

            if (modifiedValue % 2 != shortestDistance % 2)
            {
                modifiedValue++;
                Debug.LogWarning("Path cannot end, setting it to an " + (shortestDistance % 2 == 0 ? "even" : "uneven") + " number");
            }

            if (modifiedValue > gridSize.x * gridSize.y)
            {
                modifiedValue = gridSize.x * gridSize.y;
                Debug.LogWarning("Path length is too long, setting to max length");
            };

            m_pathLength = modifiedValue;
            OnPathLengthChanged?.Invoke(modifiedValue);
        }
    }

    private bool m_isLevelGenerated = false;
    public bool isLevelGenerated
    {
        get => m_isLevelGenerated;
        set { m_isLevelGenerated = value; OnLevelGeneratedStateChanged?.Invoke(value); }
    }
    private bool isRegenerating = false;

    //# Private variables 
    //> The following 3 fields are required to be public to allow access to GenerationHandlerEditor
    public NodeManager nodeManager { get; private set; }
    public PathGenerator pathGenerator { get; private set; }
    public WaveFunctionSolver waveFunctionSolver { get; private set; }

    public static UnityEvent<bool> OnLevelGeneratedStateChanged = new UnityEvent<bool>();
    public static UnityEvent<Vector2Int> OnGridSizeChanged = new UnityEvent<Vector2Int>();
    public static UnityEvent<Vector2Int> OnStartPositionChanged = new UnityEvent<Vector2Int>();
    public static UnityEvent<Vector2Int> OnEndPositionChanged = new UnityEvent<Vector2Int>();
    public static UnityEvent<float> OnPathLengthChanged = new UnityEvent<float>();

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

    private void OnEnable()
    {
        gameEventManager.generateMapEvent.AddListener(RegenerateLevel);
    }

    private void OnDisable()
    {
        gameEventManager.generateMapEvent.RemoveListener(RegenerateLevel);
    }

    public void RegenerateLevel()
    {
        if (isRegenerating)
            return;

        StartCoroutine(RegenerateNextLevel());
    }

    private IEnumerator RegenerateNextLevel()
    {
        isLevelGenerated = false;
        isRegenerating = true;

        nodeManager.Regenerate();
        yield return new WaitForSecondsRealtime(0.0001f);
        GenerateLevel();
    }

    public void GenerateLevel()
    {
        GeneratePath();
        pathGenerator.OnPathGenerated.AddListener(GenerateTilemap);
    }

    public void GeneratePath()
    {
        Debug.Log($"[Generator] Starting path generation.");

        pathGenerator.Generate(true);   //< Always generate path instantly, as showing it stepwise can lead to the path generator taking ages.
        isRegenerating = false;  //< If path generator fails, GenerateTilemap is never called, hence isRegenerating needs to be set to false here already.
    }

    public void GenerateTilemap()
    {
        Debug.Log($"[Generator] Starting tilemap generation.");
        if (generateInstantly)
            waveFunctionSolver.SolveInstantly();
        else
            waveFunctionSolver.SolveStepwise();

        isLevelGenerated = true;
        pathGenerator.OnPathGenerated.RemoveListener(GenerateTilemap);
    }

    private void OnValidate()
    {
        //> Ensure positions are within the gridsize
        m_startPositionIndex.x = Mathf.Clamp(startPositionIndex.x, 0, gridSize.x - 1);
        m_startPositionIndex.y = Mathf.Clamp(startPositionIndex.y, 0, gridSize.y - 1);

        m_endPositionIndex.x = Mathf.Clamp(endPositionIndex.x, 0, gridSize.x - 1);
        m_endPositionIndex.y = Mathf.Clamp(endPositionIndex.y, 0, gridSize.y - 1);

        int shortestDistance = (Mathf.Abs(endPositionIndex.x - startPositionIndex.x) + Mathf.Abs(endPositionIndex.y - startPositionIndex.y)) + 1;

        if (pathLength < shortestDistance)
        {
            pathLength = shortestDistance;
            Debug.LogWarning("Path length is too short, setting to shortest distance");
        }

        if (pathLength % 2 != shortestDistance % 2)
        {
            pathLength++;
            Debug.LogWarning("Path cannot end, setting it to an " + (shortestDistance % 2 == 0 ? "even" : "uneven") + " number");
        }

        if (pathLength > gridSize.x * gridSize.y)
        {
            pathLength = gridSize.x * gridSize.y;
            Debug.LogWarning("Path length is too long, setting to max length");
        }
    }

    public Vector2Int GetPathMinMax()
    {
        int pathMin = (Mathf.Abs(endPositionIndex.x - startPositionIndex.x) + Mathf.Abs(endPositionIndex.y - startPositionIndex.y)) + 1;
        int pathMax = gridSize.x * gridSize.y;
        return new Vector2Int(pathMin, pathMax);
    }
}
