using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using System;
using TMPro;

namespace SG
{
    public class PlayerUIEquipmentManager : PlayerUIMenu
    {
        [Header("Weapon Slots")]
        [SerializeField] Image rightHandSlot01;
        private Button rightHandSlot01Button;
        [SerializeField] Image rightHandSlot02;
        private Button rightHandSlot02Button;
        [SerializeField] Image rightHandSlot03;
        private Button rightHandSlot03Button;

        [SerializeField] Image leftHandSlot01;
        private Button leftHandSlot01Button;
        [SerializeField] Image leftHandSlot02;
        private Button leftHandSlot02Button;
        [SerializeField] Image leftHandSlot03;
        private Button leftHandSlot03Button;

        [SerializeField] Image headEquipmentSlot;
        private Button headEquipmentSlotButton;
        [SerializeField] Image bodyEquipmentSlot;
        private Button bodyEquipmentSlotButton;
        [SerializeField] Image legEquipmentSlot;
        private Button legEquipmentSlotButton;
        [SerializeField] Image handEquipmentSlot;
        private Button handEquipmentSlotButton;

        [SerializeField] Image QuickSlot01;
        private Button QuickSlot01Button;
        [SerializeField] TextMeshProUGUI QuickSlot01Count;
        [SerializeField] Image QuickSlot02;
        private Button QuickSlot02Button;
        [SerializeField] TextMeshProUGUI QuickSlot02Count;
        [SerializeField] Image QuickSlot03;
        private Button QuickSlot03Button;
        [SerializeField] TextMeshProUGUI QuickSlot03Count;

        //This Inventory Populates with Related items when changing equipment
        [Header("Equipment Inventory")]
        //Current Selected Equipment Slot
        [SerializeField] GameObject equipmentInventoryWindow;
        public EquipmentType currentSelectedEquipmentSlot;
        [SerializeField] GameObject equipmentInventorySlotPrefab;
        [SerializeField] Transform equipmentInventoryContentWindow;
        [SerializeField] Item currentSelectedItem;

        private void Awake()
        {
            rightHandSlot01Button = rightHandSlot01.GetComponentInParent<Button>(true);
            rightHandSlot02Button = rightHandSlot02.GetComponentInParent<Button>(true);
            rightHandSlot03Button = rightHandSlot03.GetComponentInParent<Button>(true);
            leftHandSlot01Button = leftHandSlot01.GetComponentInParent<Button>(true);
            leftHandSlot02Button = leftHandSlot02.GetComponentInParent<Button>(true);
            leftHandSlot03Button = leftHandSlot03.GetComponentInParent<Button>(true);

            headEquipmentSlotButton = headEquipmentSlot.GetComponentInParent<Button>(true);
            bodyEquipmentSlotButton = bodyEquipmentSlot.GetComponentInParent<Button>(true);
            legEquipmentSlotButton = legEquipmentSlot.GetComponentInParent<Button>(true);
            handEquipmentSlotButton = handEquipmentSlot.GetComponentInParent<Button>(true);

            QuickSlot01Button = QuickSlot01.GetComponentInParent<Button>(true);
            QuickSlot02Button = QuickSlot02.GetComponentInParent<Button>(true);
            QuickSlot03Button = QuickSlot03.GetComponentInParent<Button>(true);

        }

        public override void OpenMenu()
        {
            base.OpenMenu();

            ToggleEquipmentButtons(true);
            equipmentInventoryWindow.SetActive(false);
            ClearEquipmentInventory();
            RefreshEquipmentSlotIcon();
        }

        public void RefreshMenu() 
        {

            ClearEquipmentInventory();
            RefreshEquipmentSlotIcon();
        }

        private void ToggleEquipmentButtons(bool isEnabled) 
        {
            rightHandSlot01Button.enabled = isEnabled;
            rightHandSlot02Button.enabled = isEnabled;
            rightHandSlot03Button.enabled = isEnabled;

            leftHandSlot01Button.enabled = isEnabled;
            leftHandSlot02Button.enabled = isEnabled;
            leftHandSlot03Button.enabled = isEnabled;

            headEquipmentSlotButton.enabled = isEnabled;
            bodyEquipmentSlotButton.enabled = isEnabled;
            legEquipmentSlotButton.enabled = isEnabled;
            handEquipmentSlotButton.enabled = isEnabled;

            QuickSlot01Button.enabled = isEnabled;
            QuickSlot02Button.enabled = isEnabled;
            QuickSlot03Button.enabled = isEnabled;
        }

