//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144), Jan Rau (769214)
//========================================================================

using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Node : MonoBehaviour
{
    #region Variables and Properties

    public int entropy => potentialTiles.Count; //< The entropy of a node, previously named "numberOfRemainingPotentialTiles"

    private Vector2Int? mGridPosition;

    public Vector2Int gridPosition
    {
        get => mGridPosition ?? default;  //< If mGridPosition != null, return it. Otherwise return default
        set
        {
            if (mGridPosition.HasValue)
                UnregisterFromManager(); //< To basically "move" this node on the grid from one position...
            mGridPosition = value;
            RegisterInManager();         // ...to another.
            name = $"Node {value}";
        }
    }

    [FormerlySerializedAs("m_potentialTiles")] [SerializeField]
    private List<Tile> mPotentialTiles; //< Defines this node's superposition

    public List<Tile> potentialTiles
    {
        get => mPotentialTiles;
        set
        {
            mPotentialTiles = value;
            onPotentialTilesUpdated?.Invoke(potentialTiles);
        }
    }

    private UnityEvent<List<Tile>> onPotentialTilesUpdated { get; } = new UnityEvent<List<Tile>>();

    //# Path Generation variables 
    public List<Vector2Int> possiblePathDirections = new List<Vector2Int>(); //< Used during path generation to limit random walk
    public bool isPath;
    public List<Vector2Int> pathDirection = new List<Vector2Int>(); //< Used after path generation to define which sides should have path sockets
    
    #endregion
    
    //# Monobehaviour Methods 
    private void Start()
    {
        CreateNodePositionVisualizer();
        InitializePossiblePathDirections();
    }

    private void OnDestroy()
    {
        RemoveNodePositionVisualizer();
        UnregisterFromManager();
    }

    #region Public Methods

    /// <summary> This function causes one of the potential tiles to be instantiated, thereby collapsing the superposition. </summary>
    /// <returns> Returns true if collapse was successful, false if entropy was 0. </returns>
    public bool Collapse()
    {
        if (entropy == 1)
            potentialTiles[0].InstantiatePrefab(this.transform);
        else if (entropy > 1)
        {
            Tile randomlyChosenTile = potentialTiles[Random.Range(0, potentialTiles.Count)];
            potentialTiles.RemoveAll(x => x != randomlyChosenTile); //< Removes every entry from potentialTiles list except for the randomlyChosenTile
            randomlyChosenTile.InstantiatePrefab(this.transform);
        }
        else
        {
            Debug.LogError($"Tile {this.name} does not have any potential tiles left.");
            return false;
        }

        RemoveNodePositionVisualizer(); //< Only remove visualizer if collapsing was successful
        return true;
    }

    /// <returns> Returns true if potentialTiles were reduced by limiter, false if not. </returns>
    public bool ReducePotentialTilesBySocketCompatibility(HashSet<Socket> compatibleSockets, Vector2Int socketSide)
    {
        List<Tile> reducedPotentialTiles = new List<Tile>(potentialTiles);
        foreach (Tile tile in potentialTiles)
        {
            bool isTileCompatible = false;

            List<Socket> socketsOnSide = tile.GetSocketsOnSide(socketSide);
            foreach (Socket socket in socketsOnSide)
            {
                if (compatibleSockets.Contains(socket)) //< If at least one of the tile's sockets matches the required sockets on that side, this tile is compatible.
                    isTileCompatible = true;
            }

            if (!isTileCompatible)
                reducedPotentialTiles.Remove(tile);
        }

        if (reducedPotentialTiles.SequenceEqual(potentialTiles))
            return false;
        else
        {
            potentialTiles = reducedPotentialTiles;
            return true;
        }
    }

    /// <summary> Filters out potential tiles that do not match the node's isPath value. </summary>
    public void ReducePotentialTilesByPathFlag()
    {
        List<Tile> reducedPotentialTiles = new List<Tile>(potentialTiles);
        foreach (Tile entry in potentialTiles)
        {
            if (entry.isPath != this.isPath)
                reducedPotentialTiles.Remove(entry);
        }
        potentialTiles = reducedPotentialTiles;
    }

    /// <summary> Filters out potential tiles that do not match the node's pathDirections. </summary>
    public void ReducePotentialTilesByPathDirection()
    {
        List<Tile> reducedPotentialTiles = new List<Tile>(potentialTiles);
        foreach (Tile tile in potentialTiles)
        {
            //> If any of the node's path directions is not present in the tile's pathDirection, remove the tile
            bool isTileCompatible = true;
            foreach (Vector2Int direction in pathDirection)
            {
                if (!tile.pathDirection.Contains(direction))
                    isTileCompatible = false;
            }

            if (!isTileCompatible)
                reducedPotentialTiles.Remove(tile);
        }
        potentialTiles = reducedPotentialTiles;
    }

    #endregion


    #region Private Methods

    private void RegisterInManager() => NodeManager.instance.RegisterNode(this, gridPosition); //< Nodes assign their spot in the nodeGrid themselves.
    private void UnregisterFromManager() => NodeManager.instance.UnregisterNode(this);

    private void CreateNodePositionVisualizer() => gameObject.AddComponent<SuperpositionVisualizer>();
    private void RemoveNodePositionVisualizer() => gameObject.GetComponent<SuperpositionVisualizer>()?.Remove();

    //> This method was originally part of Jan's path generation nodes. It sets the node's possible path 
    //  directions before running the path generator based on where it is in the nodeGrid.
    //  This way for example, "left" is not a valid direction if this node is on the left edge of the map.
    private void InitializePossiblePathDirections()
    {
        if (gridPosition.x != 0)
            possiblePathDirections.Add(Vector2Int.left);
        if (gridPosition.x != GenerationHandler.instance.gridSize.x - 1)
            possiblePathDirections.Add(Vector2Int.right);
        if (gridPosition.y != 0)
            possiblePathDirections.Add(Vector2Int.down);
        if (gridPosition.y != GenerationHandler.instance.gridSize.y - 1)
            possiblePathDirections.Add(Vector2Int.up);
    }

    #endregion

    //> This part was added by Jan to define points where navigation destinations would need to 
    //  be generated in order to make enemies path / walk straight from one corner to the next.
    //> Daniel then refactored it to take up less space and processing power.
    public bool IsCornerPiece()
    {
        if (pathDirection.Count == 2)
        {
            float directionSqrMagnitude = (pathDirection[0] + pathDirection[1]).sqrMagnitude; //< Calculation-based approach to the original implementation.
            return Math.Abs(directionSqrMagnitude - 2.0f) < 0.1f;
            //< If both directions pointed in opposite directions, sqrMagnitude would be 0.
            //  If both pointed in the same direction, it would be 4.

            #region Original (disabled) Implementation

            // if (pathDirection.Contains(new Vector2Int(1, 0)) && pathDirection.Contains(new Vector2Int(0, 1)))
            // {
            //     return true;
            // }
            // else if (pathDirection.Contains(new Vector2Int(-1, 0)) && pathDirection.Contains(new Vector2Int(0, 1)))
            // {
            //     return true;
            // }
            // else if (pathDirection.Contains(new Vector2Int(1, 0)) && pathDirection.Contains(new Vector2Int(0, -1)))
            // {
            //     return true;
            // }
            // else if (pathDirection.Contains(new Vector2Int(-1, 0)) && pathDirection.Contains(new Vector2Int(0, -1)))
            // {
            //     return true;
            // }
            // else
            // {
            //     return false;
            // }

            #endregion
        }
        return false;
    }
}