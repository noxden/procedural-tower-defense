using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Implement different generation calls as async -> Await cast and so on
public class GenerationHandler : MonoBehaviour
{
    public static GenerationHandler instance { get; set; }

    [Header("Generation Settings")]
    public bool generateInstantly = false;
    public Vector2Int gridSize;
    public Vector2Int startPositionIndex;
    public Vector2Int endPositionIndex;

    [HideInInspector]
    public int pathLength;

    //# Private variables 
    //> Are required to be public to allow access to GenerationHandlerEditor
    public NodeManager nodeManager { get; private set; }
    public PathGenerator pathGenerator { get; private set; }
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
        pathGenerator.OnPathGenerated.AddListener(GenerateTilemap);
    }

    public void GeneratePath()
    {
        Debug.Log($"[Generator] Starting path generation.");
        pathGenerator.Generate(generateInstantly);
    }

    public void GenerateTilemap()
    {
        Debug.Log($"[Generator] Starting tilemap generation.");
        if (generateInstantly)
            waveFunctionSolver.SolveInstantly();
        else
            waveFunctionSolver.SolveStepwise();

        pathGenerator.OnPathGenerated.RemoveListener(GenerateTilemap);
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
