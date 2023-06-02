//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public bool RESOLVE = false; //! DEBUG
    //# Public Variables 
    public static NodeManager instance { get; set; }
    public Dictionary<Vector2Int, Node> nodeGrid { get; private set; }
    public List<Tile> allTiles;

    //# Private Variables 
    [SerializeField]
    private Vector2Int nodeGridSize = new Vector2Int(8, 8);  //< Number of tiles in x/z axis
    private readonly Vector2 tileExtends = new Vector2(3, 3);    //< in Meters
    private readonly float tileSpacerThickness = 0.0f;

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
    }

    private void Update()   //! DEBUG
    {
        if (RESOLVE)
        {
            ResolveNodes();
            RESOLVE = false;
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

    public void RemoveNodeFromGrid(Vector2Int position)
    {
        nodeGrid.Remove(position);
    }

    //# Private Methods 
    private void GenerateNodeGrid()
    {
        GameObject nodeGridGO = new GameObject(name: "Node Grid");


        nodeGrid = new Dictionary<Vector2Int, Node>();

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
        foreach (KeyValuePair<Vector2Int, Node> pair in nodeGrid)
        {
            Node node = pair.Value;
            node.Resolve();
            new WaitForSeconds(1);
        }
    }
}