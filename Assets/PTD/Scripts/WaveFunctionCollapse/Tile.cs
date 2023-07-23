//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Tile
{
    #region Variables and Properties

    public string name;
    public TileTag tag;
    public GameObject prefab;
    public List<Socket> northSockets;
    public List<Socket> eastSockets;
    public List<Socket> southSockets;
    public List<Socket> westSockets;
    public int rotationVariantIdentifier;
    public bool isPath; //< Is set automatically based on result of GeneratePathDirection()
    private List<Vector2Int> mPathDirection;
    public List<Vector2Int> pathDirection
    {
        get
        {
            if (mPathDirection == null)
                GeneratePathDirection();

            return mPathDirection;
        }
    }

    private GameObject instantiatedPrefab;

    #endregion

    #region Constructors

    /// <summary> Construct Tile from TileDefinition. </summary>
    public Tile(TileDefinition definition, int rotationIterations = 0)
    {
        this.tag = definition.optionalTileTag;
        this.prefab = definition.prefab;
        this.northSockets = new List<Socket>(definition.northSockets);
        this.eastSockets = new List<Socket>(definition.eastSockets);
        this.southSockets = new List<Socket>(definition.southSockets);
        this.westSockets = new List<Socket>(definition.westSockets);
        this.rotationVariantIdentifier = rotationIterations;
        this.name = $"{definition.name}{(rotationIterations == 0 ? "" : $"_r{rotationVariantIdentifier}")}";
        GeneratePathDirection();
    }

    /// <summary> Construct Tile from scratch. </summary>
    public Tile(GameObject prefab, IEnumerable<Socket> northSockets, IEnumerable<Socket> eastSockets, IEnumerable<Socket> southSockets, IEnumerable<Socket> westSockets,
        TileTag tileTag = TileTag.None, int amountRotatedClockwise = 0)
    {
        this.tag = tileTag;
        this.prefab = prefab;
        this.northSockets = new List<Socket>(northSockets);
        this.eastSockets = new List<Socket>(eastSockets);
        this.southSockets = new List<Socket>(southSockets);
        this.westSockets = new List<Socket>(westSockets);
        this.rotationVariantIdentifier = amountRotatedClockwise;
        this.name = $"Tile({prefab.name}){(amountRotatedClockwise == 0 ? "" : $"_r{amountRotatedClockwise}")}";
        GeneratePathDirection();
    }

    #endregion

    public void InstantiatePrefab(Transform parentTransform)
    {
        instantiatedPrefab = Object.Instantiate(prefab, parentTransform, false);
        if (rotationVariantIdentifier != 0)
            instantiatedPrefab.transform.Rotate(0f, 90f * rotationVariantIdentifier, 0f, Space.Self);
    }

    public List<Socket> GetSocketsOnSide(Vector2Int tileSide)
    {
        switch (tileSide)
        {
            case var v when v.Equals(Vector2Int.up): //< This is a really cool way to make switch cases accept Vector shorthands.
                return northSockets;
            case var v when v.Equals(Vector2Int.right):
                return eastSockets;
            case var v when v.Equals(Vector2Int.down):
                return southSockets;
            case var v when v.Equals(Vector2Int.left):
                return westSockets;
            default:
                Debug.LogError($"{tileSide} is not registered as a valid side.");
                return null;
        }
    }

    //# Private Methods 
    private void GeneratePathDirection() //< Sets pathDirection list automatically based on where this tile has path sockets.
    {
        mPathDirection = new List<Vector2Int>();
        if (northSockets.Contains(Socket.p2) || northSockets.Contains(Socket.p2_3) || northSockets.Contains(Socket.p3))
            mPathDirection.Add(Vector2Int.up);
        if (eastSockets.Contains(Socket.p2) || eastSockets.Contains(Socket.p2_3) || eastSockets.Contains(Socket.p3))
            mPathDirection.Add(Vector2Int.right);
        if (southSockets.Contains(Socket.p2) || southSockets.Contains(Socket.p2_3) || southSockets.Contains(Socket.p3))
            mPathDirection.Add(Vector2Int.down);
        if (westSockets.Contains(Socket.p2) || westSockets.Contains(Socket.p2_3) || westSockets.Contains(Socket.p3))
            mPathDirection.Add(Vector2Int.left);
        // Debug.Log($"[Setup] Generated path directions of {this.name}.");

        isPath = (mPathDirection.Count > 0);
    }
}