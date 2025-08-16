using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SweetClown
{
    public class Interactable : NetworkBehaviour
    {
        public string interactableText; //Text prompt when entering the interaction collider (pick up item, pull lever)
        [SerializeField] protected Collider interactableCollider; //Collider that check for player interaction
        [SerializeField] protected bool hostOnlyInteractable = true; //When enabled, object cannot be interacted with by co-op players

        protected virtual void Awake()
        {
            if (interactableCollider == null)
                interactableCollider = GetComponent<Collider>();
        }

        protected virtual void Start()
        {

        }

        public virtual void Interact(PlayerManager player) 
        {
            Debug.Log("You have interacted");

            if (!player.IsOwner)
                return;

            interactableCollider.enabled = false;
            player.playerInteractionManager.RemoveInteractionFromList(this);
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();

            //Save Game
            WorldSaveGameManager.instance.SaveGame();
        }

        public virtual void OnTriggerEnter(Collider other) 
        {
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null) 
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;

                if (!player.IsOwner)
                    return;

                //Pass The Interaction to the player 
                player.playerInteractionManager.AddInteractionToList(this);
            }
        }

        public virtual void OnTriggerExit(Collider other) 
        {
            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player != null)
            {
                if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                    return;

                if (!player.IsOwner)
                    return;

                //Remove The Interaction to the player
                player.playerInteractionManager.RemoveInteractionFromList(this);

                PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
            }
        }

    }
}
