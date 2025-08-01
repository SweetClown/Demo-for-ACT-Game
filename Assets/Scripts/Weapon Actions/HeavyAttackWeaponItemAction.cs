using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Heavy Attack Action")]
    public class HeavyAttackWeaponItemAction : WeaponItemAction
    {
        [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";
        [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02";// Main = main hand

        [SerializeField] string Two_Hand_Heavy_Attack_01 = "Two_Hand_Heavy_Attack_01";
        [SerializeField] string Two_Hand_Heavy_Attack_02 = "Two_Hand_Heavy_Attack_02";

        [SerializeField] string Main_Hand_Jumping_Heavy_Attack_01 = "Main_Heavy_Jump_Attack_01";
        [SerializeField] string Two_Hand_Jumping_Heavy_Attack_01 = "Th_Heavy_Jump_Attack_01";
        public override void AttemptToPerfromAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerfromAction(playerPerformingAction, weaponPerformingAction);

            if (!playerPerformingAction.IsOwner)
                return;

            if (playerPerformingAction.playerCombatManager.isUsingItem)
                return;

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;


            //If we are in the air, we can perform jumping attack
            if (!playerPerformingAction.playerLocomotionManager.isGrounded) 
            {
                PerformJumpingHeavyAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            if (playerPerformingAction.playerNetworkManager.isJumping.Value)
                return;

            PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
        }

        private void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                PerformTwoHandHeavyAttack(playerPerformingAction, weaponPerformingAction);
            }
            else 
            {
                PerformMainHandHeavyAttack(playerPerformingAction, weaponPerformingAction);
            }
        }

        private void PerformMainHandHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) 
        {
            //if we are attacking currently , and we can combo, perform the combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                //Perform an attack based on the previours attack we just played
                if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack02, heavy_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
                }
            }
            //otherwise, if we are not already attacking just perform a regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
            }
        }

        private void PerformTwoHandHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) 
        {
            //if we are attacking currently , and we can combo, perform the combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                //Perform an attack based on the previours attack we just played
                if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == Two_Hand_Heavy_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack02, Two_Hand_Heavy_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, Two_Hand_Heavy_Attack_01, true);
                }
            }
            //otherwise, if we are not already attacking just perform a regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyAttack01, Two_Hand_Heavy_Attack_01, true);
            }
        }

        private void PerformJumpingHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                PerformTwoHandJumpingHeavyAttack(playerPerformingAction, weaponPerformingAction);
            }
            else
            {
                PerformMainHandJumpingHeavyAttack(playerPerformingAction, weaponPerformingAction);
            }
        }

        private void PerformMainHandJumpingHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) 
        {
            if (playerPerformingAction.isPerformingAction)
                return;

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyJumpingAttack01, Main_Hand_Jumping_Heavy_Attack_01, true);
        }

        private void PerformTwoHandJumpingHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.isPerformingAction)
                return;

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.HeavyJumpingAttack01, Two_Hand_Jumping_Heavy_Attack_01, true);
        }
    }
}
