using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Character Actions/Weapon Actions/Off Hand Melee Action")]
    public class OffHandMeleeAction :WeaponItemAction
    {
        public override void AttemptToPerfromAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
        {
            base.AttemptToPerfromAction(playerPerformingAction, weaponPerformingAction);

            //Check for power stance action (Duel attack)

            //Check for can block
            if (!playerPerformingAction.playerCombatManager.canBlock)
                return;

            if (playerPerformingAction.playerCombatManager.isUsingItem)
                return;

            if (playerPerformingAction.playerNetworkManager.isAttacking.Value) 
            {
                //Disable Blocking
                if (playerPerformingAction.IsOwner) 
                    playerPerformingAction.playerNetworkManager.isBlocking.Value = false;

                return;
            }

            if (playerPerformingAction.playerNetworkManager.isBlocking.Value)
                return;

            if (playerPerformingAction.IsOwner) 
            {
                playerPerformingAction.playerNetworkManager.isBlocking.Value = true;
            }
        }
    }
}
