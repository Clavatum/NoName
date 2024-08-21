using UnityEngine;
using static Models;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    public PlayerController playerController;
    [HideInInspector]
    public Vector3 targetRotation;
    public GameObject yGimbal;
    private Vector3 yGimbalRotation;

    [Header("Settings")]
    public CameraSettingsModel cameraSettings;

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
    }

    #endregion

    #region - Position / Rotation -  

    private void CameraRotation()
    {
        var viewInput = playerController.inputView;

        targetRotation.y += (cameraSettings.InvertedX ? -(viewInput.x * cameraSettings.SensitivityX) : (viewInput.x * cameraSettings.SensitivityX) * Time.deltaTime);

        transform.rotation = Quaternion.Euler(targetRotation);
        yGimbalRotation.x += (cameraSettings.InvertedY ? (viewInput.y * cameraSettings.SensitivityY) : -(viewInput.y * cameraSettings.SensitivityY) * Time.deltaTime);
        yGimbalRotation.x = Mathf.Clamp(yGimbalRotation.x, cameraSettings.YClampMin, cameraSettings.YClampMax);

        yGimbal.transform.localRotation = Quaternion.Euler(yGimbalRotation);
    }

    private void FollowPlayerCameraTarget()
    {
        transform.position = playerController.cameraTarget.position;
    }

    #endregion

}
