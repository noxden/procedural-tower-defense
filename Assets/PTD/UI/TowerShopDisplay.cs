using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerShopDisplay : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    [SerializeField] private FinanceManagerScriptableObject financeManager;
    [SerializeField] private Tower tower;
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerCostText;
    [SerializeField] private LayerMask layerMask;
    private GameObject prePlacementTowerObject;
    private Camera mainCamera;
    private TileInformation currentTile;
    private bool canPlaceTower = false;

    private void Start()
    {
        mainCamera = Camera.main;

        towerIcon.sprite = tower.towerIcon;
        towerCostText.text = tower.cost.ToString();
        prePlacementTowerObject = Instantiate(tower.towerObject, transform.position, Quaternion.identity);
        prePlacementTowerObject.SetActive(false);
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!financeManager.CanAfford(tower.cost))
            return;

        prePlacementTowerObject.SetActive(true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!financeManager.CanAfford(tower.cost))
            return;

        currentTile = null;
        canPlaceTower = true;

        if (prePlacementTowerObject.activeSelf)
        {
            Ray ray = mainCamera.ScreenPointToRay(eventData.position);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, layerMask))
            {
                TileInformation tileInformation = hit.collider.GetComponent<TileInformation>();
                if (tileInformation == null)
                {
                    canPlaceTower = false;
                    return;
                }

                if (tileInformation.tileType == PtdEnums.TileType.Path)
                    canPlaceTower = false;

                if (tileInformation.HasTower())
                    canPlaceTower = false;

                prePlacementTowerObject.transform.position = tileInformation.GetTowerPlacementPosition();
                currentTile = tileInformation;
            }
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if(currentTile != null && canPlaceTower)
        {
            financeManager.ChangeMoney(-tower.cost);
            currentTile.PlaceTower(tower.towerObject);
        }
        prePlacementTowerObject.SetActive(false);
    }
}
