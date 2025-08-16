using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SweetClown {
    [CreateAssetMenu(menuName = "A.I/States/PursueTarget")]
    public class PursueTargetState : AIState
    {
        public override AIState Tick(AICharacterManager aiCharacter)
        {
            //Check if we are performing an action 
            if (aiCharacter.isPerformingAction) 
            {
                aiCharacter.characterAnimatorManager.SetAnimatorMovementParameters(0, 0);
                return this;
            }

            aiCharacter.characterAnimatorManager.SetAnimatorMovementParameters(0, 1);

            //Check if our target is null, if we do not have a target, return to idle state
            if (aiCharacter.AICharacterCombatManager.currentTarget == null)
                return SwitchState(aiCharacter, aiCharacter.idle);

            //Make sure our navmesh agent is active, if its not enable it
            if (!aiCharacter.navMeshAgent.enabled)
                aiCharacter.navMeshAgent.enabled = true;

            if (aiCharacter.AICharacterCombatManager.enablePivot) 
            {
                //If our target goes outside of the characters FOV to face them
                if (aiCharacter.AICharacterCombatManager.viewableAngle < aiCharacter.AICharacterCombatManager.minimumFOV ||
                    aiCharacter.AICharacterCombatManager.viewableAngle > aiCharacter.AICharacterCombatManager.maximumFOV)
                {
                    aiCharacter.AICharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }


            aiCharacter.AICharacterLocomotionManager.RotateTowardsAgent(aiCharacter);

            //If we are within combat range of a target, switch state to combat stance state
            //Option 01
            //if (aiCharacter.AICharacterCombatManager.distanceFromTarget <= aiCharacter.combatStance.maximumEngagementDistance)
            //return SwitchState(aiCharacter, aiCharacter.combatStance);

            if (aiCharacter.AICharacterCombatManager.distanceFromTarget <= aiCharacter.navMeshAgent.stoppingDistance)
                return SwitchState(aiCharacter, aiCharacter.combatStance);


            //If the target is not reachable, nad they are far away, return home

            //Pursue the target

            //Option 02
            NavMeshPath path = new NavMeshPath();
            aiCharacter.navMeshAgent.CalculatePath(aiCharacter.AICharacterCombatManager.currentTarget.transform.position, path);
            aiCharacter.navMeshAgent.SetPath(path);

            return this;
        }
    }
}
