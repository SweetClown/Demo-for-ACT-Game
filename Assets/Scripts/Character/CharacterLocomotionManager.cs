using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    public class CharacterLocomotionManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Ground Check & Jumping")]
        [SerializeField] protected float gravityForce = -5.55f;
        [SerializeField] LayerMask groundLayer;
        [SerializeField] float groundCheckSphereRadius = 1;
        [SerializeField] protected Vector3 yVelocity; //The force at which our character is pulled up or down (Jumping or Falling)
        [SerializeField] protected float groundYVelocity = -20; //The force at which our character is sticking to the ground whilst they are grounded
        [SerializeField] protected float fallStartYvelocity = -5;
        protected bool fallingVelocityHasBeenSet = false;
        protected float inAirTimer = 0;

        [Header("Flags")]
        public bool isRolling = false; 
        public bool canMove = true;
        public bool canRotate = true;
        public bool isGrounded = true;
        public bool canRun = true;
        public bool canRoll = true;

        protected virtual void Awake()
        { 
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Update() 
        {
            HandleGroundCheck();

            if (isGrounded)
            {
                if (yVelocity.y < 0)
                {
                    inAirTimer = 0;
                    fallingVelocityHasBeenSet = false;
                    yVelocity.y = groundYVelocity;
                }
            }
            else 
            {
                //if we are not jumping , and our falling velocity has not been set
                if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet) 
                {
                    fallingVelocityHasBeenSet = true;
                    yVelocity.y = fallStartYvelocity;
                }

                inAirTimer = inAirTimer + Time.deltaTime;
                character.animator.SetFloat("InAirTimer", inAirTimer);
                yVelocity.y += gravityForce * Time.deltaTime;
            }
            character.characterController.Move(yVelocity * Time.deltaTime);
        }

        protected void HandleGroundCheck() 
        {
            isGrounded = Physics.CheckSphere(character.transform.position, groundCheckSphereRadius, groundLayer);
        }

        protected void OnDrawGizmosSelected()
        {
            //Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
        }

        public void EnableCanRotate() 
        {
            canRotate = true;
        }

        public void DisableCanRotate() 
        {
            canRotate = false;
        }
    }
}
