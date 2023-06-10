//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;

public class NodeManager : MonoBehaviour
{
    //# Debug "Button" Variables 
    [SerializeField] private bool RESOLVE = false; //! DEBUG
    [SerializeField] private bool REGENERATE = false; //! DEBUG
    //# Public Variables 
    public static NodeManager instance { get; set; }
    public Dictionary<Vector2Int, Node> nodeGrid { get; private set; }
    public List<Tile> allTiles; //< Would be static if that did not prevent adding the tiles in the editor

    //# Private Variables 
    [SerializeField]
    private Vector2Int nodeGridSize = new Vector2Int(8, 8);  //< Number of tiles in x/z axis
    private readonly Vector2 tileExtends = new Vector2(3, 3);    //< in Meters
    private readonly float tileSpacerThickness = 0.0f;
    [SerializeField]
    [Tooltip("For visualization only.")]
    private List<Node> unresolvedNodes;  //< Contains all unresolved nodes

    //# Monobehaviour Events 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    private void Start()
    {
        GenerateNodeGrid();

        foreach (KeyValuePair<Vector2Int, Node> pair in nodeGrid)
        {
            Node node = pair.Value;
            unresolvedNodes.Add(node);
        }
    }

    private void Update()   //! DEBUG
    {
        if (RESOLVE)
        {
            ResolveNodes();
            RESOLVE = false;
        }

        if (REGENERATE)
        {
            Destroy(GameObject.Find("Node Grid"));
            nodeGrid.Clear();
            unresolvedNodes.Clear();
            Start();
            REGENERATE = false;
        }
    }

    //# Public Methods 
    /// <summary>
    /// Tries to add node and returns false if it was not possible.
    /// </summary>
    public bool RegisterNodeToGrid(Vector2Int position, Node node)
    {
        bool success = nodeGrid.TryAdd(position, node);
        if (!success) Debug.Log($"NodeGrid already carries an entry for position \"{position}\". Adding new node \"{node.name}\" failed.");
        return success;
    }
    public void UnregisterNodeFromGrid(Vector2Int position) => nodeGrid.Remove(position);

    //# Private Methods 
    private void GenerateNodeGrid()
    {
        nodeGrid = new Dictionary<Vector2Int, Node>();
        GameObject nodeGridGO = new GameObject(name: "Node Grid");

        for (int x = 0; x < nodeGridSize.x; x++)  //< gridOffset could already be applied here, but it would probably just make the for-loop more calculation-heavy.
        {
            for (int y = 0; y < nodeGridSize.y; y++)
            {
                GameObject nodeGO = new GameObject();
                nodeGO.transform.SetParent(nodeGridGO.transform);
                nodeGO.transform.localPosition = new Vector3(x * (tileExtends.x + tileSpacerThickness), 0, y * (tileExtends.y + tileSpacerThickness));
                Node newNode = nodeGO.AddComponent<Node>();

                Vector2Int gridPosition = new Vector2Int(x, y);
                newNode.gridPosition = gridPosition;
                nodeGO.name = $"Node {gridPosition}";
            }
        }
    }

    private void ResolveNodes()
    {
        while (unresolvedNodes.Count > 0)
        {
            unresolvedNodes = unresolvedNodes.OrderBy(n => n.entropy).ToList();
            List<Node> nodesWithLowestEntropy = new List<Node>(unresolvedNodes.FindAll(n => n.entropy == unresolvedNodes[0].entropy));
            Debug.Log($"Resolving entries with an entropy of {unresolvedNodes[0].entropy} / {nodesWithLowestEntropy[0].entropy}");

            Node randomlyChosenNode = nodesWithLowestEntropy[Random.Range(0, nodesWithLowestEntropy.Count)];
            randomlyChosenNode.Resolve();
            unresolvedNodes.Remove(randomlyChosenNode);
        }
    }
}