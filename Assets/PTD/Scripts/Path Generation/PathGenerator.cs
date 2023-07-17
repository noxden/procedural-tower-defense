//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau, Daniel Heilmann
//========================================================================

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[DefaultExecutionOrder(1)]
public class PathGenerator : MonoBehaviour
{
    [SerializeField] private WaveManagerScriptableObject waveManager;

    //# Debug "Button" Variables 
    private Vector2Int gridSize = Vector2Int.zero;
    private Vector2Int startPositionIndex;
    private Vector2Int endPositionIndex;
    private int pathLength;
    [SerializeField] private float stepDelayInSeconds;

    private PathNode[,] gridBackup;
    private Dictionary<Vector2Int, Node> currentGrid;
    [SerializeField] private List<Node> path = new List<Node>();
    private List<Node> currentPath;
    public static UnityEvent onPathGenerated { get; } = new UnityEvent();

    private void Start()
    {
        startPositionIndex = GenerationHandler.instance.startPositionIndex;
        endPositionIndex = GenerationHandler.instance.endPositionIndex;
        pathLength = GenerationHandler.instance.pathLength;

        gridSize = GenerationHandler.instance.gridSize;
        currentGrid = NodeManager.instance.nodeGrid;
    }

    #region Public Methods

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
    
    #endregion
    
    #region Private Methods

    private void CloneGrid()
    {
        gridBackup = new PathNode[gridSize.x, gridSize.y];
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                Vector2Int currentGridPos = new Vector2Int(x, y);
                currentGrid.TryGetValue(currentGridPos, out Node originalNode);
                PathNode clonedNode = CreatePathNodeFromNode(originalNode);
                if (originalNode != null) clonedNode.visited = originalNode.isPath;
                gridBackup[x, y] = clonedNode;
            }
        }
    }

    private IEnumerator GeneratePath(bool generateInstantly)
    {
        CloneGrid();

        currentPath = new List<Node>();
        currentGrid.TryGetValue(startPositionIndex, out Node currentNode);

        if (currentNode != null)
        {
            currentNode.isPath = true;
            currentPath.Add(currentNode);

            while (currentPath.Count != pathLength || currentPath[^1].gridPosition != endPositionIndex)
            {
                if (currentNode.possiblePathDirections.Count == 0)
                {
                    if (currentPath.Count == 1)
                    {
                        Debug.Log("No path found");
                        yield break; //< Unfortunately, we could not find a way to automatically restart the path generator when it fails.
                    }

                    //> If the current possible directions are 0, but the algorithm hasn't backtracked back to the start, go back one more node 
                    //  in the path and then check the possible path directions again.
                    currentNode.isPath = false;
                    currentNode.possiblePathDirections = gridBackup[currentNode.gridPosition.x, currentNode.gridPosition.y].possibleDirections;
                    currentPath.Remove(currentNode);

                    currentNode = currentPath[^1];
                }
                else
                {
                    Vector2Int direction = currentNode.possiblePathDirections[Random.Range(0, currentNode.possiblePathDirections.Count)];
                    currentNode.possiblePathDirections.Remove(direction);

                    currentGrid.TryGetValue(new Vector2Int((currentNode.gridPosition.x + direction.x), (currentNode.gridPosition.y + direction.y)), out Node nextNode);
                    if (nextNode != null)
                    {
                        nextNode.possiblePathDirections.Remove(-direction);

                        if (!CanVisitNode(nextNode))
                            continue;

                        currentNode.isPath = true;
                        currentPath.Add(nextNode);
                        currentNode = nextNode;
                    }
                }

                if (!generateInstantly)
                    yield return new WaitForSeconds(stepDelayInSeconds);
            }
        }

        yield return null;

        currentPath[^1].isPath = true; //! Quick fix for issue where the endnode would not have its isPath variable set accordingly.
        Debug.Log($"Path is generated!");
        path = currentPath;

        UpdateAllNodesBasedOnPathValue();
        OverwritePotentialTilesOfStartAndEndNodes();
        UpdateNodesInPathBasedOnPathDirection();
        waveManager.SetNavigationPath(path);
        onPathGenerated.Invoke();
    }

    private static void UpdateAllNodesBasedOnPathValue()
    {
        Dictionary<Vector2Int, Node> nodeGrid = NodeManager.instance.nodeGrid;
        foreach (Node node in nodeGrid.Select(keyValuePair => keyValuePair.Value))
            node.ReducePotentialTilesByPathFlag();
    }

    private void OverwritePotentialTilesOfStartAndEndNodes()
    {
        path[0].potentialTiles = NodeManager.startTiles;
        path[^1].potentialTiles = NodeManager.endTiles;
    }

    private void UpdateNodesInPathBasedOnPathDirection()
    {
        int currentIndex = 0;
        foreach (Node node in path) //< Implemented this as for loop before, but the foreach loop is much more readable.
        {
            if (currentIndex > 0) //< For every node but the one at path index 0 (so the first path node), do the following...
            {
                Vector2Int direction = path[currentIndex - 1].gridPosition - node.gridPosition;
                node.pathDirection.Add(direction);
            }

            int lastIndex = path.Count - 1; //< To make the if clause below more readable
            if (currentIndex < lastIndex) //< For every node but the one at the last path index (so the last path node), do the following...
            {
                Vector2Int direction = path[currentIndex + 1].gridPosition - node.gridPosition;
                node.pathDirection.Add(direction);
            }

            node.ReducePotentialTilesByPathDirection();
            currentIndex++;
        }
    }

    private bool CanVisitNode(Node nextNode) => !HasVisitedNode(nextNode) && CanReachEnd(nextNode);

    private static bool HasVisitedNode(Node nextNode) => nextNode.isPath;

    private bool CanReachEnd(Node nextNode)
    {
        int shortestDistance = (Mathf.Abs(endPositionIndex.x - nextNode.gridPosition.x) + Mathf.Abs(endPositionIndex.y - nextNode.gridPosition.y));
        int pathLengthLeft = pathLength - (currentPath.Count + 1);

        return shortestDistance <= pathLengthLeft;
    }

    private static PathNode CreatePathNodeFromNode(Node node) => new(node.gridPosition, node.possiblePathDirections);

    #endregion

    #region Editor

    private void OnDrawGizmos()
    {
        Vector3 gizmoScale = new Vector3(1f, 0.5f, 1f);
        const float gizmoHeight = 0f;
        if (currentGrid == null)
            return;

        foreach (Node node in currentGrid.Select(dictionaryEntry => dictionaryEntry.Value))
        {
            Gizmos.color = node.isPath ? Color.red : Color.gray;
            Vector3 position = node.gameObject.transform.position;
            Gizmos.DrawCube(new Vector3(position.x, gizmoHeight, position.z), gizmoScale);
        }

        if (currentPath == null) return;
        for (int i = 0; i < currentPath.Count; i++)
        {
            if (currentPath[i] == null)
                return;
            float greyScale = (float)i / currentPath.Count;
            Gizmos.color = new Color(0, greyScale, 0, 1f);
            Gizmos.DrawCube(new Vector3(currentPath[i].gameObject.transform.position.x, gizmoHeight, currentPath[i].gameObject.transform.position.z), gizmoScale);
        }
    }

    #endregion
}