        public void SelectLastSelectedEquipmentSlot()
        {
            Button lastSelectedButton = null;
            ToggleEquipmentButtons(true);
            switch (currentSelectedEquipmentSlot)
            {
                case EquipmentType.RightWeapon01:
                    lastSelectedButton = rightHandSlot01Button;
                    break;
                case EquipmentType.RightWeapon02:
                    lastSelectedButton = rightHandSlot02Button;
                    break;
                case EquipmentType.RightWeapon03:
                    lastSelectedButton = rightHandSlot03Button;
                    break;
                case EquipmentType.LeftWeapon01:
                    lastSelectedButton = leftHandSlot01Button;
                    break;
                case EquipmentType.LeftWeapon02:
                    lastSelectedButton = leftHandSlot02Button;
                    break;
                case EquipmentType.LeftWeapon03:
                    lastSelectedButton = leftHandSlot03Button;
                    break;
                case EquipmentType.Head:
                    lastSelectedButton = headEquipmentSlotButton;
                    break;
                case EquipmentType.Body:
                    lastSelectedButton = bodyEquipmentSlotButton;
                    break;
                case EquipmentType.Legs:
                    lastSelectedButton = legEquipmentSlotButton;
                    break;
                case EquipmentType.Hands:
                    lastSelectedButton = handEquipmentSlotButton;
                    break;
                case EquipmentType.QuickSlot01:
                    lastSelectedButton = QuickSlot01Button;
                    break;
                case EquipmentType.QuickSlot02:
                    lastSelectedButton = QuickSlot02Button;
                    break;
                case EquipmentType.QuickSlot03:
                    lastSelectedButton = QuickSlot03Button;
                    break;
                default:
                    break;
            }

            if (lastSelectedButton != null) 
            {
                lastSelectedButton.Select();
                lastSelectedButton.OnSelect(null);
            }

            equipmentInventoryWindow.SetActive(false);
        }

        private void RefreshEquipmentSlotIcon() 
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            //Right Weapon 01
            WeaponItem rightHandWeapon01 = player.playerInventoryManager.weaponsInRightHandSlots[0];

            if (rightHandWeapon01.itemIcon != null)
            {
                rightHandSlot01.enabled = true;
                rightHandSlot01.sprite = rightHandWeapon01.itemIcon;
            }
            else 
            {
                rightHandSlot01.enabled = false;
            }

            //Right Weapon 02
            WeaponItem rightHandWeapon02 = player.playerInventoryManager.weaponsInRightHandSlots[1];

            if (rightHandWeapon02.itemIcon != null)
            {
                rightHandSlot02.enabled = true;
                rightHandSlot02.sprite = rightHandWeapon02.itemIcon;
            }
            else
            {
                rightHandSlot02.enabled = false;
            }

            //Right Weapon 03
            WeaponItem rightHandWeapon03 = player.playerInventoryManager.weaponsInRightHandSlots[2];

            if (rightHandWeapon03.itemIcon != null)
            {
                rightHandSlot03.enabled = true;
                rightHandSlot03.sprite = rightHandWeapon03.itemIcon;
            }
            else
            {
                rightHandSlot03.enabled = false;
            }

            //left Weapon 01
            WeaponItem leftHandWeapon01 = player.playerInventoryManager.weaponsInLeftHandSlots[0];

            if (leftHandWeapon01.itemIcon != null)
            {
                leftHandSlot01.enabled = true;
                leftHandSlot01.sprite = leftHandWeapon01.itemIcon;
            }
            else
            {
                leftHandSlot01.enabled = false;
            }

            //left Weapon 02
            WeaponItem leftHandWeapon02 = player.playerInventoryManager.weaponsInLeftHandSlots[1];

