using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace SweetClown
{
    public class PlayerInventoryManager : CharacterInventoryManager
    {
        public WeaponItem currentRightHandWeapon;
        public WeaponItem currentLeftHandWeapon;
        public WeaponItem currentTwoHandWeapon;

        [Header("Quick Slots")]
        public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[3];
        public int rightHandWeaponIndex = 0;
        public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[3];
        public int leftHandWeaponIndex = 0;
        public SpellItem currentSpell;
        public QuickSlotItem[] quickSlotItemsInQuickSlots = new QuickSlotItem[3];
        public int quickSlotItemIndex = 0;
        public QuickSlotItem currentQuickSlotItem;

        [Header("Armor")]
        public HeadEquipmentItem headEquipment;
        public BodyEquipmentItem bodyEquipment;
        public LegEquipmentItem legEquipment;
        public HandEquipmentItem handEquipment;

        [Header("Inventory")]
        public List<Item> itemInIvnventory;

        public void AddItemToInventory(Item item) 
        {
            itemInIvnventory.Add(item);
        }

        public void RemoveItemFromInventory(Item item) 
        {
            itemInIvnventory.Remove(item);

            for (int i = itemInIvnventory.Count - 1; i > -1; i--) 
            {
                if (itemInIvnventory[i] == null) 
                {
                    itemInIvnventory.RemoveAt(i);
                }
            }
        }
    }
}
