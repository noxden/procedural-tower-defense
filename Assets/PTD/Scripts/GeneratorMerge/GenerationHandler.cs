using System.Collections;
using UnityEngine;
using UnityEngine.Events;

// TODO: Implement different generation calls as async -> Await cast and so on
public class GenerationHandler : MonoBehaviour
{
    public static GenerationHandler instance { get; set; }

    [SerializeField] private GameEventManagerScriptableObject gameEventManager;

    [Header("Generation Settings")]
    public bool generateInstantly = false;

    [SerializeField] private Vector2Int m_gridSize = Vector2Int.zero;
    public Vector2Int gridSize
    {
        get => m_gridSize;
        set
        {
            m_gridSize = new Vector2Int(Mathf.Clamp(value.x, 1, 999), Mathf.Clamp(value.y, 1, 999));
            ClampStartEndPos(m_gridSize);

            OnGridSizeChanged?.Invoke(gridSize);
        }
    }

    [SerializeField] private Vector2Int m_startPositionIndex;
    public Vector2Int startPositionIndex
    {
        get => m_startPositionIndex;
        set
        {
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
            int clampedPathLength = ClampPathLength(value);
            m_pathLength = clampedPathLength;
            OnPathLengthChanged?.Invoke(clampedPathLength);
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
    //> Are required to be public to allow access to GenerationHandlerEditor
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

        pathGenerator.Generate(true);
        isRegenerating = false;
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

    private void ClampStartEndPos(Vector2Int newGridSize)   //< This method has been introduced to clean up the gridSize property field.
    {
        //> Ensure positions are within the gridsize
        m_startPositionIndex.x = Mathf.Clamp(startPositionIndex.x, 0, Mathf.Max(0, newGridSize.x - 1));
        m_startPositionIndex.y = Mathf.Clamp(startPositionIndex.y, 0, Mathf.Max(0, newGridSize.y - 1));
        m_endPositionIndex.x = Mathf.Clamp(endPositionIndex.x, 0, Mathf.Max(0, newGridSize.x - 1));
        m_endPositionIndex.y = Mathf.Clamp(endPositionIndex.y, 0, Mathf.Max(0, newGridSize.y - 1));
    }

    private int ClampPathLength(int newLength)
    {
        Vector2Int pathMinMax = GetPathMinMax();
        int minLength = pathMinMax.x;   //< Just for better readability, I could've also just 
        int maxLength = pathMinMax.y;   //  used the pathMinMax variable directly in the code below.

        if (newLength < minLength)
        {
            newLength = minLength;
            Debug.LogWarning("Path length is too short, setting to shortest distance");
        }

        if (newLength > maxLength)
        {
            newLength = maxLength;
            Debug.LogWarning("Path length is too long, setting to max length");
        };

        if (newLength % 2 != minLength % 2)
        {
            if (newLength == maxLength)
                newLength--;
            else
                newLength++;
            Debug.LogWarning("Path cannot end, setting it to an " + (minLength % 2 == 0 ? "even" : "uneven") + " number");
        }

        return newLength;
    }
    
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
        ClampStartEndPos(m_gridSize);
        m_pathLength = ClampPathLength(m_pathLength);
    }

}