            if (leftHandWeapon02.itemIcon != null)
            {
                leftHandSlot02.enabled = true;
                leftHandSlot02.sprite = leftHandWeapon02.itemIcon;
            }
            else
            {
                leftHandSlot02.enabled = false;
            }

            //left Weapon 03
            WeaponItem leftHandWeapon03 = player.playerInventoryManager.weaponsInLeftHandSlots[2];

            if (leftHandWeapon03.itemIcon != null)
            {
                leftHandSlot03.enabled = true;
                leftHandSlot03.sprite = leftHandWeapon03.itemIcon;
            }
            else
            {
                leftHandSlot03.enabled = false;
            }

            //Head Equipment
            HeadEquipmentItem headEquipment = player.playerInventoryManager.headEquipment;

            if (headEquipment != null)
            {
                headEquipmentSlot.enabled = true;
                headEquipmentSlot.sprite = headEquipment.itemIcon;
            }
            else
            {
                headEquipmentSlot.enabled = false;
            }

            //Body Equipment
            BodyEquipmentItem bodyEquipment = player.playerInventoryManager.bodyEquipment;

            if (bodyEquipment != null)
            {
                bodyEquipmentSlot.enabled = true;
                bodyEquipmentSlot.sprite = bodyEquipment.itemIcon;
            }
            else
            {
                bodyEquipmentSlot.enabled = false;
            }

            //leg Equipment
            LegEquipmentItem legEquipment = player.playerInventoryManager.legEquipment;

            if (legEquipment != null)
            {
                legEquipmentSlot.enabled = true;
                legEquipmentSlot.sprite = legEquipment.itemIcon;
            }
            else
            {
                legEquipmentSlot.enabled = false;
            }

            //hand Equipment
            HandEquipmentItem handEquipment = player.playerInventoryManager.handEquipment;

            if (handEquipment != null)
            {
                handEquipmentSlot.enabled = true;
                handEquipmentSlot.sprite = handEquipment.itemIcon;
            }
            else
            {
                handEquipmentSlot.enabled = false;
            }

            //Quick Slots
            QuickSlotItem quickSlotEquipment01 = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];

            if (quickSlotEquipment01 != null)
            {
                QuickSlot01.enabled = true;
                QuickSlot01.sprite = quickSlotEquipment01.itemIcon;

                if (quickSlotEquipment01.isConsumable)
                {
                    QuickSlot01Count.enabled = true;
                    QuickSlot01Count.text = quickSlotEquipment01.GetCurrentAmount(player).ToString();
                }
                else 
                {
                    QuickSlot01Count.enabled = false;
                }
            }
            else
            {
                QuickSlot01.enabled = false;
                QuickSlot01Count.enabled = false;
            }

            QuickSlotItem quickSlotEquipment02 = player.playerInventoryManager.quickSlotItemsInQuickSlots[1];

            if (quickSlotEquipment02 != null)
            {
                QuickSlot02.enabled = true;
                QuickSlot02.sprite = quickSlotEquipment02.itemIcon;

                if (quickSlotEquipment02.isConsumable)
                {
                    QuickSlot02Count.enabled = true;
                    QuickSlot02Count.text = quickSlotEquipment02.GetCurrentAmount(player).ToString();
                }
                else
                {
                    QuickSlot02Count.enabled = false;
                }
            }
            else
            {
                QuickSlot02.enabled = false;
                QuickSlot02Count.enabled = false;
            }

            QuickSlotItem quickSlotEquipment03 = player.playerInventoryManager.quickSlotItemsInQuickSlots[2];

