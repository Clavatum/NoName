using Combat;
using UnityEngine;

public class ResetAttackOnExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var playerCombat = animator.GetComponent<PlayerCombat>();
        if (playerCombat != null)
        {
            playerCombat.isAttacking = false;
        }
    }
}
