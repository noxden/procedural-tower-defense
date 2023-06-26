//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau, Daniel Heilmann
//========================================================================

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
public class PathGenerator : MonoBehaviour
{
    //# Debug "Button" Variables 
    [SerializeField] private bool GENERATE = false;    //! FOR DEBUG PURPOSES ONLY
    private Vector2Int gridSize = Vector2Int.zero;
    [SerializeField] private Vector2Int startPositionIndex;
    [SerializeField] private Vector2Int endPositionIndex;
    [SerializeField] private int pathLength;

    [SerializeField] private float stepDelayInSeconds = 0f;

    private PathNode[,] gridBackup;
    private Dictionary<Vector2Int, Node> currentGrid;
    private List<Node> path = new List<Node>();
    [SerializeField] private List<Node> currentPath;
    public UnityEvent OnPathGenerated { get; } = new UnityEvent();

    private void Start()
    {
        gridSize = NodeManager.instance.nodeGridSize;
        currentGrid = NodeManager.instance.nodeGrid;
        ValidateSettings();
    }

    private void Update()
    {
        if (GENERATE)
        {
            Generate();
            GENERATE = false;
        }
    }

    public void Generate()
    {
        StopAllCoroutines();
        StartCoroutine(GeneratePath());
    }

    private void CloneGrid()
    {
        gridBackup = new PathNode[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int currentGridPos = new Vector2Int(x, y);
                Node originalNode = null;
                currentGrid.TryGetValue(currentGridPos, out originalNode);
                PathNode clonedNode = NodeToPathNode(originalNode);
                clonedNode.visited = originalNode.isPath;
                gridBackup[x, y] = clonedNode;
            }
        }
    }

    private IEnumerator GeneratePath()
    {
        CloneGrid();

        currentPath = new List<Node>();
        Node currentNode = null;
        currentGrid.TryGetValue(startPositionIndex, out currentNode);

        currentNode.isPath = true;
        currentPath.Add(currentNode);

        while (currentPath.Count != pathLength || currentPath[currentPath.Count - 1].gridPosition != endPositionIndex)
        {
            if (currentNode.possiblePathDirections.Count == 0)
            {
                if (currentPath.Count == 1)
                {
                    Debug.Log("No path found");
                    yield break;
                }

                currentNode.isPath = false;
                currentNode.possiblePathDirections = gridBackup[currentNode.gridPosition.x, currentNode.gridPosition.y].possibleDirections;
                currentPath.Remove(currentNode);

                currentNode = currentPath[currentPath.Count - 1];
            }
            else
            {
                Vector2Int direction = currentNode.possiblePathDirections[UnityEngine.Random.Range(0, currentNode.possiblePathDirections.Count)];
                currentNode.possiblePathDirections.Remove(direction);

                Node nextNode = null;
                currentGrid.TryGetValue(new Vector2Int((currentNode.gridPosition.x + direction.x), (currentNode.gridPosition.y + direction.y)), out nextNode);
                nextNode.possiblePathDirections.Remove(-direction);

                if (!CanVisitNode(nextNode, currentPath))
                {
                    continue;
                }
                currentNode.isPath = true;
                currentPath.Add(nextNode);
                currentNode = nextNode;
            }
            yield return new WaitForSeconds(stepDelayInSeconds);
        }

        currentPath[currentPath.Count - 1].isPath = true;   //! Quick fix for issue where the endnode would not have its isPath variable set accordingly.
        // Debug.Log($"Path generated from {(currentGrid.TryGetValue(startPositionIndex, out Node startNode) ? "" : "")}{startNode.name} to {(currentGrid.TryGetValue(endPositionIndex, out Node endNode) ? "" : "")}{endNode.name}.");
        Debug.Log($"Path is generated!");
        path = currentPath;
        OnPathGenerated.Invoke();

        // TODO: Now, each node should reduce its potentialTiles based on their isPath value. Thanks to the path list here, we already know which nodes to notify!
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
        return nextNode.isPath;
    }

    private bool CanReachEnd(Node nextNode, List<Node> currentPath)
    {
        int shortestDistance = (Mathf.Abs(endPositionIndex.x - nextNode.gridPosition.x) + Mathf.Abs(endPositionIndex.y - nextNode.gridPosition.y));
        int pathLengthleft = pathLength - (currentPath.Count + 1);

        if (shortestDistance <= pathLengthleft)
            return true;

        return false;
    }


    private void ValidateSettings()
    {
        //> Ensure positions are within the gridsize
        startPositionIndex.x = Mathf.Clamp(startPositionIndex.x, 0, gridSize.x - 1);
        startPositionIndex.y = Mathf.Clamp(startPositionIndex.y, 0, gridSize.y - 1);

        endPositionIndex.x = Mathf.Clamp(endPositionIndex.x, 0, gridSize.x - 1);
        endPositionIndex.y = Mathf.Clamp(endPositionIndex.y, 0, gridSize.y - 1);

        int shortestDistance = (Mathf.Abs(endPositionIndex.x - startPositionIndex.x) + Mathf.Abs(endPositionIndex.y - startPositionIndex.y)) + 1;

        if (pathLength % 2 != shortestDistance % 2)
        {
            pathLength++;
            Debug.LogWarning("Path cannot end, setting it to an " + (shortestDistance % 2 == 0 ? "even" : "uneven") + " number");
        }

        if (pathLength < shortestDistance)
        {
            pathLength = shortestDistance;
            Debug.LogWarning("Path length is too short, setting to shortest distance");
        }

        if (pathLength > gridSize.x * gridSize.y)
        {
            pathLength = gridSize.x * gridSize.y;
            Debug.LogWarning("Path length is too long, setting to max length");
        }
    }

    private PathNode NodeToPathNode(Node node)
    {
        return new PathNode(node.gridPosition, node.possiblePathDirections);
    }

    #region Editor

    private void OnValidate()
    {
        if (!(gridSize.x == 0 & gridSize.y == 0))
            ValidateSettings();

    }

    private void OnDrawGizmos()
    {
        Vector3 gizmoScale = new Vector3(1f, 0.5f, 1f);
        float gizmoHeight = 4.5f;
        if (currentGrid == null)
            return;

        foreach (var dictionaryEntry in currentGrid)
        {
            Node node = dictionaryEntry.Value;
            Gizmos.color = node.isPath ? Color.red : Color.gray;
            Gizmos.DrawCube(new Vector3(node.gameObject.transform.position.x, gizmoHeight, node.gameObject.transform.position.z), gizmoScale);
        }

        if (currentPath != null)
        {
            for (int i = 0; i < currentPath.Count; i++)
            {
                float greyScale = (float)i / currentPath.Count;
                Gizmos.color = new Color(0, greyScale, 0, 1f);
                Gizmos.DrawCube(new Vector3(currentPath[i].gameObject.transform.position.x, gizmoHeight, currentPath[i].gameObject.transform.position.z), gizmoScale);
            }
        }
    }
    #endregion
}
