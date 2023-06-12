//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: There has to be a way to make it more performance-friendly / take up less memory
[ExecuteInEditMode]
[CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse/Tile")]
public class Tile : ScriptableObject
{
    public GameObject prefab;
    public List<Tile> validTilesN;
    public List<Tile> validTilesNE;
    public List<Tile> validTilesE;
    public List<Tile> validTilesSE;
    public List<Tile> validTilesS;
    public List<Tile> validTilesSW;
    public List<Tile> validTilesW;
    public List<Tile> validTilesNW;

    private Vector2Int North = new Vector2Int(0, 1);

    [Header("In-Editor Tile Setup")]
    [SerializeField]
    private Tile addToAllDirections;
    [SerializeField]
    private Tile removeFromAllDirections;

    //# Public Methods 
    public List<Tile> GetValidTilesInDirection(Vector2Int direction)
    {
        switch (direction)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                Debug.Log($"Return validTilesN");
                return validTilesN;
            case Vector2Int v when v.Equals(Vector2Int.up + Vector2Int.right):
                Debug.Log($"Return validTilesNE");
                return validTilesNE;
            case Vector2Int v when v.Equals(Vector2Int.right):
                Debug.Log($"Return validTilesE");
                return validTilesE;
            case Vector2Int v when v.Equals(Vector2Int.right + Vector2Int.down):
                Debug.Log($"Return validTilesSE");
                return validTilesSE;
            case Vector2Int v when v.Equals(Vector2Int.down):
                Debug.Log($"Return validTilesS");
                return validTilesS;
            case Vector2Int v when v.Equals(Vector2Int.down + Vector2Int.left):
                Debug.Log($"Return validTilesSW");
                return validTilesSW;
            case Vector2Int v when v.Equals(Vector2Int.left):
                Debug.Log($"Return validTilesW");
                return validTilesW;
            case Vector2Int v when v.Equals(Vector2Int.left + Vector2Int.up):
                Debug.Log($"Return validTilesNW");
                return validTilesNW;
        }
        Debug.Log($"R{direction} is not registered as a valid direction.");
        return null;
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
            TryAddTileToList(addToAllDirections, validTilesNE);
            TryAddTileToList(addToAllDirections, validTilesE);
            TryAddTileToList(addToAllDirections, validTilesSE);
            TryAddTileToList(addToAllDirections, validTilesS);
            TryAddTileToList(addToAllDirections, validTilesSW);
            TryAddTileToList(addToAllDirections, validTilesW);
            TryAddTileToList(addToAllDirections, validTilesNW);
        }
        addToAllDirections = null;

        if (removeFromAllDirections != null)
        {
            TryRemoveTileFromList(removeFromAllDirections, validTilesN);
            TryRemoveTileFromList(removeFromAllDirections, validTilesNE);
            TryRemoveTileFromList(removeFromAllDirections, validTilesE);
            TryRemoveTileFromList(removeFromAllDirections, validTilesSE);
            TryRemoveTileFromList(removeFromAllDirections, validTilesS);
            TryRemoveTileFromList(removeFromAllDirections, validTilesSW);
            TryRemoveTileFromList(removeFromAllDirections, validTilesW);
            TryRemoveTileFromList(removeFromAllDirections, validTilesNW);
        }
        removeFromAllDirections = null;
    }

    /// <summary>
    /// Tries to add tile to list, returning false if the list already contains the tile.
    /// </summary>
    private bool TryAddTileToList(Tile tile, List<Tile> list)
    {
        if (list.Contains(tile))
            return false;

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
