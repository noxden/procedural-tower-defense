//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Direction { N, NE, E, SE, S, SW, W, NW }

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
    private Vector2Int? _gridPosition;
    [SerializeField]
    private List<Tile> potentialTiles;  //< Defines this node's superposition
    private GameObject debugVisualizer;

    private void Start()
    {
        potentialTiles = new List<Tile>(NodeManager.instance.allTiles);
        CreateNodePositionVisualizer();
    }

    public void Resolve()
    {
        RemoveNodePositionVisualizer();
        if (potentialTiles.Count == 1)
            Instantiate(potentialTiles[0].prefab, this.transform, false);
        else if (potentialTiles.Count > 1)
        {
            Tile randomlyChosenTile = potentialTiles[Random.Range(0, potentialTiles.Count)];
            potentialTiles.RemoveAll(x => x != randomlyChosenTile); //< Removes every entry but randomlyChosenTile from potentialTiles list
            Instantiate(randomlyChosenTile.prefab, this.transform, false);
        }
        else
            Debug.LogError($"Tile {this.name} does not have any potential tiles left.");
    }

    /// <summary>
    /// This function is called when the MonoBehaviour will be destroyed.
    /// </summary>
    private void OnDestroy()
    {
        UnregisterFromManager();
    }

    private void RegisterInManager()
    {
        NodeManager.instance.RegisterNode(gridPosition, this);
    }

    private void UnregisterFromManager()
    {
        NodeManager.instance.UnregisterNode(gridPosition);
    }

    private void CreateNodePositionVisualizer()
    {
        debugVisualizer = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        debugVisualizer.transform.SetParent(this.transform, false);
        debugVisualizer.transform.localScale = Vector3.one * 0.75f;
    }

    private void RemoveNodePositionVisualizer()
    {
        Destroy(debugVisualizer);
    }
}
