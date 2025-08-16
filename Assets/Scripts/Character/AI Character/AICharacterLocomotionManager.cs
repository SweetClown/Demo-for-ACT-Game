using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    public class AICharacterLocomotionManager : CharacterLocomotionManager
    {
        AICharacterManager aiCharacter;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        protected override void Update()
        {
            base.Update();

            if (aiCharacter.IsOwner)
            {
                aiCharacter.characterNetworkManager.verticalMovement.Value = aiCharacter.animator.GetFloat("Vertical");
                aiCharacter.characterNetworkManager.horizontalMovement.Value = aiCharacter.animator.GetFloat("Horizontal");
            }
            else
            {
                aiCharacter.animator.SetFloat("Vertical", aiCharacter.AICharacterNetworkManager.verticalMovement.Value, 0.1f, Time.deltaTime);
                aiCharacter.animator.SetFloat("Horizontal", aiCharacter.AICharacterNetworkManager.verticalMovement.Value, 0.1f, Time.deltaTime);
            }
        }

        public void RotateTowardsAgent(AICharacterManager aiCharacter) 
        {
            if (aiCharacter.AICharacterNetworkManager.isMoving.Value) 
            {
                aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
            }
        }
    }
}
