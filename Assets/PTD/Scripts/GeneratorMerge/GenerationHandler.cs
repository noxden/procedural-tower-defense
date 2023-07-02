using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Implement different generation calls as async -> Await cast and so on
public class GenerationHandler : MonoBehaviour
{
    public bool generateInstantly = false;
    public static GenerationHandler instance { get; set; }

    [Header("Generation Settings")]
    public Vector2Int gridSize = new Vector2Int(10, 10);
    public Vector2Int startPositionIndex;
    public Vector2Int endPositionIndex;

    [HideInInspector]
    public int pathLength;

    //# Private variables 
    private NodeManager nodeManager;
    public PathGenerator pathGenerator { get; private set; } //< Is required to be accessible by GenerationHandlerEditor right now
    public WaveFunctionSolver waveFunctionSolver { get; private set; }

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

    public void GenerateLevel()
    {
        GeneratePath();
    }

    public void GeneratePath()
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

    public void GenerateTilemap()
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
