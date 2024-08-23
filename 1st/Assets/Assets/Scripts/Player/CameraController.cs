using Cinemachine;
using UnityEngine;
using static Models;

public class CameraController : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    public CinemachineVirtualCamera faceTargetVirtualCamera; 
    public CinemachineVirtualCamera defaultVirtualCamera;
    public Transform target; 

    [Header("References")]
    public PlayerController playerController;
    public Transform Player;
    [HideInInspector]
    public Vector3 targetRotation;
    public GameObject yGimbal;
    private Vector3 yGimbalRotation;

    [Header("Settings")]
    public CameraSettingsModel cameraSettings;

    private void Awake()
    {
        playerInputActions = new PlayerInputActions();
        playerInputActions.Movement.LockTarget.performed += e => DetectTarget();
        playerInputActions.Enable();

    }

    #region - Update -

    private void Update()
    {
    }

    #endregion

    #region - LateUpdate -
    private void LateUpdate()
    {
        CameraRotation();
        FollowPlayerCameraTarget();
        if (playerController.isFaceTarget)
        {
            LookAtTarget();
        }
        ChangeCamera();
    }

    #endregion

    #region - Position / Rotation -  

    private void CameraRotation()
    {
        var viewInput = playerController.inputView;

        targetRotation.y += (cameraSettings.InvertedX ? -(viewInput.x * cameraSettings.SensitivityX) : (viewInput.x * cameraSettings.SensitivityX) * Time.deltaTime);

        transform.rotation = Quaternion.Euler(targetRotation);
        yGimbalRotation.x += (cameraSettings.InvertedY ? (viewInput.y * cameraSettings.SensitivityY) : -(viewInput.y * cameraSettings.SensitivityY) * Time.deltaTime);

        if (playerController.isFaceTarget)
        {
            cameraSettings.SensitivityX = 0;
            cameraSettings.SensitivityY = 0;
        }
        else {
            yGimbalRotation.x = Mathf.Clamp(yGimbalRotation.x, cameraSettings.YClampMin, cameraSettings.YClampMax);
            cameraSettings.SensitivityX = 12;
            cameraSettings.SensitivityY = 12;
        }

        yGimbal.transform.localRotation = Quaternion.Euler(yGimbalRotation);

        if (playerController.isTargetMode)
        {
            var currentRotation = playerController.transform.rotation;

            var newRotation = currentRotation.eulerAngles;
            newRotation.y = targetRotation.y;

            currentRotation = Quaternion.Lerp(currentRotation, Quaternion.Euler(newRotation), cameraSettings.CharacterRotationSmoothDamp);

            playerController.transform.rotation = currentRotation;
        }
    }

    private void FollowPlayerCameraTarget()
    {
        transform.position = playerController.cameraTarget.position;
    }

    private void LookAtTarget()
    {
        var directionToTarget = playerController.target.position - transform.position;
        var rotationToTarget = Quaternion.LookRotation(directionToTarget);

        transform.rotation = Quaternion.Euler(0, rotationToTarget.eulerAngles.y, 0);
    }

    private void DetectTarget()
    {
        //playerController.isFaceTarget = !playerController.isFaceTarget;
        if (IsEnemyNearby() && !playerController.isFaceTarget)
        {
            Debug.Log("t");
            playerController.isFaceTarget = true;
        }else if (playerController.isFaceTarget)
        {
            playerController.isFaceTarget = false;
        }
        else
        {
            Debug.Log("f");
            playerController.isFaceTarget = false;
        }
    }

    private bool IsEnemyNearby()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 cameraForward = Camera.main.transform.forward;

        Ray ray = new Ray(cameraPosition, cameraForward);

        Debug.DrawRay(cameraPosition, cameraForward * cameraSettings.sphereCastDistance, Color.red, 2f);

        RaycastHit hitInfo;
        bool isHit = Physics.SphereCast(ray, cameraSettings.sphereCastRadius, out hitInfo, cameraSettings.sphereCastDistance, LayerMask.GetMask("Enemy"));

        return isHit;
    }

    #endregion

    #region - Events -

    private void ChangeCamera()
    {
        if (playerController.isFaceTarget)
        {
            Debug.Log("x");
            SetVirtualCameraActive(faceTargetVirtualCamera, true);
            SetVirtualCameraActive(defaultVirtualCamera, false);
        }
        else
        {
            Debug.Log("y");
            SetVirtualCameraActive(faceTargetVirtualCamera, false);
            SetVirtualCameraActive(defaultVirtualCamera, true);
        }
    }


    void SetVirtualCameraActive(CinemachineVirtualCamera camera, bool isActive)
    {
        if (camera != null)
        {
            camera.gameObject.SetActive(isActive);
        }
    }

    #endregion

    #region - Enable/Disable -

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }

    #endregion
}
