using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace SweetClown
{
    public class AIState : ScriptableObject
    {
        public virtual AIState Tick(AICharacterManager aiCharacter) 
        {
            //Do Some Logic to find the player

            //If we have found the player, return the pursue target state instead

            //If we have not found the player, continue to return the idle state
            return this;
        }

        public virtual AIState SwitchState(AICharacterManager aiCharacter, AIState newState) 
        {
            ResetStateFlags(aiCharacter);
            return newState;
        }

        protected virtual void ResetStateFlags(AICharacterManager aiCharacter) 
        {
            //Reset any state flags here so when you return to the state, they are blank once again
        }

        public bool IsDestinationReachable(AICharacterManager aiCharacter, Vector3 destination) 
        {
            aiCharacter.navMeshAgent.enabled = true;

            NavMeshPath navMeshPath = new NavMeshPath();

            if (aiCharacter.navMeshAgent.CalculatePath(destination, navMeshPath) && navMeshPath.status == NavMeshPathStatus.PathComplete)
            {
                return true;
            }
            else 
            {
                return false;
            }
        }
    }
}
