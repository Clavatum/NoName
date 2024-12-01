using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Models;

namespace Combat
{
    public class PlayerCombat : MonoBehaviour
    {
        private Animator playerAnimator;
        private PlayerInputActions playerInputActions;
        private PlayerController playerController;
        public CameraController cameraController;

        public List<AttackSO> slashCombo;
        public List<AttackSO> kickCombo;

        public float lastSlashComboEnd;
        public int slashComboCounter;

        public float lastKickComboEnd;
        public int kickComboCounter;

        public float combatCoolDown;
        private int crouchAttackType;
        public float slashCd;
        public float bigAttackCd;
        public float crouchSlashCd;
        private float fire1Timer;
        private float kickTimer;
        public bool isAttacking;
        public static bool isBlocking;

        private void Awake()
        {
            playerAnimator = GetComponent<Animator>();
            playerController = GetComponent<PlayerController>();
            playerInputActions = new PlayerInputActions();

            playerInputActions.Actions.Fire1.performed += e => Slash();
            playerInputActions.Actions.BigAttack.performed += e => BigAttack();
            playerInputActions.Actions.Kick.performed += e => Kick();

            playerInputActions.Actions.BlockPressed.performed += e => BlockPressed();
            playerInputActions.Actions.BlockReleased.performed += e => BlockReleased();
            playerInputActions.Enable();
        }

        void Update()
        {
            CalculateCombat();
            ExitAttack();
        }

        #region - Combat -

        private void Slash()
        {
            if(combatCoolDown == 0 && playerController.playerStance == PlayerStance.Slide)
            {
                StartAttacking();
                playerAnimator.SetTrigger("SlideAttack");
                combatCoolDown += bigAttackCd;
                return;
            }

            if (combatCoolDown == 0 && playerController.isRunning ) 
            {
                StartAttacking();
                playerAnimator.SetTrigger("RunningAttack");
                combatCoolDown += bigAttackCd;
                return; 
            }
            
            if (playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Slash") && playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime < 0.9f)
            {
                return;
            }

            if (fire1Timer <= 0)
            {
                fire1Timer = 0.4f;
                return;
            }

            if (!playerController.jumpingTriggered && !isAttacking && !cameraController.isMapCamActive && Time.time - lastSlashComboEnd > 0.2f && slashComboCounter <= slashCombo.Count)
            {
                CancelInvoke("EndCombo");

                if (playerController.isCrouching)
                {
                    if (combatCoolDown == 0)
                    {
                        StartAttacking();
                        combatCoolDown += crouchSlashCd;
                        crouchAttackType = Random.Range(1, 3);
                        playerAnimator.SetTrigger("CrouchAttackSlash" + crouchAttackType);
                        return;
                    }
                }
                else
                {
                    if (combatCoolDown == 0)
                    {
                        playerAnimator.runtimeAnimatorController = slashCombo[slashComboCounter].animatorOV;
                        StartAttacking();
                        playerAnimator.Play("Slash", 3, 0);
                        slashComboCounter++;
                        combatCoolDown += slashCd;

                        if (slashComboCounter >= slashCombo.Count)
                        {
                            slashComboCounter = 0;
                        }
                    }
                }
            }
        }

        public void BigAttack()
        {
            if (playerController.playerStance == PlayerStance.Slide)
            {
                return;
            }
            if (playerController.playerStance == PlayerStance.Crouch)
            {
                playerAnimator.SetTrigger("CrouchToStand");
                playerAnimator.SetBool("isCrouching", false);
                playerController.playerStance = PlayerStance.Stand;
                playerController.isCrouching = false;
            }
            if (!playerController.jumpingTriggered && !isAttacking && !cameraController.isMapCamActive && combatCoolDown == 0 && playerController.IsGrounded() && (!playerInputActions.Actions.Fire1.triggered || !playerInputActions.Actions.Kick.triggered))
            {
                //StartAttacking();
                combatCoolDown += bigAttackCd;
                playerAnimator.SetTrigger("BigAttack");
            }
        }

        public void Kick()
        {
            if (playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Kick") && playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime < 0.9f)
            {
                return;
            }

            if (playerController.playerStance == PlayerStance.Slide)
            {
                return;
            }

            if (kickTimer <= 0)
            {
                kickTimer = 0.4f;
                return;
            }

            if (!playerController.jumpingTriggered && !isAttacking && !cameraController.isMapCamActive && Time.time - lastKickComboEnd > 0.5f && kickComboCounter <= kickCombo.Count)
            {
                CancelInvoke("EndCombo");

                if (combatCoolDown == 0)
                {
                    playerAnimator.runtimeAnimatorController = kickCombo[kickComboCounter].animatorOV;
                    //StartAttacking();
                    playerAnimator.Play("Kick", 3, 0);
                    kickComboCounter++;
                    combatCoolDown += slashCd;

                    if (kickComboCounter >= kickCombo.Count)
                    {
                        kickComboCounter = 0;
                    }
                }
            }
        }

        private void BlockPressed()
        {
            if (!cameraController.isMapCamActive)
            {
                isBlocking = true;
                playerAnimator.SetTrigger("BlockPressed");
            }
        }

        private void BlockReleased()
        {
            isBlocking = false;
            playerAnimator.SetTrigger("BlockReleased");
        }

        #endregion

        #region - Events -

        private void ExitAttack()
        {
            if (kickComboCounter != 0 && playerInputActions.Actions.Fire1.triggered)
            {
                kickComboCounter = 0;
            }
            else if (slashComboCounter != 0 && playerInputActions.Actions.Kick.triggered)
            {
                slashComboCounter = 0;
            }
            if (playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime > 0.9f && (playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Slash") || playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Kick")))
            {
                Invoke("EndCombo", 1.5f);
            }
        }

        public void CalculateCombat()
        {
            if (isBlocking)
            {
                playerAnimator.SetBool("IsBlocking", true);
                playerAnimator.SetBool("CanIdle", false);
            }
            else { playerAnimator.SetBool("IsBlocking", false); }

            if (fire1Timer >= 0) { fire1Timer -= Time.deltaTime; }
            if (kickTimer >= 0) { kickTimer -= Time.deltaTime; }
            if (combatCoolDown > 0)
            {
                if (!isAttacking)
                {
                    combatCoolDown -= Time.deltaTime;
                }
            }
            else if (combatCoolDown <= 0)
            {
                combatCoolDown = 0;
            }
        }

        private void EndCombo()
        {
            slashComboCounter = 0;
            kickComboCounter = 0;
            lastSlashComboEnd = Time.time;
            lastKickComboEnd = Time.time;
        }

        public void StartAttacking()
        {
            isAttacking = true;

            playerAnimator.SetBool("CanIdle", false);
        }

        public void FinishAttacking()
        {
            isAttacking = false;

            playerAnimator.SetBool("CanIdle", true);
        }

        public void SetIdleTypeAfterCrouchAttack()
        {
            playerAnimator.SetFloat("IdleType", crouchAttackType - 1);
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
}