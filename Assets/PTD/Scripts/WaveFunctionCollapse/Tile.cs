//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse/Tile")]
public class Tile : ScriptableObject
{
    public GameObject prefab;
    public List<Tile> validTilesN;
    public List<Tile> validTilesE;
    public List<Tile> validTilesS;
    public List<Tile> validTilesW;
    public bool isPath;

    [Header("In-Editor Tile Setup")]
    [SerializeField]
    private Tile addToAllDirections;
    [SerializeField]
    private Tile removeFromAllDirections;
    [SerializeField]
    private bool forceAddMode;

    //# Public Methods 
    public List<Tile> GetValidTilesInDirection(Vector2Int direction)
    {
        switch (direction)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                return validTilesN;
            case Vector2Int v when v.Equals(Vector2Int.right):
                return validTilesE;
            case Vector2Int v when v.Equals(Vector2Int.down):
                return validTilesS;
            case Vector2Int v when v.Equals(Vector2Int.left):
                return validTilesW;
            default:
                Debug.LogError($"{direction} is not registered as a valid direction.");
                return null;
        }
    }

    //# Private Methods 
    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        if (addToAllDirections != null)
        {
            TryAddTileToList(addToAllDirections, validTilesN);
            TryAddTileToList(addToAllDirections, validTilesE);
            TryAddTileToList(addToAllDirections, validTilesS);
            TryAddTileToList(addToAllDirections, validTilesW);
        }
        addToAllDirections = null;

        if (removeFromAllDirections != null)
        {
            TryRemoveTileFromList(removeFromAllDirections, validTilesN);
            TryRemoveTileFromList(removeFromAllDirections, validTilesE);
            TryRemoveTileFromList(removeFromAllDirections, validTilesS);
            TryRemoveTileFromList(removeFromAllDirections, validTilesW);
        }
        removeFromAllDirections = null;
    }

    /// <summary>
    /// Tries to add tile to list, returning false if the list already contains the tile.
    /// </summary>
    private bool TryAddTileToList(Tile tile, List<Tile> list)
    {
        if (!forceAddMode)
        {
            if (list.Contains(tile))
                return false;
        }

        list.Add(tile);
        return true;
    }

    /// <summary>
    /// Tries to remove tile from list, returning false if the list does not contain the tile.
    /// </summary>
    private bool TryRemoveTileFromList(Tile tile, List<Tile> list)
    {
        return list.Remove(tile);
    }
}
