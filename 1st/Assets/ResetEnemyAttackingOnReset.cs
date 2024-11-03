using UnityEngine;

public class ResetEnemyAttackOnExit : StateMachineBehaviour
{
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        var enemyAI = animator.GetComponent<EnemyAI>();
        if (enemyAI != null)
        {
            enemyAI.isAttacking = false;
        }
    }
}
