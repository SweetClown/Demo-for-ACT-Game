using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace SweetClown
{
    public class BaceonInteractable : Interactable
    {
        [Header("Baceon Info")]
        public int BaceonID;

        [Header("Active")]
        public NetworkVariable<bool> isActivated = new NetworkVariable<bool> (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [Header("VFX")]
        [SerializeField] GameObject activatedParticles;

        [Header("Interaction Text")]
        [SerializeField] string unactivatedInteractionText = "Restore the baceon";
        [SerializeField] string activatedInteractionText = "Rest";

        [Header("Teleport Transform")]
        [SerializeField] Transform teleportTransform;

        protected override void Start()
        {
            base.Start();

            if (IsOwner) 
            {
                if (WorldSaveGameManager.instance.currentCharacterData.baceon.ContainsKey(BaceonID))
                {
                    isActivated.Value = WorldSaveGameManager.instance.currentCharacterData.baceon[BaceonID];
                }
                else
                {
                    isActivated.Value = false;
                }
            }
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkDespawn();

            //If we join when the status has already changed, we force the Onchange function to run here upon joining
            if (!IsOwner)
            {
                OnIsActivatedChanged(false, isActivated.Value);
            }

            isActivated.OnValueChanged += OnIsActivatedChanged;

            WorldObjectManager.instance.AddBaceonToList(this);
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();

            isActivated.OnValueChanged -= OnIsActivatedChanged;
        }

        private void RestoreBaceon(PlayerManager player) 
        {
            isActivated.Value = true;

            //If our save file contains info on this baceon, remove it
            if (WorldSaveGameManager.instance.currentCharacterData.baceon.ContainsKey(BaceonID))
                WorldSaveGameManager.instance.currentCharacterData.baceon.Remove(BaceonID);

            //Then Re-Add it with the value of "True" (Is Activated)
            WorldSaveGameManager.instance.currentCharacterData.baceon.Add(BaceonID, true);

            player.playerAnimatorManager.PlayTargetActionAnimation("Activate_Baceon_01", true);
            //Hide Weapon Models Whily play animation if you desire

            //Send A Pop Up
            PlayerUIManager.instance.playerUIPopUpManager.SendBaceonRestorePopUp("Restored the Baceon");

            StartCoroutine(WaitForAnimationAndPopUpThenRestoreCollider());

        }

        private void RestAtBaceon(PlayerManager player) 
        {
            PlayerUIManager.instance.playerUIBaceonManager.OpenMenu();

            interactableCollider.enabled = true; //Temporapily re-enabling the collider here until we add the menu so you can respawn monsters
            player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxHealth.Value;
            player.playerNetworkManager.currentMana.Value = player.playerNetworkManager.maxMana.Value;
            player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;

            //Test Only
            player.playerNetworkManager.remainingHealthPoison.Value = 3;
            player.playerNetworkManager.remainingManaPoison.Value = 3;

            //Update / Force move quest character (Todo)
            WorldAIManager.instance.ResetAllCharacters();
        }

        private IEnumerator WaitForAnimationAndPopUpThenRestoreCollider() 
        {
            yield return new WaitForSeconds(2); // This Should Give enough time for the animation to play and the pop up to being fading
            interactableCollider.enabled = true;
        }

        private void OnIsActivatedChanged(bool oldStatus, bool newStatus) 
        {
            if (isActivated.Value)
            {
                activatedParticles.SetActive(true);
                interactableText = activatedInteractionText;
            }
            else
            { 
                interactableText = unactivatedInteractionText;
            }
        }

        public override void Interact(PlayerManager player)
        {
            base.Interact(player);

            if (player.isPerformingAction)
                return;

            if (player.playerCombatManager.isUsingItem)
                return;

            WorldSaveGameManager.instance.currentCharacterData.lastBaceonRestedAt = BaceonID;

            if (!isActivated.Value) 
            {
                RestoreBaceon(player);
            }
            else 
            {
                RestAtBaceon(player);
            }
        }

        public void TeleportToBaceon() 
        {
            //The player is only able to teleport when not in a co-op game so we can grab the local player from the network manager
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            //Enable Loading Screen
            PlayerUIManager.instance.playerUILoadingScreenManager.ActiveateLoadingScreen();

            player.transform.position = teleportTransform.position;

            //Disable Loading Screen
            PlayerUIManager.instance.playerUILoadingScreenManager.DeactiveateLoadingScreen();
        }
    }
}
