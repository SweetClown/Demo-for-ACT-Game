using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "A.I/States/Attack State")]
    public class AttackState : AIState
    {
        [Header("Current Attack")]
        [HideInInspector] public AICharacterAttackAction currentAttack;
        [HideInInspector] public bool willPerformCombo = false;

        [Header("State Flags")]
        protected bool hasPerformedAttack = false;
        protected bool hasPerformedCombo = false;

        [Header("Pivot After Attack")]
        [SerializeField] protected bool pivotAfterAttack = false;

        public override AIState Tick(AICharacterManager aiCharacter)
        {
            if (aiCharacter.AICharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            if (aiCharacter.AICharacterCombatManager.currentTarget.isDead.Value) 
                return SwitchState(aiCharacter, aiCharacter.idle);

            aiCharacter.AICharacterCombatManager.RotateTowardsTargetWhilstAttacking(aiCharacter);

            aiCharacter.characterAnimatorManager.UpdateAnimatorMovementParameters(0, 0, false);

            //Set movement values to 0

            if (willPerformCombo && !hasPerformedCombo) 
            {
                if (currentAttack.comboAction != null) 
                {
                //    If we can combo
                //    hasPerformedCombo = true;
                //    currentAttack.comboAction.AttemptToPerformAction(aiCharacter);
                }
            }

            if (aiCharacter.isPerformingAction)
                return this;

            if (!hasPerformedAttack) 
            {
                //If we are still recovering from an action, wait before performing another
                if (aiCharacter.AICharacterCombatManager.actionRecoveryTimer > 0)
                    return this;

                PerformAttack(aiCharacter);

                //Return to the top , so if we have a combo we process that when we are able
                return this;
            }
            if (pivotAfterAttack)
                aiCharacter.AICharacterCombatManager.PivotTowardsTarget(aiCharacter);

            return SwitchState(aiCharacter, aiCharacter.combatStance);
        }

        protected void PerformAttack(AICharacterManager aiCharacter) 
        {
            hasPerformedAttack = true;
            currentAttack.AttemptToPerformAction(aiCharacter);
            aiCharacter.AICharacterCombatManager.actionRecoveryTimer = currentAttack.actionRecoveryTime;

        }

        protected override void ResetStateFlags(AICharacterManager aiCharacter)
        {
            base.ResetStateFlags(aiCharacter);

            hasPerformedAttack = false;
            hasPerformedCombo = false;
        }
    }
}
