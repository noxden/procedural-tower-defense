//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;

public class Node : MonoBehaviour
{
    //# Public Variables 
    public int entropy { get => potentialTiles.Count; }  //< The entropy of a node, previously named "numberOfRemainingPotentialTiles"
    public Vector2Int gridPosition
    {
        get
        {
            return _gridPosition.Value;
        }
        set
        {
            if (_gridPosition.HasValue)
                UnregisterFromManager();
            _gridPosition = value;
            name = $"Node {gridPosition}";
            RegisterInManager();
        }
    }
    public UnityEvent<List<Tile>> OnPotentialTilesUpdated { get; } = new UnityEvent<List<Tile>>();
    public List<Tile> potentialTiles
    {
        get
        {
            return _potentialTiles;
        }
        set
        {
            _potentialTiles = value;
            OnPotentialTilesUpdated?.Invoke(potentialTiles);
        }
    }

    //# Path Generation variables 
    public List<Vector2Int> possiblePathDirections = new List<Vector2Int>();
    public bool isPath = false;

    //# Private Variables 
    [SerializeField] private List<Tile> _potentialTiles;  //< Defines this node's superposition
    private Vector2Int? _gridPosition;
    private GameObject debugVisualizer;

    //# Monobehaviour Events 
    private void Start()
    {
        // CreateNodePositionVisualizer();
        InitializePossiblePathDirections();
    }

    private void OnDestroy()
    {
        UnregisterFromManager();
    }

    //# Public Methods 
    /// <summary>
    /// This function causes one of the potential tiles to be instantiated, thereby collapsing the superposition. It returns true if collapse was successful, false if entropy was 0.
    /// </summary>
    public bool Collapse()
    {
        if (entropy == 1)
            Instantiate(potentialTiles[0].prefab, this.transform, false);
        else if (entropy > 1)
        {
            Tile randomlyChosenTile = potentialTiles[Random.Range(0, potentialTiles.Count)];
            potentialTiles.RemoveAll(x => x != randomlyChosenTile);  //< Removes every entry but randomlyChosenTile from potentialTiles list
            Instantiate(randomlyChosenTile.prefab, this.transform, false);
        }
        else
        {
            Debug.LogError($"Tile {this.name} does not have any potential tiles left.");
            return false;
        }
        // RemoveNodePositionVisualizer();    //< Only remove visualizer if collapsing was successful
        return true;
    }

    /// <summary>
    /// Returns true if potentialTiles were reduced by limiter, false if not.
    /// </summary>
    public bool ReducePotentialTilesByLimiter(List<Tile> limiter)
    {
        List<Tile> reducedPotentialTiles = new List<Tile>(potentialTiles);
        foreach (Tile entry in potentialTiles)
        {
            if (!limiter.Contains(entry))
                reducedPotentialTiles.Remove(entry);
        }
        if (reducedPotentialTiles.SequenceEqual(potentialTiles))
        {
            return false;
        }
        else
        {
            potentialTiles = reducedPotentialTiles;
            return true;
        }
    }

    public void ReducePotentialTilesByPath()
    {
        List<Tile> reducedPotentialTiles = new List<Tile>(potentialTiles);
        foreach (Tile entry in potentialTiles)
        {
            if (entry.isPath != this.isPath)
                reducedPotentialTiles.Remove(entry);
        }

        potentialTiles = reducedPotentialTiles;
    }

    //# Private Methods 
    private void RegisterInManager() => NodeManager.instance.RegisterNode(gridPosition, this);

    private void UnregisterFromManager() => NodeManager.instance.UnregisterNode(gridPosition);

    private void CreateNodePositionVisualizer() => gameObject.AddComponent<SuperpositionVisualizer>();

    private void RemoveNodePositionVisualizer() => gameObject.GetComponent<SuperpositionVisualizer>().Remove();

    private void InitializePossiblePathDirections()
    {
        if (gridPosition.x != 0)
            possiblePathDirections.Add(new Vector2Int(-1, 0));
        if (gridPosition.x != GenerationHandler.instance.gridSize.x - 1)
            possiblePathDirections.Add(new Vector2Int(1, 0));
        if (gridPosition.y != 0)
            possiblePathDirections.Add(new Vector2Int(0, -1));
        if (gridPosition.y != GenerationHandler.instance.gridSize.y - 1)
            possiblePathDirections.Add(new Vector2Int(0, 1));
    }
}
