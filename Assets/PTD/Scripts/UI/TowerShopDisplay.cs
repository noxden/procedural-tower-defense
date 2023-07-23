//========================================================================
// Darmstadt University of Applied Sciences, Expanded Realities
// Course:      [Elective] Procedural Level Generation (Andreas Fuchs)
// Group:       #5 (Procedural Tower Defense)
// Script by:   Jan Rau (769214)
//========================================================================

using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TowerShopDisplay : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private FinanceManagerScriptableObject financeManager;
    [SerializeField] private TooltipSystemScriptableObject tooltipSystem;
    [SerializeField] private WarningManagerScriptableObject warningManager;
    [SerializeField] private GameObject towerObject;
    private Tower tower;
    [SerializeField] private Image towerIcon;
    [SerializeField] private TextMeshProUGUI towerCostText;
    [SerializeField] private LayerMask layerMask;
    private GameObject prePlacementTowerObject;
    private Camera mainCamera;
    private TileInformation currentTile;
    private bool canPlaceTower = false;
    string contentText;

    private void Start()
    {
        mainCamera = Camera.main;
        tower = towerObject.GetComponent<TowerObject>().tower;

        towerIcon.sprite = tower.towerIcon;
        towerCostText.text = tower.cost.ToString();
        prePlacementTowerObject = Instantiate(towerObject, transform.position, Quaternion.identity);
        prePlacementTowerObject.SetActive(false);

        contentText = "";
        contentText += "Cost: " + tower.cost + "\n";
        contentText += "Damage: " + tower.damage + "\n";
        contentText += "Range: " + tower.range + "\n";
        contentText += "Attack Speed: " + (1 / tower.secondsPerAttack) + "\n";
        contentText += "Layers: ";
        foreach (PtdEnums.TileType layer in tower.buildableHeights)
        {
            contentText += layer.ToString() + ", ";
        }
    }
    
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!financeManager.CanAfford(tower.cost))
        {
            warningManager.UpdateWarningUI("Not enough gold!");
            return;
        }

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

            if (Physics.Raycast(ray, out hit, maxDistance:float.MaxValue, layerMask))
            {
                TileInformation tileInformation = hit.collider.GetComponent<TileInformation>();
                if (tileInformation == null)
                {
                    canPlaceTower = false;
                    return;
                }

                if(!tower.buildableHeights.Contains(tileInformation.tileType))
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
            currentTile.PlaceTower(towerObject);
        }
        else
        {
            warningManager.UpdateWarningUI("Can't place tower here!");
        }
        prePlacementTowerObject.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        tooltipSystem.Show(contentText, tower.towerName);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        tooltipSystem.Hide();
    }
}