using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;

public class IdleChange : StateMachineBehaviour
{
    [SerializeField]
    private int numberOfIdleType = 4;
    [SerializeField]
    private float timeUntilBored = 5f;

    private float idleType;
    private bool isBored;
    public float idleTime;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        ResetIdle();
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (!animator.GetBool("CanIdle"))
        {
            ResetIdle();
            animator.SetFloat("IdleType", idleType, 0.2f, Time.deltaTime);
            return;
        }
        if (!isBored)
        {
            idleTime += Time.deltaTime;
            if(idleTime > timeUntilBored && stateInfo.normalizedTime % 1 < 0.02f)
            {
                isBored = true;
                idleType = Random.Range(1, numberOfIdleType + 1);
                idleType = (idleType * 2) - 1;
                animator.SetFloat("IdleType", idleType - 1);      
            }
        }else if(stateInfo.normalizedTime % 1 > 0.98f) {
            ResetIdle();
        }
        animator.SetFloat("IdleType", idleType, 0.2f, Time.deltaTime);
    }

    private void ResetIdle()
    {
        if (isBored)
        {
            idleType--;
        }
        isBored = false;
        idleTime = 0f;
    }

}
