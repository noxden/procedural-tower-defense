//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using UnityEngine;

public class TileInformation : MonoBehaviour
{
    public PtdEnums.TileType tileType;
    private TowerObject currentTower = null;

    public Vector3 GetTowerPlacementPosition()
    {
        return transform.position + new Vector3(0, transform.localScale.y / 2, 0);
    }

    public bool HasTower()
    {
        return currentTower != null;
    }

    public void PlaceTower(GameObject tower)
    {
        currentTower = Instantiate(tower).GetComponent<TowerObject>();
        currentTower.transform.position = GetTowerPlacementPosition();
        currentTower.transform.parent = transform;
        currentTower.Place();
    }
}