            if (quickSlotEquipment03 != null)
            {
                QuickSlot03.enabled = true;
                QuickSlot03.sprite = quickSlotEquipment03.itemIcon;

                if (quickSlotEquipment03.isConsumable)
                {
                    QuickSlot03Count.enabled = true;
                    QuickSlot03Count.text = quickSlotEquipment03.GetCurrentAmount(player).ToString();
                }
                else
                {
                    QuickSlot03Count.enabled = false;
                }
            }
            else
            {
                QuickSlot03.enabled = false;
                QuickSlot03Count.enabled = false;
            }
        }

        private void ClearEquipmentInventory() 
        {
            foreach (Transform item in equipmentInventoryContentWindow) 
            {
                Destroy(item.gameObject);
            }
        }

        public void LoadEquipmentInventory() 
        {
            ToggleEquipmentButtons(false);
            equipmentInventoryWindow.SetActive(true);

            switch (currentSelectedEquipmentSlot)
            {
                case EquipmentType.RightWeapon01:
                    LoadWeaponInventory();
                    break;
                case EquipmentType.RightWeapon02:
                    LoadWeaponInventory();
                    break;
                case EquipmentType.RightWeapon03:
                    LoadWeaponInventory();
                    break;
                case EquipmentType.LeftWeapon01:
                    LoadWeaponInventory();
                    break;
                case EquipmentType.LeftWeapon02:
                    LoadWeaponInventory();
                    break;
                case EquipmentType.LeftWeapon03:
                    LoadWeaponInventory();
                    break;
                case EquipmentType.Head:
                    LoadHeadEquipmentInventory();
                    break;
                case EquipmentType.Body:
                    LoadBodyEquipmentInventory();
                    break;
                case EquipmentType.Legs:
                    LoadLegsEquipmentInventory();
                    break;
                case EquipmentType.Hands:
                    LoadHandEquipmentInventory();
                    break;
                case EquipmentType.QuickSlot01:
                    LoadQuickSlotInventory();
                    break;
                case EquipmentType.QuickSlot02:
                    LoadQuickSlotInventory();
                    break;
                case EquipmentType.QuickSlot03:
                    LoadQuickSlotInventory();
                    break;
                default:
                    break;
            }
        }

        private void LoadHeadEquipmentInventory()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            List<HeadEquipmentItem> headEquipmentInInventory = new List<HeadEquipmentItem>();

            for (int i = 0; i < player.playerInventoryManager.itemInIvnventory.Count; i++)
            {
                HeadEquipmentItem equipment = player.playerInventoryManager.itemInIvnventory[i] as HeadEquipmentItem;

                if (equipment != null)
                    headEquipmentInInventory.Add(equipment);
            }

            if (headEquipmentInInventory.Count <= 0)
            {
                equipmentInventoryWindow.SetActive(false);
                ToggleEquipmentButtons(true);
                RefreshMenu();
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < headEquipmentInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);

                UI_EquipmenInventorySlot equipmenInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmenInventorySlot>();

                equipmenInventorySlot.AddItem(headEquipmentInInventory[i]);

                //This will select the first button in the list
                if (!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        private void LoadBodyEquipmentInventory()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            List<BodyEquipmentItem> bodyEquipmentInInventory = new List<BodyEquipmentItem>();

            for (int i = 0; i < player.playerInventoryManager.itemInIvnventory.Count; i++)
            {
                BodyEquipmentItem equipment = player.playerInventoryManager.itemInIvnventory[i] as BodyEquipmentItem;

                if (equipment != null)
                    bodyEquipmentInInventory.Add(equipment);
            }

            if (bodyEquipmentInInventory.Count <= 0)
            {
                equipmentInventoryWindow.SetActive(false);
                ToggleEquipmentButtons(true);
                RefreshMenu();
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < bodyEquipmentInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);

                UI_EquipmenInventorySlot equipmenInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmenInventorySlot>();

                equipmenInventorySlot.AddItem(bodyEquipmentInInventory[i]);

                //This will select the first button in the list
                if (!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        private void LoadLegsEquipmentInventory()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            List<LegEquipmentItem> legEquipmentInInventory = new List<LegEquipmentItem>();

            for (int i = 0; i < player.playerInventoryManager.itemInIvnventory.Count; i++)
            {
                LegEquipmentItem equipment = player.playerInventoryManager.itemInIvnventory[i] as LegEquipmentItem;

                if (equipment != null)
                    legEquipmentInInventory.Add(equipment);
            }

            if (legEquipmentInInventory.Count <= 0)
            {
                equipmentInventoryWindow.SetActive(false);
                ToggleEquipmentButtons(true);
                RefreshMenu();
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < legEquipmentInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);

                UI_EquipmenInventorySlot equipmenInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmenInventorySlot>();

                equipmenInventorySlot.AddItem(legEquipmentInInventory[i]);

                //This will select the first button in the list
                if (!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        private void LoadHandEquipmentInventory()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            List<HandEquipmentItem> handEquipmentInInventory = new List<HandEquipmentItem>();

            for (int i = 0; i < player.playerInventoryManager.itemInIvnventory.Count; i++)
            {
                HandEquipmentItem equipment = player.playerInventoryManager.itemInIvnventory[i] as HandEquipmentItem;

                if (equipment != null)
                    handEquipmentInInventory.Add(equipment);
            }

            if (handEquipmentInInventory.Count <= 0)
            {
                equipmentInventoryWindow.SetActive(false);
                ToggleEquipmentButtons(true);
                RefreshMenu();
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < handEquipmentInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);

                UI_EquipmenInventorySlot equipmenInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmenInventorySlot>();

                equipmenInventorySlot.AddItem(handEquipmentInInventory[i]);

                //This will select the first button in the list
                if (!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        private void LoadWeaponInventory()
        {

            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();


            List<WeaponItem> weaponsInInventory = new List<WeaponItem>();

            for (int i = 0; i < player.playerInventoryManager.itemInIvnventory.Count; i++) 
            {
                WeaponItem weapon = player.playerInventoryManager.itemInIvnventory[i] as WeaponItem;

                if (weapon != null)
                    weaponsInInventory.Add(weapon);
            }

            if (weaponsInInventory.Count <= 0) 
            {
                //Send a player message that he has none of item type in inventory
                equipmentInventoryWindow.SetActive(false);
                ToggleEquipmentButtons(true);
                RefreshMenu();
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < weaponsInInventory.Count; i++) 
            {
                GameObject inventorySlotGameObject =  Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);

                UI_EquipmenInventorySlot equipmenInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmenInventorySlot>();

                equipmenInventorySlot.AddItem(weaponsInInventory[i]);

                //This will select the first button in the list
                if (!hasSelectedFirstInventorySlot) 
                {
                    hasSelectedFirstInventorySlot= true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        private void LoadQuickSlotInventory()
        {

            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();


            List<QuickSlotItem> quickSlotItemInInventory = new List<QuickSlotItem>();

            for (int i = 0; i < player.playerInventoryManager.itemInIvnventory.Count; i++)
            {
                QuickSlotItem quickSlotItem = player.playerInventoryManager.itemInIvnventory[i] as QuickSlotItem;

                if (quickSlotItem != null)
                    quickSlotItemInInventory.Add(quickSlotItem);
            }

            if (quickSlotItemInInventory.Count <= 0)
            {
                //Send a player message that he has none of item type in inventory
                equipmentInventoryWindow.SetActive(false);
                ToggleEquipmentButtons(true);
                RefreshMenu();
                return;
            }

            bool hasSelectedFirstInventorySlot = false;

            for (int i = 0; i < quickSlotItemInInventory.Count; i++)
            {
                GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);

                UI_EquipmenInventorySlot equipmenInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmenInventorySlot>();

                equipmenInventorySlot.AddItem(quickSlotItemInInventory[i]);

                //This will select the first button in the list
                if (!hasSelectedFirstInventorySlot)
                {
                    hasSelectedFirstInventorySlot = true;
                    Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                    inventorySlotButton.Select();
                    inventorySlotButton.OnSelect(null);
                }
            }
        }

        public void SelectEquipmentSlot(int equipmentSlot) 
        {
            currentSelectedEquipmentSlot = (EquipmentType)equipmentSlot;
        }

        public void UnEquipSelectedItem() 
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            Item unequippedItem;

            switch (currentSelectedEquipmentSlot)
            {
                case EquipmentType.RightWeapon01:
                    unequippedItem = player.playerInventoryManager.weaponsInRightHandSlots[0];

                    if (unequippedItem != null)
                    {
                        player.playerInventoryManager.weaponsInRightHandSlots[0] = Instantiate(WorldItemDataBase.Instance.unarmedWeapon);

                        if (unequippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                            player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }

                    if (player.playerInventoryManager.rightHandWeaponIndex == 0)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;

                    break;

                case EquipmentType.RightWeapon02:
                    unequippedItem = player.playerInventoryManager.weaponsInRightHandSlots[1];

                    if (unequippedItem != null)
                    {
                        player.playerInventoryManager.weaponsInRightHandSlots[1] = Instantiate(WorldItemDataBase.Instance.unarmedWeapon);

                        if (unequippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                            player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }

                    if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;

                    break;
                case EquipmentType.RightWeapon03:
                    unequippedItem = player.playerInventoryManager.weaponsInRightHandSlots[2];

                    if (unequippedItem != null)
                    {
                        player.playerInventoryManager.weaponsInRightHandSlots[2] = Instantiate(WorldItemDataBase.Instance.unarmedWeapon);

                        if (unequippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                            player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }

                    if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
                    break;

                case EquipmentType.LeftWeapon01:
                    unequippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[0];

                    if (unequippedItem != null)
                    {
                        player.playerInventoryManager.weaponsInLeftHandSlots[0] = Instantiate(WorldItemDataBase.Instance.unarmedWeapon);

                        if (unequippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                            player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }

                    if (player.playerInventoryManager.leftHandWeaponIndex == 0)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
                    break;

                case EquipmentType.LeftWeapon02:
                    unequippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[1];

                    if (unequippedItem != null)
                    {
                        player.playerInventoryManager.weaponsInLeftHandSlots[1] = Instantiate(WorldItemDataBase.Instance.unarmedWeapon);

                        if (unequippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                            player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }

                    if (player.playerInventoryManager.leftHandWeaponIndex == 1)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
                    break;

                case EquipmentType.LeftWeapon03:
                    unequippedItem = player.playerInventoryManager.weaponsInLeftHandSlots[2];

                    if (unequippedItem != null)
                    {
                        player.playerInventoryManager.weaponsInLeftHandSlots[2] = Instantiate(WorldItemDataBase.Instance.unarmedWeapon);

                        if (unequippedItem.itemID != WorldItemDataBase.Instance.unarmedWeapon.itemID)
                            player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }

                    if (player.playerInventoryManager.leftHandWeaponIndex == 2)
                        player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
                    break;
                case EquipmentType.Head:
                    unequippedItem = player.playerInventoryManager.headEquipment;

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.headEquipment = null;
                    player.playerEquipmentManager.LoadHeadEquipment(player.playerInventoryManager.headEquipment);
                    break;

                case EquipmentType.Body:
                    unequippedItem = player.playerInventoryManager.bodyEquipment;

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.bodyEquipment = null;

                    player.playerEquipmentManager.LoadBodyEquipment(player.playerInventoryManager.bodyEquipment);
                    break;

                case EquipmentType.Legs:
                    unequippedItem = player.playerInventoryManager.legEquipment;

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.legEquipment = null;

                    player.playerEquipmentManager.LoadLegEquipment(player.playerInventoryManager.legEquipment);
                    break;

                case EquipmentType.Hands:
                    unequippedItem = player.playerInventoryManager.handEquipment;

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.handEquipment = null;

                    player.playerEquipmentManager.LoadHandEquipment(player.playerInventoryManager.handEquipment);
                    break;
                case EquipmentType.QuickSlot01:
                    unequippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[0];

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = null;

                    if (player.playerInventoryManager.quickSlotItemIndex == 0)
                        player.playerNetworkManager.currentQuickSlotID.Value = -1;
                    break;
                case EquipmentType.QuickSlot02:
                    unequippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[1];

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = null;

                    if (player.playerInventoryManager.quickSlotItemIndex == 1)
                        player.playerNetworkManager.currentQuickSlotID.Value = -1;
                    break;
                case EquipmentType.QuickSlot03:
                    unequippedItem = player.playerInventoryManager.quickSlotItemsInQuickSlots[2];

                    if (unequippedItem != null)
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);

                    player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = null;

                    if (player.playerInventoryManager.quickSlotItemIndex == 2)
                        player.playerNetworkManager.currentQuickSlotID.Value = -1;
                    break;
            }

            //Refresh Menu
            RefreshMenu();
        }
    }
}