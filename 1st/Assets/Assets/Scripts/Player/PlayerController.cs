using System.Collections;
using UnityEngine;
using static Models;

public class PlayerController : MonoBehaviour
{
    PlayerInputActions playerInputActions;
    Rigidbody characterRb;
    Animator characterAnimator;

    [Header("References")]
    private CapsuleCollider playerCapsuleCollider;
    public Transform feetTransform;
    public Transform cameraHolder;
    public Transform cameraTarget;
    public CameraController cameraController;

    [HideInInspector]
    public Vector2 inputMovement;
    [HideInInspector]
    public Vector2 inputView;

    [Header("Settings")]
    public PlayerSettingsModel playerSettings;
    public bool isTargetMode;
    public bool isWalking;
    public bool isRunning;
    public bool isCrouching = false;
    public bool isGrounded;
    public bool isAttacking;
    public bool isSliding;

    [Header("Camera")]
    private float cameraHeight;
    private float cameraHeightVelocity;

    [Header("Movement")]
    public float movementSpeedOffset = 1f;
    public float movementSmoothDamp = 0.25f;

    private float verticalSpeed;
    private float targetVerticalSpeed;
    private float verticalSpeedVelocity;

    private float horizontalSpeed;
    private float targetHorizontalSpeed;
    private float horizontalSpeedVelocity;

    private Vector3 relativePlayerVelocity;
    private Vector3 cameraRelativeForward;
    private Vector3 cameraRelativeRight;
    Vector3 playerMovement;

    [Header("Stance")]
    private float stanceCheckErrorMargin = 0.05f;
    private float currentSpeed;
    private float crouchHeightVelocity;
    private Vector3 crouchCenterVelocity;
    private float crouchSmoothing;
    public ChracterStance playerStandStance;
    public ChracterStance playerCrouchStance;
    public PlayerStance playerStance;
    public LayerMask playerMask;

    [Header("Player Stats")]
    public PlayerStatsModel playerStats;

    [Header("Gravity")]
    private float groundCheckDistance = 0.72f;
    public LayerMask groundMask;

    [Header("Jumping / Falling")]
    private float fallingSpeed;
    private float fallingSpeedPeak;
    private float fallingThreshold = -4f;
    public float fallingMovementSpeed;
    public float fallingRunningMovementSpeed;
    public float maxFallingMovementSpeed = 5f;
    public bool jumpingTriggered; // make it private later
    public bool fallingTriggered;

    [Header("Combat")]
    public float combatCoolDown;
    private float fire1Timer;
    private float kickTimer;
    
    #region - Awake / Start -

