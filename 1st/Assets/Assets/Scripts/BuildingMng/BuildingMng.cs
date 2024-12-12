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
    public static bool ignoreTowerClick = false;
    private CapsuleCollider towerCollider;

    private GameObject selectedTower;
    public static bool isPanelActive = false;

    private GameObject towerUIPanel;

    private Color originalColor;

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

    private bool CanBuildHere(Vector3 position)
    {
        Ray ray = new Ray(position + Vector3.up * 10f, Vector3.down);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("Buildable"))
            {
                return true;
            }
            else if (hit.collider.CompareTag("Path"))
            {
                return false;
            }
        }
        return false;
    }

    private void MoveTowerWithMouse()
    {
        mouseWorldPosition = GetMouseWorldPosition();

        if (mouseWorldPosition != Vector3.zero && currentTowerPreview != null)
        {
            currentTowerPreview.position = new Vector3(mouseWorldPosition.x, currentTowerPreview.localScale.y / 6.5f, mouseWorldPosition.z);

            if (CanBuildHere(mouseWorldPosition) && !IsCollidingWithOtherTower())
            {
                currentTowerPreview.GetComponent<Renderer>().material.color = originalColor;
            }
            else
            {
                currentTowerPreview.GetComponent<Renderer>().material.color = Color.red;
            }
        }
    }

    private bool IsCollidingWithOtherTower()
    {
        Collider[] colliders = Physics.OverlapBox(
            currentTowerPreview.position,
            currentTowerPreview.localScale / 2,
            Quaternion.identity);

        foreach (Collider collider in colliders)
        {
            if (collider.gameObject.CompareTag("Tower"))
            {
                Debug.Log("You can not build tower here");
                return true;
            }
        }
        return false;
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

        towerCollider = currentTowerPreview.GetComponent<CapsuleCollider>();
        if (towerCollider != null)
        {
            towerCollider.enabled = false;
        }

        originalColor = currentTowerPreview.GetComponent<Renderer>().material.color;

        isPlacingTower = true;
    }

    private void InstantiateTower()
    {
        if (towerInputActions.Actions.MouseLeft.triggered && isPlacingTower)
        {
            if (mouseWorldPosition != Vector3.zero)
            {
                if (!IsCollidingWithOtherTower() && CanBuildHere(mouseWorldPosition))
                {
                    if (towerCollider != null)
                    {
                        towerCollider.enabled = true;
                    }

                    isPlacingTower = false;
                    currentTowerPreview = null;
                }
                else
                {
                    Debug.Log("You can not build tower here");
                }
            }
        }
    }

    private void DetectTowerClick()
    {
        if (ignoreTowerClick) { return; }
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

                    isPanelActive = !isPanelActive;
                    towerUIPanel.SetActive(isPanelActive);

                    if (isPanelActive)
                    {
                        Vector3 panelPosition = selectedTower.transform.position + new Vector3(0, 28, 0);
                        towerUIPanel.transform.position = panelPosition;

                        towerUIPanel.transform.LookAt(mapCamera.transform);
                        towerUIPanel.transform.Rotate(0, 180, 0);
                    }
                }
            }
        }
    }
}