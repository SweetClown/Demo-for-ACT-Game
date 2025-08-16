using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Light Attack Action")]
    public class LightAttackWeaponItemAction : WeaponItemAction
    {
        //Main Hand
        [Header("Light Attacks")]
        [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
        [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";
        [SerializeField] string light_Jumping_Attack_01 = "Main_Light_Jump_Attack_01";

        [Header("Run Attacks")]
        [SerializeField] string Running_Attack_01 = "Main_Run_Attack_01";

        [Header("Roll Attacks")]
        [SerializeField] string Rolling_Attack_01 = "Main_Roll_Attack_01";

        [Header("BackStep Attacks")]
        [SerializeField] string BackStep_Attack_01 = "Main_BackStep_Attack_01";

        //Two Hand
        [Header("Two Hand Light Attacks")]
        [SerializeField] string Two_Hand_Light_Attack_01 = "Two_Hand_Light_Attack_01";
        [SerializeField] string Two_Hand_Light_Attack_02 = "Two_Hand_Light_Attack_02";
        [SerializeField] string Two_Hand_Jumping_Attack_01 = "Th_Light_Jump_Attack_01";

        [Header("Two Hand Run Attacks")]
        [SerializeField] string Two_Hand_Running_Attack_01 = "Two_Hand_Run_Attack_01";

        [Header("Two Hand Roll Attacks")]
        [SerializeField] string Two_Hand_Rolling_Attack_01 = "Two_Hand_Roll_Attack_01";

        [Header("Two HandBackStep Attacks")]
        [SerializeField] string Two_Hand_BackStep_Attack_01 = "Two_Hand_BackStep_Attack_01";
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
                PerformJumpingLightAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            if (playerPerformingAction.playerNetworkManager.isJumping.Value)
                return;

            //If we are Sprinting, Performing a running attack
            if (playerPerformingAction.characterNetworkManager.isSprinting.Value) 
            {
                PerformRunningAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            ////If we are Rolling, perform a rolling attack
            if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
            {
                PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            //If we are backstepping, perform a backstep attack
            if (playerPerformingAction.characterCombatManager.canDoBackStepAttack)
            {
                PerformBackStepAttack(playerPerformingAction, weaponPerformingAction);
                return;
            }

            playerPerformingAction.characterCombatManager.AttemptCriticalAttack();

            PerformLightAttack(playerPerformingAction, weaponPerformingAction);
        }

        private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                PerformTwoHandLightAttack(playerPerformingAction, weaponPerformingAction);
            }
            else 
            {
                PerformMainHandLightAttack(playerPerformingAction, weaponPerformingAction);
            }

        }

        private void PerformMainHandLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) 
        {
            //if we are attacking currently , and we can combo, perform the combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                //Perform an attack based on the previours attack we just played
                if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack02, light_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, light_Attack_01, true);
                }
            }
            //otherwise, if we are not already attacking just perform a regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, light_Attack_01, true);
            }
        }

        private void PerformTwoHandLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //if we are attacking currently , and we can combo, perform the combo attack
            if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

                //Perform an attack based on the previours attack we just played
                if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == Two_Hand_Light_Attack_01)
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack02, Two_Hand_Light_Attack_02, true);
                }
                else
                {
                    playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, Two_Hand_Light_Attack_01, true);
                }
            }
            //otherwise, if we are not already attacking just perform a regular attack
            else if (!playerPerformingAction.isPerformingAction)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightAttack01, Two_Hand_Light_Attack_01, true);
            }
        }

        private void PerformRunningAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            //If we are two handing our weapon perform a two hand run attack
            //Else perform a one hand run attack
            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RunningAttack01, Two_Hand_Running_Attack_01, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RunningAttack01, Running_Attack_01, true);
            }
        }

        private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;

            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                //If we are two handing our weapon perform a two hand Rolling attack
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RollingAttack01, Two_Hand_Rolling_Attack_01, true);
            }
            else 
            {
                //Else perform a one hand run attack
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.RollingAttack01, Rolling_Attack_01, true);
            }
        }

        private void PerformBackStepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canDoBackStepAttack = false;

            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.BackStepAttack01, Two_Hand_BackStep_Attack_01, true);
            }
            else 
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.BackStepAttack01, BackStep_Attack_01, true);
            }
        }

        private void PerformJumpingLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
            {
                PerformTwoHandJumpingLightAttack(playerPerformingAction, weaponPerformingAction);
            }
            else
            {
                PerformMainHandJumpingLightAttack(playerPerformingAction, weaponPerformingAction);
            }

        }

        private void PerformMainHandJumpingLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.isPerformingAction)
                return;

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightJumpingAttack01, light_Jumping_Attack_01, true);
        }

        private void PerformTwoHandJumpingLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            if (playerPerformingAction.isPerformingAction)
                return;

            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimation(weaponPerformingAction, AttackType.LightJumpingAttack01, Two_Hand_Jumping_Attack_01, true);
        }

    }
}