    private void Awake()
    {
        #region - Caching -

        playerCapsuleCollider = GetComponent<CapsuleCollider>();
        characterRb = GetComponent<Rigidbody>();
        characterAnimator = GetComponent<Animator>();
        playerInputActions = new PlayerInputActions();

        #endregion

        #region - Input -

        #region - Movement Inputs -
        playerInputActions.Movement.Movement.performed += e => inputMovement = e.ReadValue<Vector2>();
        playerInputActions.Movement.View.performed += e => inputView = e.ReadValue<Vector2>();
        #endregion

        #region - Action Inputs -
        playerInputActions.Actions.Jump.performed += e => Jump();

        playerInputActions.Actions.Run.performed += e => Run();

        playerInputActions.Actions.Crouch.performed += e => Crouch();

        playerInputActions.Actions.Fire1.performed += e => Fire1();
        playerInputActions.Actions.BigAttack.performed += e => BigAttack();

        playerInputActions.Actions.Kick.performed += e => Kick();
        playerInputActions.Actions.KickHold.performed += e => KickHold();

        playerInputActions.Actions.Slide.performed += e => SlidePressed();

        #endregion

        playerInputActions.Enable();

        #endregion
        
        cameraHeight = cameraHolder.localPosition.y;
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    #endregion

    #region - FixedUpdate / Update -

    private void FixedUpdate()
    {
        IsNearGround();
        CalculateStance();
        CalculateFalling();
        Movement();
        CalculateSlide();
        CalculateRunning();
    }
    private void Update()
    {        
        CalculateCombat();
    }

    #endregion

    #region - Gravity - 

    private bool IsGrounded()
    {
        if (Physics.CheckSphere(transform.position, 0.2f, groundMask))
        {
            isGrounded = true;
            characterAnimator.SetBool("isGrounded", true);
            return true;
        }
        isGrounded = false;
        characterAnimator.SetBool("isGrounded", false);
        return false;
    }

    private bool IsFalling()
    {
        if (fallingSpeed < fallingThreshold)
        {
            return true;
        }
        return false;
    }

    private void CalculateFalling()
    {
        fallingSpeed = relativePlayerVelocity.y;

        if (fallingSpeed < fallingSpeedPeak && fallingSpeed < -0.1f && (fallingTriggered || jumpingTriggered))
        {
            fallingSpeedPeak = fallingSpeed;
        }

        if ((IsFalling() && !IsGrounded() && !jumpingTriggered && !fallingTriggered) || (jumpingTriggered && !fallingTriggered && !IsGrounded()))
        {
            fallingTriggered = true;
            characterAnimator.SetTrigger("Falling");
        }

        if (fallingTriggered && IsGrounded() && fallingSpeed < -1f)
        {
            fallingTriggered = false;
            jumpingTriggered = false;

            fallingSpeedPeak = 0f;
        }
    }

    #endregion

    #region - Movement -

    public bool IsMoving()
    {
        if (relativePlayerVelocity.x > 0.4f || relativePlayerVelocity.x < -0.4f)
        {
            return true;
        }
        if (relativePlayerVelocity.z > 0.4f || relativePlayerVelocity.z < -0.4f)
        {
            return true;
        }
        return false;
    }

    public bool IsInputMoving()
    {
        if (inputMovement.x > 0.2f || inputMovement.x < -0.2f)
        {
            characterAnimator.SetBool("isMoving", true);
            characterAnimator.SetBool("CanIdle", false);
            return true;
        }
        if (inputMovement.y > 0.2f || inputMovement.y < -0.2f)
        {
            characterAnimator.SetBool("isMoving", true);
            characterAnimator.SetBool("CanIdle", false);
            return true;
        }
        characterAnimator.SetBool("isMoving", false);
        characterAnimator.SetBool("CanIdle", true);
        return false;
    }

    private void Movement()
    {
        characterAnimator.SetBool("isTargetMode", isTargetMode);

        relativePlayerVelocity = transform.InverseTransformDirection(characterRb.velocity);

        if (isTargetMode)
        {
            if (inputMovement.y > 0)
            {
                targetVerticalSpeed = (isWalking ? playerSettings.walkingSpeed : playerSettings.runningSpeed);
            }
            else
            {
                targetVerticalSpeed = (isWalking ? playerSettings.walkingBackwardSpeed : playerSettings.runningBackwardSpeed);
            }

            targetHorizontalSpeed = (isWalking ? playerSettings.walkingStrafingSpeed : playerSettings.runningStrafingSpeed);
        }
        else
        {
            var originalRotation = transform.rotation;
            transform.LookAt(playerMovement + transform.position, Vector3.up);
            var newRotation = transform.rotation;
            transform.rotation = Quaternion.Lerp(originalRotation, newRotation, playerSettings.CharacterRotationSmoothDamp);

            float playerSpeed;

            if (isCrouching)
            {
                playerSpeed = playerSettings.crouchSpeed;
            }
            else
            {
                playerSpeed = (isWalking ? playerSettings.walkingSpeed : playerSettings.runningSpeed);
            }

            targetVerticalSpeed = playerSpeed;
            targetHorizontalSpeed = playerSpeed;

        }

        targetVerticalSpeed = (targetVerticalSpeed * movementSpeedOffset) * inputMovement.y;
        targetHorizontalSpeed = (targetHorizontalSpeed * movementSpeedOffset) * inputMovement.x;

        verticalSpeed = Mathf.SmoothDamp(verticalSpeed, targetVerticalSpeed, ref verticalSpeedVelocity, movementSmoothDamp);
        horizontalSpeed = Mathf.SmoothDamp(horizontalSpeed, targetHorizontalSpeed, ref horizontalSpeedVelocity, movementSmoothDamp);

        if (isTargetMode)
        {
            characterAnimator.SetFloat("Vertical", verticalSpeed);
            characterAnimator.SetFloat("Horizontal", horizontalSpeed);
        }
        else
        {
            float verticalActualSpeed = verticalSpeed < 0 ? verticalSpeed * -1 : verticalSpeed;
            float horizontalActualSpeed = horizontalSpeed < 0 ? horizontalSpeed * -1 : horizontalSpeed;

            float animatorVertical = verticalActualSpeed > horizontalActualSpeed ? verticalActualSpeed : horizontalActualSpeed;

            characterAnimator.SetFloat("Vertical", animatorVertical);
        }

        if (IsInputMoving())
        {
            cameraRelativeForward = cameraController.transform.forward;
            cameraRelativeRight = cameraController.transform.right;
        }

        playerMovement = cameraRelativeForward * verticalSpeed;
        playerMovement += cameraRelativeRight * horizontalSpeed;

        if (jumpingTriggered || IsFalling() || !IsGrounded())
        {
            characterAnimator.applyRootMotion = false;

            if (Vector3.Dot(characterRb.velocity, playerMovement) < maxFallingMovementSpeed)
            {
                characterRb.AddForce(playerMovement * (isWalking ? fallingMovementSpeed : fallingRunningMovementSpeed));

            }
        }
        else
        {
            characterAnimator.applyRootMotion = true;
        }

    }

    #endregion

    #region - Running -
    private void Run()
    {
        if (!CanRun())
        {
            return;
        }

        if (playerStats.Stamina > (playerStats.MaxStamina / 4))
        {
            if (isCrouching && CanRun())
            {
                isCrouching = false;
                playerStance = PlayerStance.Stand;
                characterAnimator.SetTrigger("CrouchToStand");
                characterAnimator.SetBool("isCrouching", false);
            }
            isRunning = true;
            isWalking = false;
        }
    }

    private bool CanRun()
    {
        if (isTargetMode)
        {
            return false;
        }

        var runFalloff = 0.4f;

        if ((inputMovement.y < 0 ? inputMovement.y * -1 : inputMovement.y) < runFalloff && (inputMovement.x < 0 ? inputMovement.x * -1 : inputMovement.x) < runFalloff)
        {
            return false;
        }

        return true;
    }

    private void CalculateRunning()
    {

        if (!CanRun())
        {
            isRunning = false;
            isWalking = true;
        }
        if (isRunning)
        {
            if (playerStats.Stamina > 0)
            {
                playerStats.Stamina -= playerStats.StaminaDrain * Time.deltaTime;
            }
            else
            {
                isRunning = false;
                isWalking = true;
            }

            playerStats.StaminaCurrentDelay = playerStats.StaminaDelay;
        }
        else
        {
            if (playerStats.StaminaCurrentDelay <= 0)
            {
                if (playerStats.Stamina < playerStats.MaxStamina)
                {
                    playerStats.Stamina += playerStats.StaminaRestore * Time.deltaTime;
                }
                else
                {
                    playerStats.Stamina = playerStats.MaxStamina;
                }
            }
            else
            {
                playerStats.StaminaCurrentDelay -= Time.deltaTime;
            }
        }
    }

    #endregion

    #region - Jumping -

    private void Jump()
    {
        if (!IsGrounded())
        {
            return;
        }
        if (isCrouching)
        {
            isCrouching = false;
            characterAnimator.SetBool("isCrouching", false);
            playerStance = PlayerStance.Stand;
            characterAnimator.SetTrigger("CrouchToStand");
            return;
        }    
        jumpingTriggered = true;
        if (IsMoving() && IsInputMoving() && (isWalking || isRunning)) // there is no walking jump anim
        {
            characterAnimator.SetBool("CanIdle", false);
            characterAnimator.SetTrigger("RunningJump");
            characterAnimator.SetTrigger("WalkingJump");
        }
        else
        {
            characterAnimator.SetBool("CanIdle", false);
            characterAnimator.SetTrigger("Jump");
        }
    }

    public void ApplyJumpForce()
    {
        if (!IsGrounded() || isCrouching) { return; }
        characterRb.AddForce(transform.up * playerSettings.jumpingForce, ForceMode.Impulse);
        fallingTriggered = true;
    }

    #endregion

    #region - Slide -
    private void CalculateSlide()
    {
        if (isSliding) { StartCoroutine(Slide()); }
        isSliding = false;
    }

    private IEnumerator Slide()
    {
        playerStance = PlayerStance.Crouch;
        characterAnimator.SetTrigger("Slide");
        characterAnimator.SetBool("CanIdle", false);
        yield return new WaitForSeconds(playerSettings.slideTime);
        playerStance = PlayerStance.Stand;
        characterAnimator.SetBool("CanIdle", true);
    }

    private void SlidePressed()
    {
        if (isRunning && IsGrounded())
        { 
            isSliding = true;
        }
    }

    #endregion

    #region - Combat -

    public void Fire1()
    {
        if (!isAttacking && combatCoolDown == 0 && IsGrounded())
        {
            if (isCrouching)
            {
                StartAttacking();
                combatCoolDown += playerSettings.slashCd;
                var crouchAttack = Random.Range(1, 3);
                characterAnimator.SetTrigger("CrouchAttackSlash" + crouchAttack);
                return;
            }

            if (fire1Timer <= 0)
            {
                fire1Timer = 0.4f;
                return;
            }
            StartAttacking();
            combatCoolDown += playerSettings.slashCd;
            var attack = Random.Range(1,3);
            characterAnimator.SetTrigger("AttackSlash" + attack);
        }
    }
    public void BigAttack()
    {
        if (!isAttacking && combatCoolDown == 0 && IsGrounded())
        {
            StartAttacking();
            combatCoolDown += playerSettings.slashCd + 0.5f;
            characterAnimator.SetTrigger("BigAttack");
        }
    }

    public void Kick()
    {
        if (!isAttacking && combatCoolDown == 0 && IsGrounded())
        {
            if (kickTimer <= 0)
            {
                kickTimer = 0.4f;
                return;
            }
            StartAttacking();
            combatCoolDown += playerSettings.kickCd;
            var attack = Random.Range(1, 3);
            characterAnimator.SetTrigger("Kick" + attack);
        }
    }
    public void KickHold()
    {
        if (!isAttacking && combatCoolDown == 0 && IsGrounded())
        {
            StartAttacking();
            combatCoolDown += playerSettings.kickCd;
            var attack = Random.Range(1, 3);
            characterAnimator.SetTrigger("KickHold" + attack);
        }
    }

    public void CalculateCombat()
    {
        if (fire1Timer >= 0) { fire1Timer -= Time.deltaTime; }
        if(kickTimer >= 0) { kickTimer -= Time.deltaTime; }
        if(combatCoolDown > 0)
        {
            if (!isAttacking)
            {
                combatCoolDown -= Time.deltaTime;
            }
        }else if (combatCoolDown <= 0)
        {
            combatCoolDown = 0;
        }
        else
        {
            isTargetMode = false;
        }

        if (IsFalling())
        {
            isTargetMode = false;
            isAttacking = false;
        }
    }

    #endregion

    #region - Events - 

    public void StartAttacking()
    {
        isAttacking = true;
        
        characterAnimator.SetBool("CanIdle", false);
    }

    public void FinishAttacking()
    {
        isAttacking = false;

        characterAnimator.SetBool("CanIdle", true);
    }

    #endregion

    #region - Stance -

    private void CalculateStance()
    {
        var currentStance = playerStandStance;

        if (playerStance == PlayerStance.Crouch)
        {
            characterAnimator.SetBool("CanIdle", false); // there is no any different crouch idle anim
            currentStance = playerCrouchStance;
        }
        if (!IsGrounded()) 
        {
            currentStance.colliderHeight = 1.2f;
            if(IsNearGround())
            {
                currentStance.colliderHeight = 1.8f;
                return;
            }
        }
        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, currentStance.CameraHeight, ref cameraHeightVelocity, crouchSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);

        playerCapsuleCollider.height = Mathf.SmoothDamp(playerCapsuleCollider.height, currentStance.colliderHeight, ref crouchHeightVelocity, crouchSmoothing);
        playerCapsuleCollider.center = Vector3.SmoothDamp(playerCapsuleCollider.center, currentStance.colliderCenter, ref crouchCenterVelocity, crouchSmoothing);
    }

