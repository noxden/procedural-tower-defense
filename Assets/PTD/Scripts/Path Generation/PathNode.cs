using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    public int posX;
    public int posY;
    public bool visited = false;

    public List<Vector2Int> possibleDirections = new List<Vector2Int>
    {
            new Vector2Int(0, 1),
            new Vector2Int(0, -1),
            new Vector2Int(-1, 0),
            new Vector2Int(1, 0)
            };

    public PathNode(int posX, int posY, List<Vector2Int> possibleDirections)
    {
        this.posX = posX;
        this.posY = posY;
        this.possibleDirections = possibleDirections;
    }

    public PathNode(Vector2Int position, List<Vector2Int> possibleDirections)
    {
        this.posX = position.x;
        this.posY = position.y;
        this.possibleDirections = possibleDirections;
    }

    public Vector2 Position { get { return new Vector2(posX, posY); } }

}
