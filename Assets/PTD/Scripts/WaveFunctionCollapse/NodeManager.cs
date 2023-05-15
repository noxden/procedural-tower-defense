using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    public bool RESOLVE = false; //! DEBUG
    //# Public Variables 
    public static NodeManager instance { get; set; }
    public Dictionary<Vector2Int, Node> nodeGrid { get; private set; }
    public List<GameObject> allPotentialTiles;

    //# Private Variables 
    [SerializeField]
    private Vector2Int nodeGridSize = new Vector2Int(8, 8);  //< Number of tiles in x/z axis
    private readonly Vector2 tileExtends = new Vector2(3, 3);    //< in Meters
    private readonly float tileSpacerThickness = 0.5f;

    //# Monobehaviour Events 
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }
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
    public bool AddNodeToGrid(Vector2Int position, Node node)
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
        nodeGrid = new Dictionary<Vector2Int, Node>();

        for (int x = 0; x < nodeGridSize.x; x++)  //< gridOffset could already be applied here, but it would probably just make the for-loop more calculation-heavy.
        {
            for (int y = 0; y < nodeGridSize.y; y++)
            {
                GameObject newGameObject = new GameObject();
                newGameObject.transform.position = new Vector3(x * (tileExtends.x + tileSpacerThickness), 0, y * (tileExtends.y + tileSpacerThickness));
                Node newNode = newGameObject.AddComponent<Node>();

                Vector2Int gridPosition = new Vector2Int(x, y);
                newNode.gridPosition = gridPosition;

                newGameObject.name = $"Node {gridPosition}";
            }
        }
    }

    private void ResolveNodes()
    {
        foreach (KeyValuePair<Vector2Int, Node> pair in nodeGrid)
        {
            Node node = pair.Value;
            node.PlaceTile();
            new WaitForSeconds(1);
        }
    }
}