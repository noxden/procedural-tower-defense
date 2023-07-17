//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================
//   Additional Notes:
// Currently does not visualize node superpositions. Instead, it only 
// serves to visualize the position of nodes, as there just were way too
// many total tiles to display all of them on every node.
// All the commented-out code is the deprecated remains of the previous 
// implementation which only displayed the superposition of the 4 
// different height-tiles.
//========================================================================

using UnityEngine;

public class SuperpositionVisualizer : MonoBehaviour
{
    //# Private Variables 
    private static GameObject _prefab;

    private Transform tileVisualizerGroup;
    //private List<GameObject> tileVisualizations = new List<GameObject>();

    //# Monobehaviour Methods 
    private void Start()
    {
        if (_prefab == null)
            _prefab = Resources.Load<GameObject>("VisualizerPrefab");

        tileVisualizerGroup = new GameObject("Superposition Visualizer").transform;
        tileVisualizerGroup.transform.SetParent(this.transform, false);

        Initialize();

        // OnPotentialTilesUpdatedInNode(GetComponent<Node>().potentialTiles);
    }

    // private void OnEnable() => GetComponent<Node>().OnPotentialTilesUpdated.AddListener(OnPotentialTilesUpdatedInNode);
    // private void OnDisable() => GetComponent<Node>().OnPotentialTilesUpdated.RemoveListener(OnPotentialTilesUpdatedInNode);

    //# Public Methods 
    // public void OnPotentialTilesUpdatedInNode(List<Tile> updatedTileList)
    // {
    //     for (int i = 0; i < NodeManager.instance.allTiles.Count; i++)
    //     {
    //         if (!updatedTileList.Contains(NodeManager.instance.allTiles[i]))
    //             tileVisualizations[i].SetActive(false);
    //     }
    // }

    public void Remove()
    {
        Destroy(tileVisualizerGroup.gameObject);
        Destroy(this);
    }

    //# Private Methods 
    private void Initialize()
    {
        //! Just a temporary band-aid solution, not a permanent fix!
        Instantiate(_prefab, tileVisualizerGroup, false);

        //> Visualize each of the four height-tiles
        // List<Tile> allTiles = NodeManager.instance.allTiles;
        // foreach (var tile in allTiles)
        // {
        //     GameObject tileVisualization = Instantiate(tile.prefab, parent: tileVisualizerGroup, worldPositionStays: false);
        //     tileVisualizations.Add(tileVisualization);

        //     if (tile == allTiles[0])
        //         tileVisualization.transform.localPosition = new Vector3(0.6f, 0, -0.6f);
        //     else if (tile == allTiles[1])
        //         tileVisualization.transform.localPosition = new Vector3(-0.6f, 0, -0.6f);
        //     else if (tile == allTiles[2])
        //         tileVisualization.transform.localPosition = new Vector3(0.6f, 0, 0.6f);
        //     else if (tile == allTiles[3])
        //         tileVisualization.transform.localPosition = new Vector3(-0.6f, 0, 0.6f);
        //     else
        //     {
        //         Debug.LogError($"Cannot find position for a fifth tileVisualization.");
        //         Destroy(tileVisualization);
        //         return;
        //     }

        //     tileVisualization.transform.localScale = new Vector3(0.35f, 0.3f, 0.35f);
        // }
    }
}