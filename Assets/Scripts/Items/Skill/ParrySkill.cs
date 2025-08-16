using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Items/Skill/Parry")]
    public class ParrySkill : Skill
    {
        public override void AttemptToPerformAction(PlayerManager playerPerformingAction)
        {
            base.AttemptToPerformAction(playerPerformingAction);

            if (!CanIUseThisAbility(playerPerformingAction))
                return;

            DeductFocusCost(playerPerformingAction);
            DeductStaminaCost(playerPerformingAction);
            PerformParryTypeBasedOnWeapon(playerPerformingAction);

        }

        public override bool CanIUseThisAbility(PlayerManager playerPerformingAction)
        {
            if (playerPerformingAction.isPerformingAction)
            {
                Debug.Log("Cannot perform skill: You are already performing action");
                return false;
            }

            if (playerPerformingAction.playerNetworkManager.isJumping.Value) 
            {
                Debug.Log("Cannot perform skill: Jumping");
                return false;
            }

            if (!playerPerformingAction.playerLocomotionManager.isGrounded)
            {
                Debug.Log("Cannot perform skill: Not stand on the Grounded");
                return false;
            }

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 1)
            {
                Debug.Log("Cannot perform skill: No stamina");
                return false;
            }
            
            return true;
        }

        //Smaller Weapons Perform Faster Parries
        private void PerformParryTypeBasedOnWeapon(PlayerManager playerPerformingAction) 
        {
            WeaponItem weaponBeingUsed = playerPerformingAction.playerCombatManager.currentWeaponBeingUsed;

            switch (weaponBeingUsed.weaponClass)
            {
                case WeaponClass.StraightSword:
                    playerPerformingAction.playerAnimatorManager.PlayTargetActionAnimation("Parry_01", true);
                    break;
                case WeaponClass.NightSkySword:
                    playerPerformingAction.playerAnimatorManager.PlayTargetActionAnimation("Parry_01", true);
                    break;
                case WeaponClass.RoundShield:
                    break;
                case WeaponClass.Fist:
                    break;
            }
        }
    }
}
