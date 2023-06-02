using System;
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

    [SerializeField] private float stepDelayInSeconds = 0;

    private Node[,] grid;
    private Node[,] currentGrid;
    private List<Node> path = new List<Node>();
    private List<Node> currentPath;

    //private List<GameObject> pathCubes = new List<GameObject>();

    private void Awake()
    {
        GenerateGrid();
        StartCoroutine(GeneratePath());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            StopAllCoroutines();
            StartCoroutine(GeneratePath());
        }
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

    private void CloneGrid()
    {
        currentGrid = new Node[gridSizeX, gridSizeY];
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {
                Node originalNode = grid[x, y];
                Node clonedNode = new Node(originalNode.posX, originalNode.posY, new List<Vector2>(originalNode.possibleDirections));
                clonedNode.visited = originalNode.visited;
                currentGrid[x, y] = clonedNode;
            }
        }
    }

    private IEnumerator GeneratePath()
    {
        CloneGrid();

        currentPath = new List<Node>();
        Node currentNode = currentGrid[(int)startPosX, (int)startPosY];

        currentNode.visited = true;
        currentPath.Add(currentNode);

        //CreateCube(currentNode);
        //Debug.Break();
        while (currentPath.Count != pathLength || currentPath[currentPath.Count -1].Position != new Vector2(endPosX, endPosY))
        {
            yield return new WaitForSeconds(stepDelayInSeconds);
            if(currentNode.possibleDirections.Count == 0)
            {
                if (currentPath.Count == 1)
                {
                    Debug.Log("No path found");
                    yield break;
                }

                currentNode.visited = false;
                currentNode.possibleDirections = grid[currentNode.posX, currentNode.posY].possibleDirections;
                currentPath.Remove(currentNode);

                currentNode = currentPath[currentPath.Count - 1];
            }
            else
            {
                Vector2 direction = currentNode.possibleDirections[UnityEngine.Random.Range(0, currentNode.possibleDirections.Count)];
                currentNode.possibleDirections.Remove(direction);

                Node nextNode = currentGrid[(int)(currentNode.posX + direction.x), (int)(currentNode.posY + direction.y)];
                nextNode.possibleDirections.Remove(-direction);

                if(!CanVisitNode(nextNode, currentPath))
                {
                    continue;
                }
                currentNode.visited = true;
                currentPath.Add(nextNode);
                currentNode = nextNode;
            }
        }

        Debug.Log("Path generated");
        path = currentPath;
    }

    private bool CanVisitNode(Node nextNode, List<Node> currentPath)
    {
        if (HasVisitedNode(nextNode))
            return false;

        if (!CanReachEnd(nextNode, currentPath))
            return false;

        return true;
    }

    private bool HasVisitedNode(Node nextNode)
    {
        return nextNode.visited;
        //foreach (Node node in currentPath)
        //{
        //    if (node.posX == nextNode.posX && node.posY == nextNode.posY)
        //        return true;
        //}
        //return false;
    }

    private bool CanReachEnd(Node nextNode, List<Node> currentPath)
    {
        int shortestDistance = (Mathf.Abs(endPosX - nextNode.posX) + Mathf.Abs(endPosY - nextNode.posY));
        int pathLengthleft = pathLength - (currentPath.Count + 1);

        if (shortestDistance <= pathLengthleft)
            return true;

        return false;
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

    private void OnDrawGizmos()
    {
        if (currentGrid == null)
            return;

        foreach (Node node in currentGrid)
        {
            Gizmos.color = node.visited ? Color.red : Color.gray;
            Gizmos.DrawCube(new Vector3(node.posX, 0f, node.posY), Vector3.one * 0.5f);
        }

        if (currentPath != null)
        {
            for (int i = 0; i < currentPath.Count; i++)
            {
                float greyScale = (float)i / currentPath.Count;
                Gizmos.color = new Color(0, greyScale, 0, 1f);
                Gizmos.DrawCube(new Vector3(currentPath[i].posX, 0f, currentPath[i].posY), Vector3.one * 0.5f);
            }
        }
    }
    #endregion
}
