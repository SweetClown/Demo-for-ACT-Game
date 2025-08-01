using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SG
{
    public class PlayerInteractionManager : MonoBehaviour
    {
        PlayerManager player;

        private List<Interactable> currentInteractableActions;

        private void Awake()
        {
            player = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            currentInteractableActions = new List<Interactable>(); 
        }

        private void FixedUpdate()
        {
            if (!player.IsOwner)
                return;

            //If our ui menu is not open, and we don.t have a pop up
            if (!PlayerUIManager.instance.menuWindowIsOpen && !PlayerUIManager.instance.popUpWindowIsOpen) 
            {
                CheckForInteractable();
            }
        }

        private void CheckForInteractable() 
        {
            if (currentInteractableActions.Count == 0)
                return;

            if (currentInteractableActions[0] == null) 
            {
                currentInteractableActions.RemoveAt(0); //If the current Interactable item at position 0 become null, We remove position 0 from the list
                return;
            }

            //If we have an interactable action and have not notified our player, we do snow here
            if (currentInteractableActions[0] != null)
                PlayerUIManager.instance.playerUIPopUpManager.SendPlayerMessagePopUp(currentInteractableActions[0].interactableText);
        }

        private void RefreshInteractionList() 
        {
            for (int i = currentInteractableActions.Count - 1; i > -1; i--) 
            {
                if (currentInteractableActions[i] == null)
                    currentInteractableActions.RemoveAt(i);
            }
        }

        public void AddInteractionToList(Interactable interactableObject) 
        {
            RefreshInteractionList();

            if (!currentInteractableActions.Contains(interactableObject))
                currentInteractableActions.Add(interactableObject);
        }

        public void RemoveInteractionFromList(Interactable interactableObject)
        {
            if (currentInteractableActions.Contains(interactableObject))
                currentInteractableActions.Remove(interactableObject);

            RefreshInteractionList();
        }

        public void Interact() 
        {
            //If we prass the interact button with or without an interactable, it will clear the pop up windows
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();

            if (currentInteractableActions.Count == 0)
                return;

            if (currentInteractableActions[0] != null) 
            {
                currentInteractableActions[0].Interact(player);
                RefreshInteractionList();
            }
        }
    }
}
