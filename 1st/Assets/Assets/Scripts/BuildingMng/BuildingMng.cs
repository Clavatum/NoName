using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class BuildingMng : MonoBehaviour
{
    TowerInputActions towerInputActions;

    [SerializeField] private Transform archerTower;
    [SerializeField] private Transform swordsmanTower;
    [SerializeField] private Transform axeTower;

    [HideInInspector]
    public Vector2 inputView;

    private Vector3 mouseWorldPosition;
    [SerializeField] private Camera mapCamera;
    private Transform currentTowerPreview;
    private bool isPlacingTower = false;

    private BoxCollider towerCollider; 

    private GameObject selectedTower;  
    private bool isPanelActive = false;  

    private GameObject towerUIPanel;  
    private Button produceUnitButton; 
    private Button upgradeTowerButton; 

    private void Awake()
    {
        towerInputActions = new TowerInputActions();
        towerInputActions.View.View.performed += e => inputView = e.ReadValue<Vector2>();
        towerInputActions.Enable();
    }

    void Update()
    {
        if (isPlacingTower)
        {
            MoveTowerWithMouse();
        }

        InstantiateTower();
        DetectTowerClick();  
    }

    private Vector3 GetMouseWorldPosition()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = mapCamera.ScreenPointToRay(mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    private void MoveTowerWithMouse()
    {
        mouseWorldPosition = GetMouseWorldPosition();

        if (mouseWorldPosition != Vector3.zero && currentTowerPreview != null)
        {
            currentTowerPreview.position = new Vector3(mouseWorldPosition.x, (currentTowerPreview.localScale.y / 2), mouseWorldPosition.z);
        }
    }

    private void InstantiateTower()
    {
        if (towerInputActions.Actions.MouseLeft.triggered && isPlacingTower)
        {
            if (mouseWorldPosition != Vector3.zero)
            {
                if (towerCollider != null)
                {
                    towerCollider.enabled = true;
                }

                isPlacingTower = false;
                currentTowerPreview = null;
            }
        }
    }

    public void SelectTower(int towerType)
    {
        if (currentTowerPreview != null)
        {
            Destroy(currentTowerPreview.gameObject);
        }

        switch (towerType)
        {
            case 1:
                currentTowerPreview = Instantiate(archerTower);
                break;
            case 2:
                currentTowerPreview = Instantiate(swordsmanTower);
                break;
            case 3:
                currentTowerPreview = Instantiate(axeTower);
                break;
        }

        towerCollider = currentTowerPreview.GetComponent<BoxCollider>();

        if (towerCollider != null)
        {
            towerCollider.enabled = false; 
        }

        isPlacingTower = true;
    }

    private void DetectTowerClick()
    {
        if (towerInputActions.Actions.MouseLeft.triggered)
        {
            Ray ray = mapCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null && hit.collider.gameObject.CompareTag("Tower"))  
                {
                    selectedTower = hit.collider.gameObject;

                    towerUIPanel = selectedTower.transform.Find("Canvas/TowerUIPanel")?.gameObject;

                    produceUnitButton = towerUIPanel.transform.Find("ProduceUnitButton")?.GetComponent<Button>();
                    upgradeTowerButton = towerUIPanel.transform.Find("UpgradeTowerButton")?.GetComponent<Button>();

                    produceUnitButton.onClick.RemoveAllListeners();  
                    produceUnitButton.onClick.AddListener(() => ProduceUnit());

                    upgradeTowerButton.onClick.RemoveAllListeners();  
                    upgradeTowerButton.onClick.AddListener(() => UpgradeTower());

                    isPanelActive = !isPanelActive;
                    towerUIPanel.SetActive(isPanelActive);

                    if (isPanelActive)
                    {
                        Vector3 panelPosition = selectedTower.transform.position + new Vector3(0, 2, 0);  
                        towerUIPanel.transform.position = panelPosition;

                        towerUIPanel.transform.LookAt(mapCamera.transform);
                        towerUIPanel.transform.Rotate(0, 180, 0); 
                    }
                }
            }
        }
    }

    private void ProduceUnit()
    {
        Debug.Log("Units produced");
    }

    private void UpgradeTower()
    {
        Debug.Log("Tower updated");
    }
}
