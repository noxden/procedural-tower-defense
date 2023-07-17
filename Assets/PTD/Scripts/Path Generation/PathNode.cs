using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    private readonly int posX;
    private readonly int posY;
    public bool visited = false;

    public readonly List<Vector2Int> possibleDirections;

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

    public Vector2 position => new(posX, posY);
}