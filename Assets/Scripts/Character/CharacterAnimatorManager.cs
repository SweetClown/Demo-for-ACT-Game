using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Unity.VisualScripting;

namespace SweetClown
{
    public class CharacterAnimatorManager : MonoBehaviour
    {
        CharacterManager character;

        int vertical;
        int horizontal;

        [Header("Flags")]
        public bool applyRootMotion = false;

        [Header("Damage Animations")]
        public string LastDamageAnimationPlayed;

        //Ping Hit Reactions
        [SerializeField] string hit_Forward_Ping_01 = "hit_Forward_Ping_01";
        [SerializeField] string hit_Forward_Ping_02 = "hit_Forward_Ping_02";

        [SerializeField] string hit_Backward_Ping_01 = "hit_Backward_Ping_01";
        [SerializeField] string hit_Backward_Ping_02 = "hit_Backward_Ping_02";

        [SerializeField] string hit_Left_Ping_01 = "hit_Left_Ping_01";
        [SerializeField] string hit_Left_Ping_02 = "hit_Left_Ping_02";

        [SerializeField] string hit_Right_Ping_01 = "hit_Right_Ping_01";
        [SerializeField] string hit_Right_Ping_02 = "hit_Right_Ping_02";

        public List<string> forward_Ping_Damage = new List<string>();
        public List<string> backward_Ping_Damage = new List<string>();
        public List<string> left_Ping_Damage = new List<string>();
        public List<string> right_Ping_Damage = new List<string>();

        //Meduim hit reactions
        [SerializeField] string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
        [SerializeField] string hit_Forward_Medium_02 = "hit_Forward_Medium_02";

        [SerializeField] string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
        [SerializeField] string hit_Backward_Medium_02 = "hit_Backward_Medium_02";

        [SerializeField] string hit_Left_Medium_01 = "hit_Left_Medium_01";
        [SerializeField] string hit_Left_Medium_02 = "hit_Left_Medium_02";

        [SerializeField] string hit_Right_Medium_01 = "hit_Right_Medium_01";
        [SerializeField] string hit_Right_Medium_02 = "hit_Right_Medium_02";

        public List<string> forward_Medium_Damage = new List<string>();
        public List<string> backward_Medium_Damage = new List<string>();
        public List<string> left_Medium_Damage = new List<string>();
        public List<string> right_Medium_Damage = new List<string>();

        protected virtual void Awake() 
        {
            character = GetComponent<CharacterManager>();

            vertical = Animator.StringToHash("Vertical");
            horizontal = Animator.StringToHash("Horizontal");
        }

        protected virtual void Start() 
        {
            forward_Ping_Damage.Add(hit_Forward_Ping_01);
            forward_Ping_Damage.Add(hit_Forward_Ping_02);

            backward_Ping_Damage.Add(hit_Backward_Ping_01);
            backward_Ping_Damage.Add(hit_Backward_Ping_02);

            left_Ping_Damage.Add(hit_Left_Ping_01);
            left_Ping_Damage.Add(hit_Left_Ping_02);

            right_Ping_Damage.Add(hit_Right_Ping_01);
            right_Ping_Damage.Add(hit_Right_Ping_02);

            forward_Medium_Damage.Add(hit_Forward_Medium_01);
            forward_Medium_Damage.Add(hit_Forward_Medium_02);

            backward_Medium_Damage.Add(hit_Backward_Medium_01);
            backward_Medium_Damage.Add(hit_Backward_Medium_02);

            left_Medium_Damage.Add(hit_Left_Medium_01);
            left_Medium_Damage.Add(hit_Left_Medium_02);

            right_Medium_Damage.Add(hit_Right_Medium_01);
            right_Medium_Damage.Add(hit_Right_Medium_02);

        }

        public string GetRandomAnimationFromList(List<string> animationList) 
        {
            List<string> finalList = new List<string>();

            foreach (var item in animationList) 
            {
                finalList.Add(item);
            }

            // Check if we have already played this damage animation so it doesnt repeat
            finalList.Remove(LastDamageAnimationPlayed);

            //Check the list for null entries, and remove then
            for (int i = finalList.Count - 1; i > -1; i--) 
            {
                if (finalList[i] == null) 
                {
                    finalList.RemoveAt(i);
                }
            }

            int randomValue = Random.Range(0, finalList.Count);

            return finalList[randomValue];
        }

