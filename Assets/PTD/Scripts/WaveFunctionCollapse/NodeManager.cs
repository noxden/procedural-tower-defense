//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections.Generic;
using UnityEngine;

public class NodeManager : MonoBehaviour
{
    //# Public Variables 
    public static NodeManager instance { get; private set; }
    public Dictionary<Vector2Int, Node> nodeGrid { get; private set; }
    private static List<Tile> _allTiles;
    public static List<Tile> startTiles;
    public static List<Tile> endTiles;

    //# Private Variables 
    private Vector2Int nodeGridSize; //< Number of tiles in x/z axis
    public readonly Vector2 tileExtends = new Vector2(3, 3); //< in Meters
    public readonly float tileSpacerThickness = 0.0f; //< To add a gap between tiles
    private GameObject nodeGridGameObject;

    //# Monobehaviour Events 
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(this);

        FetchAllTilesFromResourcesFolder();
    }

    private void Start()
    {
        nodeGridSize = GenerationHandler.instance.gridSize;
        GenerateNodeGrid();
    }

    //# Public Methods 
    /// <summary>
    /// Tries to add node and returns false if it was not possible.
    /// </summary>
    public bool RegisterNode(Node node, Vector2Int position)
    {
        bool success = nodeGrid.TryAdd(position, node);
        if (!success) Debug.LogError($"NodeGrid already carries an entry for position \"{position}\". Adding new node \"{node.name}\" failed.", node);
        return success;
    }

    public void UnregisterNode(Node node) => nodeGrid.Remove(node.gridPosition);

    public Node GetNodeByPosition(Vector2Int position)
    {
        bool success = nodeGrid.TryGetValue(position, out Node node);
        // if (!success) Debug.Log($"Could not locate node at position {position}."); //< Intended behaviour whenever system would try to reach an out-of-bounds node
        return node;
    }

    [ContextMenu("Reset Grid")]
    public void ResetGrid()
    {
        DestroyImmediate(nodeGridGameObject);
        nodeGrid = null;

        nodeGridSize = GenerationHandler.instance.gridSize;
        GenerateNodeGrid();

        GetComponent<WaveFunctionSolver>().Reinitialize();
        GetComponent<PathGenerator>().Reinitialize();
    }

    //# Private Methods 
    private void GenerateNodeGrid()
    {
        nodeGrid = new Dictionary<Vector2Int, Node>();
        nodeGridGameObject = new GameObject(name: "Node Grid");

        for (int x = 0; x < nodeGridSize.x; x++)
        {
            for (int y = 0; y < nodeGridSize.y; y++)
            {
                GameObject nodeGO = new GameObject();
                nodeGO.transform.SetParent(nodeGridGameObject.transform);
                nodeGO.transform.localPosition = new Vector3(x * (tileExtends.x + tileSpacerThickness), 0, y * (tileExtends.y + tileSpacerThickness));
                Node newNode = nodeGO.AddComponent<Node>();

                newNode.potentialTiles = new List<Tile>(_allTiles); //< Fill this node's potential tiles
                newNode.gridPosition = new Vector2Int(x, y);
            }
        }
    }

    private void FetchAllTilesFromResourcesFolder()
    {
        TileDefinition[] allTileDefinitions = Resources.LoadAll<TileDefinition>("Tiles");
        if (allTileDefinitions.Length == 0) //< Skip constructing tiles if no tile definitions could be located.
            return;

        _allTiles = new List<Tile>();
        startTiles = new List<Tile>();
        endTiles = new List<Tile>();
        foreach (TileDefinition definition in allTileDefinitions)
        {
            List<Tile> listToAddTo;
            switch (definition.optionalTileTag) //< Decide which list to add to
            {
                case TileTag.StartTile:
                    listToAddTo = startTiles;
                    break;
                case TileTag.EndTile:
                    listToAddTo = endTiles;
                    break;
                default:
                    listToAddTo = _allTiles;
                    break;
            }

            listToAddTo.Add(new Tile(definition)); //< Add Tile constructed from tileDefinition

            if (definition.generateRotatedVariants) //< Only generate rotated variants if generateRotatedVariants is true
            {
                listToAddTo.Add(new Tile(TileDefinition.CreateRotatedVariant(definition, 1), 1)); //< Add tile variant that is rotated by 90 degrees

                if (!definition.isMirrorable) //< Only generate rotated variants for 180 and 270 degrees if tile is not mirrorable,
                {
                    //  as otherwise those would just be duplicates of 0 and 90 degree variants respectively.
                    for (int i = 2; i <= 3; i++)
                        listToAddTo.Add(new Tile(TileDefinition.CreateRotatedVariant(definition, i), i));
                }
            }
        }
    }
}