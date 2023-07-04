using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Tile
{
    public string name;
    public GameObject prefab;
    public List<Socket> northSockets;
    public List<Socket> eastSockets;
    public List<Socket> southSockets;
    public List<Socket> westSockets;
    public int rotationVariantIdentifier;
    public bool isPath;    //< Is set based on result of GeneratePathDirection()
    public List<Vector2Int> pathDirection
    {
        get
        {
            if (m_pathDirection == null)
                GeneratePathDirection();

            return m_pathDirection;
        }
    }
    private List<Vector2Int> m_pathDirection;
    private GameObject instantiatedPrefab;

    /// <summary>
    /// Create Tile from TileDefinition.
    /// </summary>
    public Tile(TileDefinition definition, int rotationIterations = 0)
    {
        this.prefab = definition.prefab;
        this.northSockets = new List<Socket>(definition.northSockets);
        this.eastSockets = new List<Socket>(definition.eastSockets);
        this.southSockets = new List<Socket>(definition.southSockets);
        this.westSockets = new List<Socket>(definition.westSockets);
        this.rotationVariantIdentifier = rotationIterations;
        this.name = $"{definition.name}{(rotationIterations == 0 ? "" : $"_r{rotationVariantIdentifier}")}";
        GeneratePathDirection();
    }

    /// <summary>
    /// Create Tile from scratch.
    /// </summary>
    public Tile(GameObject prefab, List<Socket> northSockets, List<Socket> eastSockets, List<Socket> southSockets, List<Socket> westSockets, int amountRotatedClockwise = 0)
    {
        this.prefab = prefab;
        this.northSockets = new List<Socket>(northSockets);
        this.eastSockets = new List<Socket>(eastSockets);
        this.southSockets = new List<Socket>(southSockets);
        this.westSockets = new List<Socket>(westSockets);
        this.rotationVariantIdentifier = amountRotatedClockwise;
        this.name = $"Tile({prefab.name}){(amountRotatedClockwise == 0 ? "" : $"_r{amountRotatedClockwise}")}";
        GeneratePathDirection();
    }

    public virtual void InstantiatePrefab(Transform parentTransform)
    {
        instantiatedPrefab = GameObject.Instantiate(prefab, parentTransform, false);
        if (rotationVariantIdentifier != 0)
            instantiatedPrefab.transform.Rotate(0f, 90f * rotationVariantIdentifier, 0f, Space.Self);
    }

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
    private void GeneratePathDirection()
    {
        m_pathDirection = new List<Vector2Int>();
        if (northSockets.Contains(Socket.p2) || northSockets.Contains(Socket.p2_3) || northSockets.Contains(Socket.p3))
            m_pathDirection.Add(Vector2Int.up);
        if (eastSockets.Contains(Socket.p2) || eastSockets.Contains(Socket.p2_3) || eastSockets.Contains(Socket.p3))
            m_pathDirection.Add(Vector2Int.right);
        if (southSockets.Contains(Socket.p2) || southSockets.Contains(Socket.p2_3) || southSockets.Contains(Socket.p3))
            m_pathDirection.Add(Vector2Int.down);
        if (westSockets.Contains(Socket.p2) || westSockets.Contains(Socket.p2_3) || westSockets.Contains(Socket.p3))
            m_pathDirection.Add(Vector2Int.left);
        // Debug.Log($"[Setup] Generated path directions of {this.name}.");

        isPath = (m_pathDirection.Count > 0);
    }
}
