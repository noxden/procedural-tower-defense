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
    public int entropy { get { return potentialTiles.Count; } }  //< "The entropy of a cell / node.", previously named "numberOfRemainingPotentialTiles"
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
    [SerializeField] 
    private List<Tile> _potentialTiles;  //< Defines this node's superposition
    private Vector2Int? _gridPosition;
    private GameObject debugVisualizer;

    private void Start()
    {
        CreateNodePositionVisualizer();
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        UnregisterFromManager();
    }

    //# Public Methods 
    /// <summary>
    /// This function causes one of the potential tiles to be instantiated, thereby collapsing the superposition. It returns true if collapse was successful, false if entropy was 0.
    /// </summary>
    public bool Collapse()  //< Returns true if collapse was successful     //? What should happen in the case that entropy does hit zero?
    {
        if (entropy == 1)
            Instantiate(potentialTiles[0].prefab, this.transform, false);
        else if (entropy > 1)
        {
            Tile randomlyChosenTile = potentialTiles[Random.Range(0, potentialTiles.Count)];
            potentialTiles.RemoveAll(x => x != randomlyChosenTile); //< Removes every entry but randomlyChosenTile from potentialTiles list
            Instantiate(randomlyChosenTile.prefab, this.transform, false);
        }
        else
        {
            Debug.LogError($"Tile {this.name} does not have any potential tiles left.");
            return false;
        }
        RemoveNodePositionVisualizer();    //< Only remove visualizer if collapsing was successful
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
            Debug.Log($"PotentialTiles of {this} have not been changed by the limiter.");
            return false;
        }
        else
        {
            Debug.Log($"Reduced potential tiles of {this}:\nfrom {string.Join(", ", potentialTiles)} \nto      {string.Join(", ", reducedPotentialTiles)}", this);
            potentialTiles = reducedPotentialTiles;
            return true;
        }
    }

    //# Private Methods 
    private void RegisterInManager()
    {
        NodeManager.instance.RegisterNode(gridPosition, this);
        // Debug.Log($"Registering {name} at position {gridPosition}.");
    }

    private void UnregisterFromManager()
    {
        NodeManager.instance.UnregisterNode(gridPosition);
    }

    private void CreateNodePositionVisualizer()
    {
        gameObject.AddComponent<SuperpositionVisualizer>();
    }

    private void RemoveNodePositionVisualizer()
    {
        gameObject.GetComponent<SuperpositionVisualizer>().Remove();
    }
}
