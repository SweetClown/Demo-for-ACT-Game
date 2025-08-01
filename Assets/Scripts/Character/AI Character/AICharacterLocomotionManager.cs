using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        public void RotateTowardsAgent(AICharacterManager aiCharacter) 
        {
            if (aiCharacter.AICharacterNetworkManager.isMoving.Value) 
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
    }
}
