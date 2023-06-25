//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Daniel Heilmann (771144)
//========================================================================

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SuperpositionVisualizer : MonoBehaviour
{
    //# Private Variables 
    private Transform tileVisualizerGroup;
    private List<GameObject> tileVisualizations = new List<GameObject>();

    //# Monobehaviour Events 
    private void Start()
    {
        tileVisualizerGroup = new GameObject("Superposition Visualizer").transform;
        tileVisualizerGroup.transform.SetParent(this.transform, false);

        Intitialize();

        OnPotentialTilesUpdatedInNode(GetComponent<Node>().potentialTiles);
    }

    private void OnEnable() => GetComponent<Node>().OnPotentialTilesUpdated.AddListener(OnPotentialTilesUpdatedInNode);
    private void OnDisable() => GetComponent<Node>().OnPotentialTilesUpdated.RemoveListener(OnPotentialTilesUpdatedInNode);

    //# Public Methods 
    public void OnPotentialTilesUpdatedInNode(List<Tile> updatedTileList)
    {
        for (int i = 0; i < NodeManager.instance.allTiles.Count; i++)
        {
            if (!updatedTileList.Contains(NodeManager.instance.allTiles[i]))
                tileVisualizations[i].SetActive(false);
        }
    }

    public void Remove()
    {
        Destroy(tileVisualizerGroup.gameObject);
        Destroy(this);
    }

    //# Private Methods 
    private void Intitialize()
    {
        List<Tile> allTiles = NodeManager.instance.allTiles;

        foreach (var tile in allTiles)
        {
            GameObject tileVisualization = Instantiate(tile.prefab, parent: tileVisualizerGroup, worldPositionStays: false);
            tileVisualizations.Add(tileVisualization);

            if (tile == allTiles[0])
                tileVisualization.transform.localPosition = new Vector3(0.6f, 0, -0.6f);
            else if (tile == allTiles[1])
                tileVisualization.transform.localPosition = new Vector3(-0.6f, 0, -0.6f);
            else if (tile == allTiles[2])
                tileVisualization.transform.localPosition = new Vector3(0.6f, 0, 0.6f);
            else if (tile == allTiles[3])
                tileVisualization.transform.localPosition = new Vector3(-0.6f, 0, 0.6f);
            else
            {
                Debug.LogError($"Cannot find position for a fifth tileVisualization.");
                Destroy(tileVisualization);
            }

            tileVisualization.transform.localScale = new Vector3(1f, 0.4f, 1f);
        }
    }
}