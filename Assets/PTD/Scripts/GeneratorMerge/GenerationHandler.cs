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
        // Here, potential pre- or post-generation methods could be called as well
        GenerateHeight();
        GeneratePath();
        GenerateTilemap();
    }

    private void GenerateHeight()
    {
        waveFunctionSolver.SolveStepwise();
    }

    private void GeneratePath()
    {
        pathGenerator.Generate();
    }

    private void GenerateTilemap()
    {

    }
}
