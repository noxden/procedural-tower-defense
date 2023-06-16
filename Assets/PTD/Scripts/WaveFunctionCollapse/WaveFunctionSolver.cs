//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaveFunctionSolver : MonoBehaviour
{
    //# Debug "Button" Variables 
    [Header("Debug Section")]
    [SerializeField] private bool SOLVE = false;    //! FOR DEBUG PURPOSES ONLY
    [SerializeField] private bool SOLVE_STEPWISE = false;    //! FOR DEBUG PURPOSES ONLY
    [SerializeField] private float stepwiseSolvingSpeedInSeconds = 1f;    //! FOR DEBUG PURPOSES ONLY
    [SerializeField] private bool ITERATE = false;    //! FOR DEBUG PURPOSES ONLY

    //# Private Variables 
    [Header("List Visualization Section")]
    [Tooltip("For visualization purposes only.")]
    [SerializeField] private List<Vector2Int> directionsToPropagateTo = new List<Vector2Int>();
    [Tooltip("For visualization purposes only.")]
    [SerializeField] private List<Node> uncollapsedNodes = new List<Node>();
    private bool isCollapsed
    {
        get
        {
            return (uncollapsedNodes.Count == 0);
        }
    }

    //# Monobehaviour Events 
    private void Start()
    {
        FillDirectionsToPropagateTo();
        Initialize();
    }

    private void Update()
    {
        if (SOLVE)  //! FOR DEBUG PURPOSES ONLY
        {
            StopAllCoroutines();
            SOLVE = false;
            StartCoroutine(Solve(solveInstantly: true));
        }
        if (SOLVE_STEPWISE)  //! FOR DEBUG PURPOSES ONLY
        {
            StopAllCoroutines();
            SOLVE_STEPWISE = false;
            StartCoroutine(Solve(solveInstantly: false));
        }
        if (ITERATE)  //! FOR DEBUG PURPOSES ONLY
        {
            ITERATE = false;
            Iterate();
        }
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
        float waitTimeBetweenIterations = (stepwiseSolvingSpeedInSeconds == 0 ? float.MinValue : stepwiseSolvingSpeedInSeconds) / uncollapsedNodes.Count;   //< Takes a snapshot of the "uncollapsedNodes.Count" at the time of starting the coroutine.
        while (!isCollapsed)
        {
            Iterate();
            if (!solveInstantly)
                yield return new WaitForSeconds(waitTimeBetweenIterations);
        }

        Debug.Log($"Wave Function is collapsed!");
    }

    private void Iterate()
    {
        Node node = GetNodeWithLowestEntropy();
        Debug.Log($"Iterating over {node.name}.");
        CollapseNode(node);
        Propagate(node);
    }

    private void CollapseNode(Node node)
    {
        if (!node.Collapse())
            Debug.LogError($"Could not collapse {node.name} properly!");
        uncollapsedNodes.Remove(node);  //< Needs to be called even if node could not be collapsed, otherwise the while-loop in Solve() will go on indefinitely, causing the game to freeze.
        // TODO: Fix the issue stated above. 
    }

    // TODO: Currently, there is a lot of recursion present in the propagation. This can be improved.
    private void Propagate(Node sourceNode)
    {
        //#> Set up propagation stack 
        List<Node> nodesToPropagateFrom = new List<Node>();
        nodesToPropagateFrom.Add(sourceNode);

        while (nodesToPropagateFrom.Count > 0)
        {
            Node nodeToPropagateFrom = nodesToPropagateFrom[0];
            Debug.Log($"Now propagating from {nodeToPropagateFrom}.");

            foreach (Vector2Int direction in directionsToPropagateTo)
            {
                //#> Generate lists of valid tiles for the desired direction 
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
                // Debug.Log($"Generated list of valid tiles of {nodeToPropagateFrom} for direction {direction}: {string.Join(", ", allValidTilesInDirection)}");

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

    //> Replace sorting implementation with a simple loop that takes the node with the lowest entropy.
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

    private void FillDirectionsToPropagateTo()
    {
        directionsToPropagateTo.Add(Vector2Int.up);
        // directionsToPropagateTo.Add(Vector2Int.up + Vector2Int.right);
        directionsToPropagateTo.Add(Vector2Int.right);
        // directionsToPropagateTo.Add(Vector2Int.right + Vector2Int.down);
        directionsToPropagateTo.Add(Vector2Int.down);
        // directionsToPropagateTo.Add(Vector2Int.down + Vector2Int.left);
        directionsToPropagateTo.Add(Vector2Int.left);
        // directionsToPropagateTo.Add(Vector2Int.left + Vector2Int.up);
    }
}