    private bool IsNearGround()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }



    private void Crouch()
    {
        if (playerStance == PlayerStance.Crouch)
        {
            if (StanceCheck(playerStandStance.colliderHeight))
            {
                return; // if player under the something which has a collider, this line ignores the input which returned from crouch position to stand position
            }
            isWalking = true;
            characterAnimator.SetBool("isCrouching", false);
            isCrouching = false;
            characterAnimator.SetTrigger("CrouchToStand");
            playerStance = PlayerStance.Stand;
            return;
        }

        if (StanceCheck(playerCrouchStance.colliderHeight))
        {
            return; // if player under the something which has a collider, this line ignores the input which returned from prone position to crouch positon
        }
        isWalking = false;
        characterAnimator.SetBool("isCrouching", true);
        isCrouching = true;
        playerStance = PlayerStance.Crouch;

        if (isCrouching)
        {
            characterAnimator.SetTrigger("StandToCrouch");
        }
    }

    private bool StanceCheck(float stanceCheckHeight)
    {
        var start = new Vector3(feetTransform.position.x, feetTransform.position.y + playerCapsuleCollider.radius + stanceCheckErrorMargin, feetTransform.position.z); // start represents the top of collider
        var end = new Vector3(feetTransform.position.x, feetTransform.position.y - playerCapsuleCollider.radius - stanceCheckErrorMargin + stanceCheckHeight, feetTransform.position.z); // end represents the bottom of collider

        return Physics.CheckCapsule(start, end, playerCapsuleCollider.radius, playerMask);
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

    #region - Gizmos -

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(transform.position, 0.2f);
    }

    #endregion

}