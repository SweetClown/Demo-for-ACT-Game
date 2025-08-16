using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Items/Consumeables/Poison")]
    public class PoisonItem : QuickSlotItem
    {
        [Header("Poison Type")]
        public bool healthPoison = true;

        [Header("Restoration Value")]
        [SerializeField] int poisonRestoration = 50;

        [Header("Empty Item")]
        public GameObject emptyPoisonItem;
        public string emptyPoisonAnimation;

        [Header("FX")]
        [SerializeField] AudioClip healingSFX;
        [SerializeField] GameObject healingParticleFX;

        public override bool CanIuseThisItem(PlayerManager player)
        {
            if (!player.playerCombatManager.isUsingItem && player.isPerformingAction)
                return false;

            if (player.playerNetworkManager.isAttacking.Value)
                return false;

            return true;
        }

        public override void AttemptToUseItem(PlayerManager player)
        {
            if (!CanIuseThisItem(player))
                return;

            if (healthPoison && player.playerNetworkManager.remainingHealthPoison.Value <= 0) 
            {
                if (player.playerCombatManager.isUsingItem)
                    return;

                player.playerCombatManager.isUsingItem = true;

                if (player.IsOwner) 
                {
                    player.playerNetworkManager.HideWeaponsServerRpc();
                    player.playerAnimatorManager.PlayTargetActionAnimation(emptyPoisonAnimation, false, false, true, true, false);
                }

                Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
                GameObject emptyPoison = Instantiate(emptyPoisonItem, player.playerEquipmentManager.rightHandWeaponSlot.transform);
                player.playerEffectsManager.activeQuickSlotItemFX = emptyPoison;
                return;
            }

            if (!healthPoison && player.playerNetworkManager.remainingManaPoison.Value <= 0) 
            {
                if (player.playerCombatManager.isUsingItem)
                    return;

                player.playerCombatManager.isUsingItem = true;

                if (player.IsOwner)
                {
                    player.playerNetworkManager.HideWeaponsServerRpc();
                    player.playerAnimatorManager.PlayTargetActionAnimation(emptyPoisonAnimation, false, false, true, true, false);
                }

                Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
                GameObject emptyPoison = Instantiate(emptyPoisonItem, player.playerEquipmentManager.rightHandWeaponSlot.transform);
                player.playerEffectsManager.activeQuickSlotItemFX = emptyPoison;
                return;
            }

            //Check for chugging

            if (player.playerCombatManager.isUsingItem) 
            {
                if (player.IsOwner)
                    player.playerNetworkManager.isChugging.Value = true;

                return;
            }

            player.playerCombatManager.isUsingItem = true;

            player.playerEffectsManager.activeQuickSlotItemFX = Instantiate(itemModel, player.playerEquipmentManager.rightHandWeaponSlot.transform);

            if (player.IsOwner) 
            {
                player.playerAnimatorManager.PlayTargetActionAnimation(useItemAnimation, false, false, true, true, false);
                player.playerNetworkManager.HideWeaponsServerRpc();
            }
        }

        public override void SuccessfullyToUseItem(PlayerManager player)
        {
            base.SuccessfullyToUseItem(player);

            if (player.IsOwner) 
            {
                if (healthPoison)
                {
                    player.playerNetworkManager.currentHealth.Value += poisonRestoration;
                    player.playerNetworkManager.remainingHealthPoison.Value -= 1;
                }
                else 
                {
                    player.playerNetworkManager.currentMana.Value += poisonRestoration;
                    player.playerNetworkManager.remainingManaPoison.Value -= 1;
                }

                PlayerUIManager.instance.playerUIHudManager.SetQuickSlotItemQuickSlotIcon(player.playerInventoryManager.currentQuickSlotItem);
            }

            if (healthPoison && player.playerNetworkManager.remainingHealthPoison.Value <= 0)
            {
                Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
                GameObject emptyPoison = Instantiate(emptyPoisonItem, player.playerEquipmentManager.rightHandWeaponSlot.transform);
                player.playerEffectsManager.activeQuickSlotItemFX = emptyPoison;
            }

            if (!healthPoison && player.playerNetworkManager.remainingManaPoison.Value <= 0)
            {
                Destroy(player.playerEffectsManager.activeQuickSlotItemFX);
                GameObject emptyPoison = Instantiate(emptyPoisonItem, player.playerEquipmentManager.rightHandWeaponSlot.transform);
                player.playerEffectsManager.activeQuickSlotItemFX = emptyPoison;
            }

            PlayHealingFX(player);
        }

        private void PlayHealingFX(PlayerManager player) 
        {
            Instantiate(WorldCharacterEffectsManager.instance.healingPoisonVFX, player.transform);
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.healingPoisonSFX);
        }

        public override int GetCurrentAmount(PlayerManager player)
        {
            int currentAmount = 0;

            if (healthPoison)
                currentAmount = player.playerNetworkManager.remainingHealthPoison.Value;

            if (!healthPoison)
                currentAmount = player.playerNetworkManager.remainingManaPoison.Value;

            return currentAmount;
                
        }
    }
}
