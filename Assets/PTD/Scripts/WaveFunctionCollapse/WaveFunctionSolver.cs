//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(2)]
public class WaveFunctionSolver : MonoBehaviour
{
    //# Private Variables 
    [SerializeField] private float stepDelayInSeconds = 0f;
    private bool isCollapsed { get => uncollapsedNodes.Count == 0; }
    private List<Vector2Int> directionsToPropagateTo = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    // [Header("Visualization Section"), Space(5)]
    [Tooltip("For visualization purposes only."), SerializeField]
    private List<Node> uncollapsedNodes = new List<Node>();

    //# Monobehaviour Events 
    private void Start()
    {
        Initialize();
    }

    public void SolveInstantly()
    {
        StopAllCoroutines();
        StartCoroutine(Solve(solveInstantly: true));
    }

    public void SolveStepwise()
    {
        StopAllCoroutines();
        StartCoroutine(Solve(solveInstantly: false));
    }

    public void Restart()
    {
        StopAllCoroutines();
        directionsToPropagateTo.Clear();
        uncollapsedNodes.Clear();
        Start();
    }

    //# Private Methods 
    private void Initialize()
    {
        Dictionary<Vector2Int, Node> nodeDictionary = NodeManager.instance.nodeGrid;
        foreach (var entry in nodeDictionary)
            uncollapsedNodes.Add(entry.Value);
    }

    private IEnumerator Solve(bool solveInstantly)
    {
        while (!isCollapsed)
        {
            Iterate();
            if (!solveInstantly)
                yield return new WaitForSeconds(stepDelayInSeconds);
        }

        Debug.Log($"Wave Function is collapsed!");
    }

    private void Iterate()
    {
        Node node = GetNodeWithLowestEntropy();
        CollapseNode(node);
        Propagate(node);
    }

    private void CollapseNode(Node node)
    {
        if (!node.Collapse())
            Debug.LogError($"Could not collapse {node.name} properly!");
        uncollapsedNodes.Remove(node);  //< Needs to be called even if node could not be collapsed, otherwise the while-loop in Solve() will go on indefinitely, causing the game to freeze.
        // TODO: Improve the implementation of this method to fix the issue stated above. 
    }

    // TODO: Currently, there is a lot of recursion present in the propagation. This can be improved.
    private void Propagate(Node sourceNode)
    {
        //> Set up propagation stack 
        List<Node> nodesToPropagateFrom = new List<Node>();
        nodesToPropagateFrom.Add(sourceNode);

        while (nodesToPropagateFrom.Count > 0)
        {
            Node nodeToPropagateFrom = nodesToPropagateFrom[0];

            foreach (Vector2Int direction in directionsToPropagateTo)
            {
                //> Generate lists of valid tiles for the desired direction 
                List<Tile> allValidTilesInDirection = new List<Tile>();
                foreach (Tile tile in nodeToPropagateFrom.potentialTiles)
                {
                    List<Tile> validTilesInDirection = tile.GetValidTilesInDirection(direction);
                    foreach (Tile validTile in validTilesInDirection)
                    {
                        if (!allValidTilesInDirection.Contains(validTile))
                            allValidTilesInDirection.Add(validTile);
                    }
                }
                // Debug.Log($"All valid tiles in direction {direction} of {nodeToPropagateFrom.name} are: {string.Join(", ", allValidTilesInDirection)}");

                //#> Check for node in that direction and apply the list generated above as a limiting factor 
                Node nodeToPropagateTo = NodeManager.instance.GetNodeByPosition(nodeToPropagateFrom.gridPosition + direction);  //< Gets node in given direction
                if (nodeToPropagateTo != null)  //TODO: Null-Check could be moved above validTiles list generation (at least partially), in order to not do that generation for nothing.
                {
                    if (nodeToPropagateTo.ReducePotentialTilesByLimiter(allValidTilesInDirection))
                        nodesToPropagateFrom.Add(nodeToPropagateTo);
                }
            }

            nodesToPropagateFrom.Remove(nodeToPropagateFrom);
        }
    }

    private Node GetNodeWithLowestEntropy()
    {
        if (uncollapsedNodes.Count > 1)
        {
            Node nodeWithLowestEntropy = uncollapsedNodes[(Random.Range(0, uncollapsedNodes.Count))];   //< Randomization prevents system from always solving the grid from Node (0,0).
            foreach (Node nodeToCompare in uncollapsedNodes)
            {
                if (nodeToCompare.entropy < nodeWithLowestEntropy.entropy)
                    nodeWithLowestEntropy = nodeToCompare;
            }
            return nodeWithLowestEntropy;
        }
        else if (uncollapsedNodes.Count == 1)
            return uncollapsedNodes[0];
        else
            return null;
    }
}
