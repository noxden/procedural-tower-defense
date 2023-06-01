using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public int posX;
    public int posY;

    public List<Vector2> possibleDirections = new List<Vector2>
    {
            new Vector2(0f, 1f),
            new Vector2(0f, -1f),
            new Vector2(-1f, 0f),
            new Vector2(1f, 0f)
            };

    public Node(int posX, int posY, List<Vector2> possibleDirections)
    {
        this.posX = posX;
        this.posY = posY;
        this.possibleDirections = possibleDirections;
    }

    public Vector2 Position { get { return new Vector2(posX, posY); } }

}
