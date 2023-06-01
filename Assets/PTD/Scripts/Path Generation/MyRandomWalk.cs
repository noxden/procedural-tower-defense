using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class MyRandomWalk : MonoBehaviour
{
    [SerializeField] private int gridSizeX;
    [SerializeField] private int gridSizeY;
    [SerializeField] private int pathLength;
    [Header("Start Position")]
    [SerializeField] private int startPosX;
    [SerializeField] private int startPosY;
    [Header("End Position")]
    [SerializeField] private int endPosX;
    [SerializeField] private int endPosY;

    [SerializeField] private int stepDelayInSeconds = 0;

    private Node[,] grid;
    private List<Node> path = new List<Node>();

    private List<GameObject> pathCubes = new List<GameObject>();

    private void Awake()
    {
        GenerateGrid();
        StartCoroutine(GeneratePath());
    }

    private void GenerateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                List<Vector2> possibleDirections = new List<Vector2>();
                if(x != 0)
                    possibleDirections.Add(new Vector2(-1f, 0f));
                if(x != gridSizeX - 1)
                    possibleDirections.Add(new Vector2(1f, 0f));
                if(y != 0)
                    possibleDirections.Add(new Vector2(0f, -1f));
                if (y != gridSizeY - 1)
                    possibleDirections.Add(new Vector2(0f, 1f));

                grid[x, y] = new Node(x, y, new List<Vector2>(possibleDirections));
            }
        }
    }

    private IEnumerator GeneratePath()
    {
        Node[,] currentGrid = grid;
        List<Node> currentPath = new List<Node>();
        Node currentNode = currentGrid[(int)startPosX, (int)startPosY];

        currentPath.Add(currentNode);
        CreateCube(currentNode);
        Debug.Break();
        while (currentPath.Count != pathLength && currentPath[currentPath.Count -1].Position != new Vector2(endPosX, endPosY))
        {
            yield return new WaitForSeconds(stepDelayInSeconds);
            if(currentNode.possibleDirections.Count == 0)
            {
                if (currentPath.Count == 1)
                {
                    Debug.Log("No path found");
                    yield break;
                }

                currentPath.Remove(currentNode);
                pathCubes.Remove(pathCubes[pathCubes.Count - 1]);
                currentGrid[currentNode.posX, currentNode.posY] = grid[currentNode.posX, currentNode.posY];
                currentNode = currentPath[currentPath.Count - 1];
            }
            else
            {
                Vector2 direction = currentNode.possibleDirections[Random.Range(0, currentNode.possibleDirections.Count)];
                currentNode.possibleDirections.Remove(direction);

                Node nextNode = currentGrid[(int)(currentNode.posX + direction.x), (int)(currentNode.posY + direction.y)];
                nextNode.possibleDirections.Remove(-direction);

                if(CanVisitNode(nextNode, currentPath))
                {
                    currentNode.possibleDirections.Remove(direction);
                    continue;
                }
                currentPath.Add(nextNode);
                CreateCube(nextNode);
                currentNode = nextNode;
            }
        }

        Debug.Log("Path generated");
        path = currentPath;
    }

    private void CreateCube(Node node)
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = new Vector3(node.posX, 0f, node.posY);
        pathCubes.Add(cube);
    }

    private bool CanVisitNode(Node nextNode, List<Node> currentPath)
    {
        if (HasVisitedNode(nextNode, currentPath))
            return false;

        if (!CanReachEnd(nextNode, currentPath))
            return false;

        return true;
    }

    private bool HasVisitedNode(Node nextNode, List<Node> currentPath)
    {
        foreach (Node node in currentPath)
        {
            if (node.posX == nextNode.posX && node.posY == nextNode.posY)
                return true;
        }
        return false;
    }

    private bool CanReachEnd(Node nextNode, List<Node> currentPath)
    {
        int shortestDistance = (Mathf.Abs(endPosX - nextNode.posX) + Mathf.Abs(endPosY - nextNode.posY));
        int pathLengthleft = pathLength - currentPath.Count;

        if (shortestDistance < pathLengthleft)
            return false;

        return true;
    }

    #region Editor

    private void OnValidate()
    {
        //Ensure positions are within the gridsize
        startPosX = Mathf.Clamp(startPosX, 0, gridSizeX - 1);
        startPosY = Mathf.Clamp(startPosY, 0, gridSizeY - 1);

        endPosX = Mathf.Clamp(endPosX, 0, gridSizeX - 1);
        endPosY = Mathf.Clamp(endPosY, 0, gridSizeY - 1);
    }

    #endregion
}
