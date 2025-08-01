using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AICharacterAnimatorManager : CharacterAnimatorManager
    {
        AICharacterManager AICharacter;

        protected override void Awake()
        {
            base.Awake();

            AICharacter = GetComponent<AICharacterManager>();
        }

        private void OnAnimatorMove()
        {
            // Host
            if (AICharacter.IsOwner)
            {
                if (!AICharacter.characterLocomotionManager.isGrounded)
                    return;

                Vector3 velocity = AICharacter.animator.deltaPosition;

                AICharacter.characterController.Move(velocity);
                AICharacter.transform.rotation *= AICharacter.animator.deltaRotation;
            }
            //Client
            else 
            {
                if (!AICharacter.characterLocomotionManager.isGrounded)
                    return;

                Vector3 velocity = AICharacter.animator.deltaPosition; 

                AICharacter.characterController.Move(velocity);
                AICharacter.transform.position = Vector3.SmoothDamp(transform.position,
                    AICharacter.characterNetworkManager.networkPosition.Value, 
                    ref AICharacter.characterNetworkManager.networkPositionVelocity,
                    AICharacter.characterNetworkManager.networkPositionSmoothTime);
                AICharacter.transform.rotation *= AICharacter.animator.deltaRotation;
            }
        }
    }
}
