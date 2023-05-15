using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public int numberOfPotentialTilesLeft { get { return potentialTiles.Count; } }
    public Vector2Int gridPosition
    {
        get
        {
            return _gridPosition.Value;
        }
        set
        {
            if (_gridPosition.HasValue)
                UnregisterFromManager(_gridPosition.Value);
            _gridPosition = value;
            RegisterInManager();
        }
    }
    private Vector2Int? _gridPosition;
    private List<GameObject> potentialTiles;

    private void Start()
    {
        potentialTiles = NodeManager.instance.allPotentialTiles;
    }

    public void PlaceTile()
    {
        Instantiate(potentialTiles[Random.Range(0, potentialTiles.Count)], this.transform, false);
    }

    private void RegisterInManager()
    {
        NodeManager.instance.AddNodeToGrid(gridPosition, this);
    }

    private void UnregisterFromManager(Vector2Int position)
    {
        NodeManager.instance.RemoveNodeFromGrid(gridPosition);
    }
}
