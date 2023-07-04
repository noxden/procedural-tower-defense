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

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "TileDefinition", menuName = "Wave Function Collapse/TileDefinition")]
public class TileDefinition : ScriptableObject
{
    [Header("Tile Variant Generation Settings")]
    public bool generateRotatedVariants;
    public bool isMirrorable;

    [Header("Tile Configuration")]
    public GameObject prefab;
    public List<Socket> northSockets;
    public List<Socket> eastSockets;
    public List<Socket> southSockets;
    public List<Socket> westSockets;

    [Header("In-Editor Socket Bulk-Setup"), Space(10)]
    [SerializeField] private List<Socket> addToAllSides;
    [SerializeField] private List<Socket> removeFromAllSides;

    public static TileDefinition CreateRotatedVariant(TileDefinition inputDefinition, int rotationIterations)
    {
        TileDefinition outputDefinition = ScriptableObject.CreateInstance<TileDefinition>();
        outputDefinition.name = inputDefinition.name;
        outputDefinition.prefab = inputDefinition.prefab;

        outputDefinition.northSockets = new List<Socket>(inputDefinition.northSockets);
        outputDefinition.eastSockets = new List<Socket>(inputDefinition.eastSockets);
        outputDefinition.southSockets = new List<Socket>(inputDefinition.southSockets);
        outputDefinition.westSockets = new List<Socket>(inputDefinition.westSockets);

        for (int i = 0; i < rotationIterations; i++)
        {
            List<Socket> buffer = new List<Socket>(outputDefinition.northSockets);
            outputDefinition.northSockets = new List<Socket>(outputDefinition.westSockets);
            outputDefinition.westSockets = new List<Socket>(outputDefinition.southSockets);
            outputDefinition.southSockets = new List<Socket>(outputDefinition.eastSockets);
            outputDefinition.eastSockets = new List<Socket>(buffer);
        }
        
        return outputDefinition;
    }

    //# Public Methods 
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
            if (list.Contains(socket))
                success = false;
            else
                list.Add(socket);
        }
        return success;
    }

    /// <summary>
    /// Tries to remove tile from list, returning false if the list does not contain the tile.
    /// </summary>
    private bool TryRemoveSocketsFromList(List<Socket> sockets, List<Socket> list)
    {
        bool success = false;
        foreach (Socket socket in sockets)
            if (list.Remove(socket))
                success = true;

        return success;
    }
}