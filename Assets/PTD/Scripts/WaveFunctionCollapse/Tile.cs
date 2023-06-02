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
[CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse Tile")]
public class Tile : ScriptableObject
{
    public GameObject prefab;

    public List<Tile> ValidTilesN;
    public List<Tile> ValidTilesNE;
    public List<Tile> ValidTilesE;
    public List<Tile> ValidTilesSE;
    public List<Tile> ValidTilesS;
    public List<Tile> ValidTilesSW;
    public List<Tile> ValidTilesW;
    public List<Tile> ValidTilesNW;

    [Header("In-Editor Tile Setup")]
    [SerializeField]
    private Tile addToAllDirections;
    [SerializeField]
    private Tile removeFromAllDirections;

    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        if (addToAllDirections != null)
        {
            TryAddTileToList(addToAllDirections, ValidTilesN);
            TryAddTileToList(addToAllDirections, ValidTilesNE);
            TryAddTileToList(addToAllDirections, ValidTilesE);
            TryAddTileToList(addToAllDirections, ValidTilesSE);
            TryAddTileToList(addToAllDirections, ValidTilesS);
            TryAddTileToList(addToAllDirections, ValidTilesSW);
            TryAddTileToList(addToAllDirections, ValidTilesW);
            TryAddTileToList(addToAllDirections, ValidTilesNW);
        }
        addToAllDirections = null;

        if (removeFromAllDirections != null)
        {
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesN);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesNE);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesE);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesSE);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesS);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesSW);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesW);
            TryRemoveTileFromList(removeFromAllDirections, ValidTilesNW);
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
