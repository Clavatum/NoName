using UnityEngine;
using UnityEngine.InputSystem;

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

    private void Awake()
    {
        towerInputActions = new TowerInputActions();

        towerInputActions.View.View.performed += e => inputView = e.ReadValue<Vector2>();

        towerInputActions.Enable();
    }

    private void Start()
    {

    }

    void Update()
    {
        if (isPlacingTower)
        {
            MoveTowerWithMouse();  
        }

        InstantiateTower(); 
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

        isPlacingTower = true;
    }
   
    #region - Enable/Disable -

    private void OnEnable()
    {
        towerInputActions.Enable();
    }

    private void OnDisable()
    {
        towerInputActions.Disable();
    }

    #endregion
}