        public void UpdateAnimatorMovementParameters(float horizontalMovement, float verticalMovement, bool isSprinting) 
        {
            float snappedHorizontal;
            float snappedVertical;

            //This if chain will round the horizontal movement to -1, -0.5, 0, 0.5 or 1;
            if (horizontalMovement > 0 && horizontalMovement <= 0.5f) 
            {
                snappedHorizontal = 0.5f;
            }
            else if (horizontalMovement > 0.5f && horizontalMovement <= 1)
            {
                snappedHorizontal = 1;
            }
            else if (horizontalMovement < 0 && horizontalMovement >= -0.5f)
            {
                snappedHorizontal = -0.5f;
            }
            else if (horizontalMovement < -0.5f && horizontalMovement >= -1)
            {
                snappedHorizontal = -1;
            }
            else
            {
                snappedHorizontal = 0;
            }

            //This if chain will round the vertical movement to -1, -0.5, 0, 0.5 or 1;
            if (verticalMovement > 0 && verticalMovement <= 0.5f)
            {
                snappedVertical = 0.5f;
            }
            else if (verticalMovement > 0.5f && verticalMovement <= 1)
            {
                snappedVertical = 1;
            }
            else if (verticalMovement < 0 && verticalMovement >= -0.5f)
            {
                snappedVertical = -0.5f;
            }
            else if (verticalMovement < -0.5f && verticalMovement >= -1)
            {
                snappedVertical = -1;
            }
            else
            {
                snappedVertical = 0;
            }


            if (isSprinting) 
            {
                snappedVertical = 2;
            }

            character.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
            character.animator.SetFloat(vertical, snappedVertical, 0.01f, Time.deltaTime);
        }

        // This Function will just pass the raw number
        public void SetAnimatorMovementParameters(float horizontalMovement, float verticalMovement) 
        {
            character.animator.SetFloat(vertical, verticalMovement, 0.1f, Time.deltaTime);
            character.animator.SetFloat(horizontal, horizontalMovement, 0.1f, Time.deltaTime);
        }

        public virtual void PlayTargetActionAnimation(string targetAnimation,
                                                      bool isPerformingAction,
                                                      bool applyRootMotion = true,
                                                      bool canRotate = false,
                                                      bool canMove = false,
                                                      bool canRun = true,
                                                      bool canRoll = false) 
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.1f);
            //Can be used to stop character from attempting new actions
            //For example, if you get damaged, and begin performing a damage animation
            //This flag will trun true if we are stunned
            //We can then check for this before attempting new actions
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.characterLocomotionManager.canRun = canRun;
            character.characterLocomotionManager.canRoll = canRoll;

            //Tell the server/host we played an animation, an to play that animation to everybody else present
            character.characterNetworkManager.NotifyTheServerOfActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void PlayTargetActionAnimationInstantly(string targetAnimation,
                                              bool isPerformingAction,
                                              bool applyRootMotion = true,
                                              bool canRotate = false,
                                              bool canMove = false,
                                              bool canRun = true,
                                              bool canRoll = false)
        {
            character.applyRootMotion = applyRootMotion;
            character.animator.Play(targetAnimation);
            //Can be used to stop character from attempting new actions
            //For example, if you get damaged, and begin performing a damage animation
            //This flag will trun true if we are stunned
            //We can then check for this before attempting new actions
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.characterLocomotionManager.canRun = canRun;
            character.characterLocomotionManager.canRoll = canRoll;

            //Tell the server/host we played an animation, an to play that animation to everybody else present
            character.characterNetworkManager.NotifyTheServerOfInstantActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public virtual void PlayTargetAttackActionAnimation(WeaponItem weapon,
                                                            AttackType attackType,
                                                            string targetAnimation,
                                                            bool isPerformingAction,
                                                            bool applyRootMotion = true,
                                                            bool canRotate = false,
                                                            bool canMove = false,
                                                            bool canRoll = false)
        {
            //Keep Track of last attack performed (for combos)
            //Keep track of current attack type (light, heavy, ect)
            //Update animation set to current weapons animations
            //Decide if our attack can be parried
            //Tell the network our "Attacking" Flag is active (for counter damage)
            character.characterCombatManager.currentAttackType = attackType;
            character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;
            UpdateAnimatorController(weapon.weaponAnimator);
            character.applyRootMotion = applyRootMotion;
            character.animator.CrossFade(targetAnimation, 0.1f);
            character.isPerformingAction = isPerformingAction;
            character.characterLocomotionManager.canRotate = canRotate;
            character.characterLocomotionManager.canMove = canMove;
            character.characterNetworkManager.isAttacking.Value = true;
            character.characterLocomotionManager.canRoll = canRoll;

            character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
        }

        public void UpdateAnimatorController(AnimatorOverrideController weaponController) 
        {
            character.animator.runtimeAnimatorController = weaponController;
        }
    }
}

