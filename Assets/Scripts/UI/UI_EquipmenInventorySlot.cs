using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace SweetClown
{
    public class UI_EquipmenInventorySlot : MonoBehaviour
    {
        public Image itemIcon;
        public Image highlightedIcon;
        [SerializeField] public Item currentItem;

        public void AddItem(Item item) 
        {
            if (item == null) 
            {
                itemIcon.enabled = false;
                return;
            }

            itemIcon.enabled = true;

            currentItem = item;
            itemIcon.sprite = item.itemIcon;
        }

        public void SelectSlot() 
        {
            highlightedIcon.enabled = true;
        }

        public void DeselectSlot() 
        {
            highlightedIcon.enabled = false;
        }

        public void EquipItem() 
        {
            PlayerManager player= NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            Item equippedItem;

            switch (PlayerUIManager.instance.playerUIEquipmentManager.currentSelectedEquipmentSlot)
            {
                case EquipmentType.RightWeapon01:
                    //If our current Weapon in this slot, is not an unarmed item, add it to our inventory
                    equippedItem = player.playerInventoryManager.weaponsInRightHandSlots[0];
                    if (equippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }
                    //Then replace the weapon in that slot with our new weapon
                    player.playerInventoryManager.weaponsInRightHandSlots[0] = currentItem as WeaponItem;
                    //Then remove the new weapon from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    //Re equip new weapon if we are holding the current weapon in this slot
                    if (player.playerInventoryManager.rightHandWeaponIndex == 0)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;

                    //Refreshes equipment Window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.RightWeapon02:

                    equippedItem = player.playerInventoryManager.weaponsInRightHandSlots[1];
                    if (equippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    player.playerInventoryManager.weaponsInRightHandSlots[1] = currentItem as WeaponItem;
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;

                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.RightWeapon03:

                    equippedItem = player.playerInventoryManager.weaponsInRightHandSlots[2];
                    if (equippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    player.playerInventoryManager.weaponsInRightHandSlots[2] = currentItem as WeaponItem;
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;

                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.LeftWeapon01:

                    equippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[0];
                    if (equippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    player.playerInventoryManager.weaponsInLeftHandSlots[0] = currentItem as WeaponItem;
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.leftHandWeaponIndex == 0)
                        player.playerNetworkManager.currentLeftHandWeaponID.Value = currentItem.itemID;

                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.LeftWeapon02:

                    equippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[1];
                    if (equippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    player.playerInventoryManager.weaponsInLeftHandSlots[1] = currentItem as WeaponItem;
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.leftHandWeaponIndex == 1)
                        player.playerNetworkManager.currentLeftHandWeaponID.Value = currentItem.itemID;

                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.LeftWeapon03:

                    equippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[2];
                    if (equippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    player.playerInventoryManager.weaponsInLeftHandSlots[2] = currentItem as WeaponItem;
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.leftHandWeaponIndex == 2)
                        player.playerNetworkManager.currentLeftHandWeaponID.Value = currentItem.itemID;

                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;
                case EquipmentType.Head:

                    //If our current Equipment in this slot, is not a null item, add it to our inventory
                    equippedItem = player.playerInventoryManager.headEquipment;

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the armor in that slot with our new armor
                    player.playerInventoryManager.headEquipment = currentItem as HeadEquipmentItem;

                    //Then remove the new armor from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    //Re equir new armor if we are hloding the current armor in this slot
                    player.playerEquipmentManager.LoadHeadEquipment(player.playerInventoryManager.headEquipment);

                    //Refreshes equipment window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.Body:
                    //If our current Equipment in this slot, is not a null item, add it to our inventory
                    equippedItem = player.playerInventoryManager.bodyEquipment;

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the armor in that slot with our new armor
                    player.playerInventoryManager.bodyEquipment = currentItem as BodyEquipmentItem;

                    //Then remove the new armor from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    //Re equir new armor if we are hloding the current armor in this slot
                    player.playerEquipmentManager.LoadBodyEquipment(player.playerInventoryManager.bodyEquipment);

                    //Refreshes equipment window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.Legs:
                    //If our current Equipment in this slot, is not a null item, add it to our inventory
                    equippedItem = player.playerInventoryManager.legEquipment;

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the armor in that slot with our new armor
                    player.playerInventoryManager.legEquipment = currentItem as LegEquipmentItem;

                    //Then remove the new armor from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    //Re equir new armor if we are hloding the current armor in this slot
                    player.playerEquipmentManager.LoadLegEquipment(player.playerInventoryManager.legEquipment);

                    //Refreshes equipment window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;

                case EquipmentType.Hands:
                    //If our current Equipment in this slot, is not a null item, add it to our inventory
                    equippedItem = player.playerInventoryManager.handEquipment;

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the armor in that slot with our new armor
                    player.playerInventoryManager.handEquipment = currentItem as HandEquipmentItem;

                    //Then remove the new armor from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    //Re equir new armor if we are hloding the current armor in this slot
                    player.playerEquipmentManager.LoadHandEquipment(player.playerInventoryManager.handEquipment);

                    //Refreshes equipment window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;
                case EquipmentType.QuickSlot01:
                    //If our current Weapon in this slot, is not an unarmed item, add it to our inventory
                    equippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the weapon in that slot with our new weapon
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = currentItem as QuickSlotItem;

                    //Then remove the new weapon from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.quickSlotItemIndex == 0)
                        player.playerNetworkManager.currentQuickSlotID.Value = currentItem.itemID;

                    //Refreshes equipment Window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;
                case EquipmentType.QuickSlot02:
                    //If our current Weapon in this slot, is not an unarmed item, add it to our inventory
                    equippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[1];

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the weapon in that slot with our new weapon
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = currentItem as QuickSlotItem;

                    //Then remove the new weapon from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.quickSlotItemIndex == 1)
                        player.playerNetworkManager.currentQuickSlotID.Value = currentItem.itemID;

                    //Refreshes equipment Window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;
                case EquipmentType.QuickSlot03:
                    //If our current Weapon in this slot, is not an unarmed item, add it to our inventory
                    equippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[2];

                    if (equippedItem != null)
                    {
                        player.playerInventoryManager.AddItemToInventory(equippedItem);
                    }

                    //Then replace the weapon in that slot with our new weapon
                    player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = currentItem as QuickSlotItem;

                    //Then remove the new weapon from our inventory
                    player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                    if (player.playerInventoryManager.quickSlotItemIndex == 2)
                        player.playerNetworkManager.currentQuickSlotID.Value = currentItem.itemID;

                    //Refreshes equipment Window
                    PlayerUIManager.instance.playerUIEquipmentManager.RefreshMenu();
                    break;
                default:
                    break;

            }

            PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
        }
    }
}
