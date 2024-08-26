using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Models;

public class PlayerCombat : MonoBehaviour
{
    private Animator playerAnimator;
    private PlayerInputActions playerInputActions;
    private CameraController cameraController;
    private PlayerController playerController;

    public List<AttackSO> slashCombo;
    public List<AttackSO> kickCombo;

    public float lastSlashClickedTime;
    public float lastSlashComboEnd;
    public int slashComboCounter;

    public float lastKickClickedTime;
    public float lastKickComboEnd;
    public int kickComboCounter;

    public float combatCoolDown;
    private int crouchAttackType;
    public float bigAttackCd;
    public bool isAttacking;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInputActions = new PlayerInputActions();

        playerInputActions.Actions.Fire1.performed += e => Slash();
        playerInputActions.Actions.BigAttack.performed += e => BigAttack();
        playerInputActions.Actions.Kick.performed += e => Kick();

        playerInputActions.Enable();
    }

    void Update()
    {
        ExitAttack();
    }

    #region - Combat -

    private void Slash()
    {
        if ((playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Slash") && playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime < 0.9f) || cameraController.isMapCamActive)
        {
            return;
        }
        Debug.Log("x");
        Debug.Log(slashComboCounter);
        if (Time.time - lastSlashComboEnd > 0.5f && slashComboCounter <= slashCombo.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastSlashClickedTime >= 0.7f)
            {
                playerAnimator.runtimeAnimatorController = slashCombo[slashComboCounter].animatorOV;
                playerAnimator.Play("Slash", 3, 0);
                slashComboCounter++;
                lastSlashClickedTime = Time.time;

                if (slashComboCounter >= slashCombo.Count)
                {
                    slashComboCounter = 0;
                }
            }
        }
    }

    public void BigAttack()
    {
        if (!isAttacking && combatCoolDown == 0 && playerController.IsGrounded() && !cameraController.isMapCamActive)
        {
            StartAttacking();
            combatCoolDown += bigAttackCd;
            playerAnimator.SetTrigger("BigAttack");
        }
    }

    public void Kick()
    {
        if ((playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Kick") && playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime < 0.9f) || cameraController.isMapCamActive)
        {
            return;
        }

        if (Time.time - lastKickComboEnd > 0.5f && kickComboCounter <= kickCombo.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastKickClickedTime >= 0.7f)
            {
                playerAnimator.runtimeAnimatorController = kickCombo[kickComboCounter].animatorOV;
                playerAnimator.Play("Kick", 3, 0);
                kickComboCounter++;
                lastKickClickedTime = Time.time;

                if (kickComboCounter >= kickCombo.Count)
                {
                    kickComboCounter = 0;
                }
            }
        }
    }

    #endregion

    #region - Events -

    private void ExitAttack()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime > 0.9f && (playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Slash") || playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Kick")))
        {
            Debug.Log("p");
            Invoke("EndCombo", 1.5f);
        }
    }

    private void EndCombo()
    {
        Debug.Log("combo ended");
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