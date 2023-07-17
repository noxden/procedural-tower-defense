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
    [SerializeField] private float stepDelayInSeconds;

    private bool isCollapsed => uncollapsedNodes.Count == 0;
    private readonly List<Vector2Int> directionsToPropagateTo = new List<Vector2Int> { Vector2Int.up, Vector2Int.right, Vector2Int.down, Vector2Int.left };

    [Tooltip("For visualization purposes only."), SerializeField]
    private List<Node> uncollapsedNodes = new List<Node>();
    
    private void Start() => Reinitialize();

    #region Public Methods

    [ContextMenu("Reinitialize")]
    public void Reinitialize()
    {
        StopAllCoroutines();
        uncollapsedNodes.Clear();
        Initialize();
    }
    
    public void StartSolve(bool instantly)
    {
        StopAllCoroutines();
        StartCoroutine(Solve(solveInstantly: instantly));
    }

    #endregion

    #region Private Methods

    private void Initialize()
    {
        Dictionary<Vector2Int, Node> nodeDictionary = NodeManager.instance.nodeGrid;
        foreach (KeyValuePair<Vector2Int, Node> entry in nodeDictionary)
            uncollapsedNodes.Add(entry.Value);

        stepDelayInSeconds = 1f / (GenerationHandler.instance.gridSize.x * GenerationHandler.instance.gridSize.y);
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

    public void Iterate() //< Only public to allow GenerationHandlerEditor access (for custom inspector button)
    {
        Node node = GetNodeWithLowestEntropy();
        CollapseNode(node);
        Propagate(node);
    }

    // TODO: Improve the implementation of this method to fix the issue stated in the comment below. 
    private void CollapseNode(Node node)
    {
        if (!node.Collapse())
            Debug.LogError($"Could not collapse {node.name} properly!");

        //> Needs to be called even if node could not be collapsed, otherwise the while-loop in Solve() will go on indefinitely, causing the game to freeze.
        uncollapsedNodes.Remove(node);
    }

    // TODO: Currently, there is a lot of recursion present in the propagation. This can be improved.
    private void Propagate(Node sourceNode)
    {
        //> Set up propagation stack 
        List<Node> nodesToPropagateFrom = new List<Node> { sourceNode };
        while (nodesToPropagateFrom.Count > 0)
        {
            Node nodeToPropagateFrom = nodesToPropagateFrom[0];

            foreach (Vector2Int direction in directionsToPropagateTo)
            {
                Node nodeToPropagateTo = NodeManager.instance.GetNodeByPosition(nodeToPropagateFrom.gridPosition + direction); //< Gets node in given direction
                //> Check if there even is a node in this direction, so that it only generates the socketHashSet if there is a node to propagate it to
                if (nodeToPropagateTo == null) continue;

                //> Generate a list of all sockets on the side of "nodeToPropagateFrom" in the direction of "direction"
                HashSet<Socket> allSocketsOnSide = new HashSet<Socket>();
                foreach (Tile tile in nodeToPropagateFrom.potentialTiles)
                {
                    List<Socket> socketsOnSide = tile.GetSocketsOnSide(direction);
                    foreach (Socket socket in socketsOnSide)
                        allSocketsOnSide.Add(socket);
                }
                // Debug.Log($"All valid tiles in direction {direction} of {nodeToPropagateFrom.name} are: {string.Join(", ", allValidTilesInDirection)}");

                //> Apply the list generated above as a limiting factor (removing any tiles that don't match the generated socket list)
                if (nodeToPropagateTo.ReducePotentialTilesBySocketCompatibility(allSocketsOnSide, -direction)) //< If this reduction operation actually removed entries...
                    nodesToPropagateFrom.Add(nodeToPropagateTo); // ... add that node to the nodes to propagate from
            }

            nodesToPropagateFrom.Remove(nodeToPropagateFrom); //< After finishing the propagation from this node, remove it from the list and move on to the next entry.
        }
    }

    private Node GetNodeWithLowestEntropy()
    {
        switch (uncollapsedNodes.Count)
        {
            case > 1:
            {
                Node nodeWithLowestEntropy = uncollapsedNodes[(Random.Range(0, uncollapsedNodes.Count))];
                //< Randomization prevents system from always solving the grid from Node (0,0).
                foreach (Node nodeToCompare in uncollapsedNodes)
                {
                    if (nodeToCompare.entropy < nodeWithLowestEntropy.entropy)
                        nodeWithLowestEntropy = nodeToCompare;
                }

                return nodeWithLowestEntropy;
            }
            case 1: //< No need to run the comparisons if there is only one node left to be collapsed
                return uncollapsedNodes[0];
            default:
                return null; //< Should never occur, as system only iterates for as long as there are entries in uncollapsedNodes
        }
    }

    #endregion
}