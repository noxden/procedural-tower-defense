using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationHandler : MonoBehaviour
{
    [SerializeField] private bool GENERATE_ALL = false;    //! FOR DEBUG PURPOSES ONLY
    public static GenerationHandler instance { get; set; }
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
        pathGenerator.Generate();
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
        waveFunctionSolver.SolveStepwise();
    }
}
