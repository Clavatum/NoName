using UnityEngine;

public class ResetSoldierAttackOnExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var solderAI = animator.GetComponent<SoldierAI>();
        if (solderAI != null)
        {
            solderAI.isAttacking = false;
        }
    }
}
