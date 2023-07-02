//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Socket { h1, h1_2, h2, h2_3, h3, h3_4, h4, p2, p2_3, p3 }

[CreateAssetMenu(fileName = "Tile", menuName = "Wave Function Collapse/Tile")]
public class Tile : ScriptableObject
{
    public GameObject prefab;
    public bool isPath;
    public List<Socket> northSockets;
    public List<Socket> eastSockets;
    public List<Socket> southSockets;
    public List<Socket> westSockets;

    [Header("In-Editor Socket Bulk-Setup"), Space(10)]
    [SerializeField] private List<Socket> addToAllSides;
    [SerializeField] private List<Socket> removeFromAllSides;

    //# Public Methods 
    public List<Socket> GetSocketsOnSide(Vector2Int tileSide)
    {
        switch (tileSide)
        {
            case Vector2Int v when v.Equals(Vector2Int.up):
                return northSockets;
            case Vector2Int v when v.Equals(Vector2Int.right):
                return eastSockets;
            case Vector2Int v when v.Equals(Vector2Int.down):
                return southSockets;
            case Vector2Int v when v.Equals(Vector2Int.left):
                return westSockets;
            default:
                Debug.LogError($"{tileSide} is not registered as a valid side.");
                return null;
        }
    }

    //# Private Methods 
    public void ApplyBulkChanges()
    {
        if (addToAllSides.Count != 0)
        {
            TryAddSocketsToList(addToAllSides, northSockets);
            TryAddSocketsToList(addToAllSides, eastSockets);
            TryAddSocketsToList(addToAllSides, southSockets);
            TryAddSocketsToList(addToAllSides, westSockets);
        }
        addToAllSides.Clear();

        if (removeFromAllSides.Count != 0)
        {
            TryRemoveSocketsFromList(removeFromAllSides, northSockets);
            TryRemoveSocketsFromList(removeFromAllSides, eastSockets);
            TryRemoveSocketsFromList(removeFromAllSides, southSockets);
            TryRemoveSocketsFromList(removeFromAllSides, westSockets);
        }
        removeFromAllSides.Clear();
    }

    /// <summary>
    /// Tries to add tile to list, returning false if the list already contains the tile.
    /// </summary>
    private bool TryAddSocketsToList(List<Socket> sockets, List<Socket> list)
    {
        bool success = true;
        foreach (Socket socket in sockets)
        {
            if (list.Contains(socket) && success == true)   //< If at least one socket cannot be added, return false
                success = false;

            list.Add(socket);
        }
        return success;   //< This can only be reached if sockets is empty, which it can't be right now, as the list length is checked before the call of TryRemoveTileFromList().
    }

    /// <summary>
    /// Tries to remove tile from list, returning false if the list does not contain the tile.
    /// </summary>
    private bool TryRemoveSocketsFromList(List<Socket> sockets, List<Socket> list)
    {
        bool success = true;
        foreach (Socket socket in sockets)
            if (list.Remove(socket) && success == true)  //< If at least one socket cannot be added, return false
                success = false;

        return success;   //< This can only be reached if sockets is empty, which it can't be right now, as the list length is checked before the call of TryRemoveTileFromList().
    }
}
