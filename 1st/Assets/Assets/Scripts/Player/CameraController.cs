using Cinemachine;
using UnityEngine;
using static Models;

public class CameraController : MonoBehaviour
{
    public CinemachineVirtualCamera[] cameras;

    public CinemachineVirtualCamera thirdPersonCam;
    public CinemachineVirtualCamera lookAtTargetCam;
    public CinemachineVirtualCamera mapCam;

    public CinemachineVirtualCamera startCamera;
    private CinemachineVirtualCamera currentCam;

    PlayerInputActions playerInputActions;
    public Transform target;
    public bool isMapCamActive;

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
        playerInputActions.Movement.MapCamera.performed += e => EnableMapCam();
        playerInputActions.Enable();

    }

    private void Start()
    {
        currentCam = startCamera;

        for (int i = 0; i < cameras.Length; i++)
        {
            if (cameras[i] == currentCam)
            {
                cameras[i].Priority = 20;
            }
            else
            {
                cameras[i].Priority = 10;
            }
        }
    }

    #region - Update -

    private void Update()
    {
    }

    #endregion

    #region - LateUpdate -
    private void LateUpdate()
    {
        //CalculateCameraChange();
        CameraRotation();
        FollowPlayerCameraTarget();
        if (playerController.isFaceTarget)
        {
            LookAtTarget();
        }
    }

    #endregion

    #region - Position / Rotation -  

    private void CameraRotation()
    {
        var viewInput = playerController.inputView;

        targetRotation.y += (cameraSettings.InvertedX ? -(viewInput.x * cameraSettings.SensitivityX) : (viewInput.x * cameraSettings.SensitivityX) * Time.deltaTime);

        transform.rotation = Quaternion.Euler(targetRotation);
        yGimbalRotation.x += (cameraSettings.InvertedY ? (viewInput.y * cameraSettings.SensitivityY) : -(viewInput.y * cameraSettings.SensitivityY) * Time.deltaTime);

        if (playerController.isFaceTarget || isMapCamActive)
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
        if (IsEnemyNearby() && !playerController.isFaceTarget)
        {
            playerController.isFaceTarget = true;
            ChangeCamera(lookAtTargetCam);
        }else if (playerController.isFaceTarget)
        {
            playerController.isFaceTarget = false;
            ChangeCamera(thirdPersonCam);
        }
        else
        {
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

    private void ChangeCamera(CinemachineVirtualCamera newCam)
    {
        currentCam = newCam;

        currentCam.Priority = 20;

        for (int i = 0; i < cameras.Length; i++) 
        {
            if (cameras[i] != currentCam)
            {
                cameras[i].Priority = 10;
            }
        }
    }

    private void EnableMapCam()
    {
        isMapCamActive = !isMapCamActive;
        
        if (isMapCamActive)
        {
            ChangeCamera(mapCam);
        }
        else
        {
            ChangeCamera(thirdPersonCam);
        }
    }

    private void CalculateCameraChange()
    {
        if (isMapCamActive)
        {
            Debug.Log("x");
            ChangeCamera(mapCam);
        }
    }

    //void SetVirtualCameraActive(CinemachineVirtualCamera camera, bool isActive)
    //{
    //    if (camera != null)
    //    {
    //        camera.gameObject.SetActive(isActive);
    //    }
    //}

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
