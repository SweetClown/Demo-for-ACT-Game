using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Incantation Action")]
    public class CastIncantationAction : WeaponItemAction
    {
        public override void AttemptToPerfromAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerfromAction(playerPerformingAction, weaponPerformingAction);

            if (!playerPerformingAction.IsOwner)
                return;

            if (playerPerformingAction.playerCombatManager.isUsingItem)
                return;

            if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
                return;

            if (!playerPerformingAction.characterLocomotionManager.isGrounded)
                return;

            if (playerPerformingAction.playerInventoryManager.currentSpell == null)
                return;

            if (playerPerformingAction.playerInventoryManager.currentSpell.SpellClass != SpellClass.Incantation)
                return;

            if (playerPerformingAction.IsOwner)
                playerPerformingAction.playerNetworkManager.isAttacking.Value = true;

            CastIncantation(playerPerformingAction, weaponPerformingAction);
        }

        private void CastIncantation(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction) 
        {
            playerPerformingAction.playerInventoryManager.currentSpell.AttemptToCastSpell(playerPerformingAction);
        }
    }

}
