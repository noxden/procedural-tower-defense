//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections.Generic;
using UnityEngine;

public enum Socket
{
    h1,
    h1_2,
    h2,
    h2_3,
    h3,
    h3_4,
    h4,
    p2,
    p2_3,
    p3
}

public enum TileTag
{
    None,
    StartTile,
    EndTile
}

[ExecuteInEditMode]
[CreateAssetMenu(fileName = "TileDefinition", menuName = "Wave Function Collapse/TileDefinition")]
public class TileDefinition : ScriptableObject
{
    #region Variables and Properties

    [Header("Tile Variant Generation Settings")]
    public bool generateRotatedVariants;

    public bool isMirrorable;

    [Header("Tile Configuration")] public GameObject prefab;
    public List<Socket> northSockets;
    public List<Socket> eastSockets;
    public List<Socket> southSockets;
    public List<Socket> westSockets;
    public TileTag optionalTileTag;

    [Header("In-Editor Socket Bulk-Setup"), Space(10)] 
    [SerializeField] private List<Socket> addToAllSides;
    [SerializeField] private List<Socket> removeFromAllSides;

    #endregion

    public static TileDefinition CreateRotatedVariant(TileDefinition inputDefinition, int rotationIterations)
    {
        TileDefinition outputDefinition = ScriptableObject.CreateInstance<TileDefinition>(); //< Creates a copy of the inputDefinition
        outputDefinition.name = inputDefinition.name;
        outputDefinition.prefab = inputDefinition.prefab;
        outputDefinition.optionalTileTag = inputDefinition.optionalTileTag;

        outputDefinition.northSockets = new List<Socket>(inputDefinition.northSockets);
        outputDefinition.eastSockets = new List<Socket>(inputDefinition.eastSockets);
        outputDefinition.southSockets = new List<Socket>(inputDefinition.southSockets);
        outputDefinition.westSockets = new List<Socket>(inputDefinition.westSockets);

        for (int i = 0; i < rotationIterations; i++) //< Rotates the sockets on the copy for "i" times. i = 2 represents a rotation of 180 degrees
        {
            List<Socket> buffer = new List<Socket>(outputDefinition.northSockets);
            outputDefinition.northSockets = new List<Socket>(outputDefinition.westSockets);
            outputDefinition.westSockets = new List<Socket>(outputDefinition.southSockets);
            outputDefinition.southSockets = new List<Socket>(outputDefinition.eastSockets);
            outputDefinition.eastSockets = new List<Socket>(buffer);
        }

        return outputDefinition;
    }

    #region Methods to allow bulk-editing TileDefinition Sockets in Editor

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

    /// <summary> Tries to add (multiple) sockets to socket list. </summary>
    /// <returns> False if the list already contains at least one of those sockets. </returns>
    private static bool TryAddSocketsToList(List<Socket> sockets, ICollection<Socket> list)
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

    /// <summary> Tries to remove (multiple) sockets from socket list. </summary>
    /// <returns> False if at least one of the sockets is not contained in the list, true if all tiles were removed successfully. </returns>
    private static bool TryRemoveSocketsFromList(IEnumerable<Socket> sockets, ICollection<Socket> list)
    {
        var success = true;
        foreach (Socket socket in sockets)
        {
            if (!list.Remove(socket))
                success = false;
        }

        return success;
    }

    #endregion
}