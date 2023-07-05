using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileInformation : MonoBehaviour
{
    public PtdEnums.TileType tileType;
    private TowerObject currentTower = null;

    public Vector3 GetTowerPlacementPosition()
    {
        return transform.position + new Vector3(0, transform.localScale.y, 0);
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
    }
}
