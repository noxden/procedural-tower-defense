using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationHandler : MonoBehaviour
{
    [SerializeField] private bool GENERATE_ALL = false;    //! FOR DEBUG PURPOSES ONLY
    public bool generateInstantly = false;
    public static GenerationHandler instance { get; set; }

    [Header("Generation Settings")]
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public Vector2Int startPositionIndex;
    public Vector2Int endPositionIndex;
    public int pathLength;

    //# Private variables 
    private NodeManager nodeManager;
    private PathGenerator pathGenerator;
    private WaveFunctionSolver waveFunctionSolver;

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
    }

    /// <summary>
    /// Update is called every frame, if the MonoBehaviour is enabled.
    /// </summary>
    private void Update()
    {
        if (GENERATE_ALL)
        {
            GenerateLevel();
            GENERATE_ALL = false;
        }
    }

    public void GenerateLevel()
    {
        GeneratePath();
    }

    private void GeneratePath()
    {
        Debug.Log($"[Generator] Starting path generation.");
        pathGenerator.Generate(generateInstantly);
        pathGenerator.OnPathGenerated.AddListener(PostPathGeneration);
    }

    private void PostPathGeneration()
    {
        Debug.Log($"[Generator] Starting post path generation.");
        Dictionary<Vector2Int, Node> nodeGrid = NodeManager.instance.nodeGrid;
        foreach (var keyValuePair in nodeGrid)
        {
            Node node = keyValuePair.Value;
            node.ReducePotentialTilesByPath();
        }
        pathGenerator.OnPathGenerated.RemoveListener(PostPathGeneration);
        Debug.Log($"[Generator] Finished post path generation.");

        GenerateTilemap();
    }

    private void GenerateTilemap()
    {
        if (generateInstantly)
            waveFunctionSolver.SolveInstantly();
        else
            waveFunctionSolver.SolveStepwise();
    }

    private void OnValidate()
    {
        //> Ensure positions are within the gridsize
        startPositionIndex.x = Mathf.Clamp(startPositionIndex.x, 0, gridSize.x - 1);
        startPositionIndex.y = Mathf.Clamp(startPositionIndex.y, 0, gridSize.y - 1);

        endPositionIndex.x = Mathf.Clamp(endPositionIndex.x, 0, gridSize.x - 1);
        endPositionIndex.y = Mathf.Clamp(endPositionIndex.y, 0, gridSize.y - 1);

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


}
