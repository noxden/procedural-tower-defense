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
    [SerializeField] private bool SOLVE = false;    //! FOR DEBUG PURPOSES ONLY

    //# Private Variables 
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
        Initialize();
    }

    private void Update()
    {
        if (SOLVE)  //! FOR DEBUG PURPOSES ONLY
        {
            SOLVE = false;
            Solve();
        }
    }

    //# Private Methods 
    private void Initialize()
    {
        Dictionary<Vector2Int, Node> nodeDictionary = NodeManager.instance.nodeGrid;
        foreach (var entry in nodeDictionary)
            uncollapsedNodes.Add(entry.Value);
    }

    private void Solve()
    {
        while (!isCollapsed)
            Iterate();

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

    private void Propagate(Node sourceNode)
    {
        //#> Set up propagation stack 
        List<Node> nodesToPropagateFrom = new List<Node>();
        nodesToPropagateFrom.Add(sourceNode);

        while (nodesToPropagateFrom.Count > 0)
        {
            Node nodeToPropagateFrom = nodesToPropagateFrom[0];
            Debug.Log($"Now propagating from {nodeToPropagateFrom}.");

            //#> Generate lists of valid tiles for the desired directions 
            Vector2Int direction = new Vector2Int(1, 0);    //< This now propagates from the sourceNode to nodes in the direction set here.

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
            Debug.Log($"Generated list of valid tiles NORTH of {nodeToPropagateFrom}: {string.Join(", ", allValidTilesInDirection)}");

            //#> Check for node in that direction and apply the list generated above as a limiting factor 
            Node nodeToPropagateTo = NodeManager.instance.GetNodeByPosition(nodeToPropagateFrom.gridPosition + direction);  //< Gets node in given direction
            if (nodeToPropagateTo != null)  //TODO: Null-Check could be moved above validTiles list generation (at least partially), in order to not do that generation for nothing.
            {
                if (nodeToPropagateTo.ReducePotentialTilesByLimiter(allValidTilesInDirection))
                    nodesToPropagateFrom.Add(nodeToPropagateTo);
            }

            nodesToPropagateFrom.Remove(nodeToPropagateFrom);
        }
    }

    private Node GetNodeWithLowestEntropy()
    {
        if (uncollapsedNodes.Count > 1)
        {
            List<Node> uncollapsedNodesSortedByEntropy = uncollapsedNodes.OrderBy(n => n.entropy).ToList();
            if (uncollapsedNodesSortedByEntropy[0].entropy == uncollapsedNodesSortedByEntropy[1].entropy)
            {
                List<Node> nodesWithLowestEntropy = new List<Node>(uncollapsedNodesSortedByEntropy.FindAll(n => n.entropy == uncollapsedNodesSortedByEntropy[0].entropy));
                Node randomlyChosenNode = nodesWithLowestEntropy[Random.Range(0, nodesWithLowestEntropy.Count)];
                Debug.Log($"Choosing randomly between nodes with an entropy of {uncollapsedNodesSortedByEntropy[0].entropy}...");
                return randomlyChosenNode;
            }
            return uncollapsedNodesSortedByEntropy[0];
        }
        else if (uncollapsedNodes.Count == 1)
            return uncollapsedNodes[0];
        else
            return null;
    }
}
