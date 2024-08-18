using UnityEngine;
using System;

public static class Models
{
    #region - Player -

    public enum PlayerStance // enum works like list
    {
        Stand,
        Crouch
    }

    [Serializable]
    public class CameraSettingsModel
    {
        [Header("Camera Settings")]
        public float SensitivityX;
        public bool InvertedX;

        public float SensitivityY;
        public bool InvertedY;

        public float YClampMin = -40f;
        public float YClampMax = 40f;

        [Header("Character")]
        public float CharacterRotationSmoothDamp = 1f;

    }

    [Serializable]
    public class PlayerSettingsModel
    {
        public float CharacterRotationSmoothDamp = 0.6f;

        [Header("Movement Speed")]
        public float walkingSpeed;
        public float runningSpeed;
        public float crouchSpeed;

        public float walkingBackwardSpeed;
        public float runningBackwardSpeed;

        public float walkingStrafingSpeed;
        public float runningStrafingSpeed;

        [Header("Jumping")]
        public float jumpingForce;

        [Header("Slide")]
        [HideInInspector]
        public float slideTime = 0.667f;
    }

    [Serializable]
    public class PlayerStatsModel
    {
        public float Stamina;
        public float MaxStamina;
        public float StaminaDrain;
        public float StaminaRestore;
        public float StaminaDelay;
        public float StaminaCurrentDelay;

    }

    [Serializable]
    public class ChracterStance
    {
        public float CameraHeight;
        public float colliderHeight;
        public Vector3 colliderCenter;
    }

    #endregion
}
