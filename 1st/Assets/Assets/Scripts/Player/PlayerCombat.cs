using System.Collections.Generic;
using UnityEngine;

public class PlayerCombat : MonoBehaviour
{
    public List<AttackSO> combo;
    public float lastClickedTime;
    public float lastComboEnd;
    int comboCounter;

    private Animator playerAnimator;
    PlayerInputActions playerInputActions;

    private void Awake()
    {
        playerAnimator = GetComponent<Animator>();
        playerInputActions = new PlayerInputActions();

        playerInputActions.Actions.Fire1.performed += e => Attack();
        playerInputActions.Enable();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ExitAttack();
    }

    private void Attack()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Attack") &&
            playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime < 0.9f)
        {
            return;
        }
        Debug.Log("x");
        Debug.Log(comboCounter);
        if (Time.time - lastComboEnd > 0.5f && comboCounter <= combo.Count)
        {
            CancelInvoke("EndCombo");

            if (Time.time - lastClickedTime >= 0.7f)
            {
                playerAnimator.runtimeAnimatorController = combo[comboCounter].animatorOV;
                playerAnimator.Play("Attack", 3, 0);
                comboCounter++;
                lastClickedTime = Time.time;

                if (comboCounter >= combo.Count)
                {
                    comboCounter = 0;
                }
            }
        }
    }

    private void ExitAttack()
    {
        if (playerAnimator.GetCurrentAnimatorStateInfo(3).normalizedTime > 0.9f && playerAnimator.GetCurrentAnimatorStateInfo(3).IsTag("Attack"))
        {
            Debug.Log("p");
            Invoke("EndCombo", 1.5f);
        }
    }

    private void EndCombo()
    {
        Debug.Log("combo ended");
        comboCounter = 0;
        lastComboEnd = Time.time;
    }

    private void OnEnable()
    {
        playerInputActions.Enable();
    }

    private void OnDisable()
    {
        playerInputActions.Disable();
    }
}