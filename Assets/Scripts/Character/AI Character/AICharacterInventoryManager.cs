using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SweetClown
{
    public class AICharacterInventoryManager : CharacterInventoryManager
    {
        AICharacterManager aiCharacter;

        [Header("Loot Chance")]
        public int dropItemChance = 10;
        [SerializeField] Item[] droppableItems;

        protected override void Awake()
        {
            base.Awake();

            aiCharacter = GetComponent<AICharacterManager>();
        }

        public void DropItem() 
        {
            if (!aiCharacter.IsOwner)
                return;

            //The status of if this character will drop an item
            bool willDropItem = false;

            //Random Number rolled from 0-100
            int itemChanceRoll = Random.Range(0, 100);

            //If the number is equal to or lower than the item drop chance, we pass the check and drop the item
            if (itemChanceRoll <= dropItemChance)
                willDropItem = true;

            if (!willDropItem)
                return;

            Item generatedItem = droppableItems[Random.Range(0, droppableItems.Length)];

            if (generatedItem == null)
                return;

            GameObject itemPickUpInteractableGameObject = Instantiate(WorldItemDataBase.Instance.pickUpItemPrefab);
            PickUpItemInteractable pickUpInteractable = itemPickUpInteractableGameObject.GetComponent<PickUpItemInteractable>();
            itemPickUpInteractableGameObject.GetComponent<NetworkObject>().Spawn();
            pickUpInteractable.itemID.Value = generatedItem.itemID;
            pickUpInteractable.networkPosition.Value = transform.position;
            pickUpInteractable.droppingCreatureID.Value = aiCharacter.NetworkObjectId;
        }
    }
}
