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
    private Vector2Int gridSize = Vector2Int.zero;
    private Vector2Int startPositionIndex;
    private Vector2Int endPositionIndex;
    private int pathLength;
    [SerializeField] private float stepDelayInSeconds = 0f;

    private PathNode[,] gridBackup;
    private Dictionary<Vector2Int, Node> currentGrid;
    [SerializeField] private List<Node> path = new List<Node>();
    private List<Node> currentPath;
    public UnityEvent OnPathGenerated { get; } = new UnityEvent();

    private void Start()
    {
        startPositionIndex = GenerationHandler.instance.startPositionIndex;
        endPositionIndex = GenerationHandler.instance.endPositionIndex;
        pathLength = GenerationHandler.instance.pathLength;

        gridSize = GenerationHandler.instance.gridSize;
        currentGrid = NodeManager.instance.nodeGrid;
    }

    [ContextMenu("Reinitialize")]
    public void Reinitialize()
    {
        path = new List<Node>();
        currentPath = null;
        Start();
    }

    public void Generate(bool generateInstantly = true)
    {
        StopAllCoroutines();
        StartCoroutine(GeneratePath(generateInstantly));
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

    private IEnumerator GeneratePath(bool generateInstantly)
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
            if (!generateInstantly)
                yield return new WaitForSeconds(stepDelayInSeconds);
        }
        yield return null;

        currentPath[currentPath.Count - 1].isPath = true;   //! Quick fix for issue where the endnode would not have its isPath variable set accordingly.
        // Debug.Log($"Path generated from {(currentGrid.TryGetValue(startPositionIndex, out Node startNode) ? "" : "")}{startNode.name} to {(currentGrid.TryGetValue(endPositionIndex, out Node endNode) ? "" : "")}{endNode.name}.");
        Debug.Log($"Path is generated!");
        path = currentPath;

        UpdateAllNodesBasedOnPathValue();

        UpdateNodesInPathBasedOnPathDirection();

        OnPathGenerated.Invoke();
    }

    private void UpdateAllNodesBasedOnPathValue()
    {
        Dictionary<Vector2Int, Node> nodeGrid = NodeManager.instance.nodeGrid;
        foreach (var keyValuePair in nodeGrid)
        {
            Node node = keyValuePair.Value;
            node.ReducePotentialTilesByPathFlag();
        }
    }

    private void UpdateNodesInPathBasedOnPathDirection()
    {
        int pathIndex = 0;
        foreach (Node node in path)     //< Did this as for loop before, but the foreach loop is much more readable.
        {
            if (pathIndex > 0)
            {
                Vector2Int direction = path[pathIndex - 1].gridPosition - node.gridPosition;
                node.pathDirection.Add(direction);
            }
            if (pathIndex < path.Count - 1)     //< Because max index is always List.Count-1, so when pathIndex is at path.Count-2, the "path[pathIndex + 1]" below will get the node at path.Count-1, which is the final node.
            {
                Vector2Int direction = path[pathIndex + 1].gridPosition - node.gridPosition;
                node.pathDirection.Add(direction);
            }
            node.ReducePotentialTilesByPathDirection();

            pathIndex++;
        }
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

    private PathNode NodeToPathNode(Node node)
    {
        return new PathNode(node.gridPosition, node.possiblePathDirections);
    }

    #region Editor
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
