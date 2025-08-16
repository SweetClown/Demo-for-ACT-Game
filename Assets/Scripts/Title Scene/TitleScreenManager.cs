using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;
using static UnityEngine.InputManagerEntry;
using TMPro;

namespace SweetClown
{
    public class TitleScreenManager : MonoBehaviour
    {
        public static TitleScreenManager instance;

        [Header("Main Menu Menus")]
        [SerializeField] GameObject titleScreenMainMenu;
        [SerializeField] GameObject titleScreenLoadMenu;
        [SerializeField] GameObject titleScreenCharacterCreationMenu;

        [Header("Main Menu Buttons")]
        [SerializeField] Button loadMenuReturnButton;
        [SerializeField] Button mainMenuLoadGameButton;
        [SerializeField] Button mainMenuNewGameButton;
        [SerializeField] Button deleteCharacterPopUpConfirmButton;

        [Header(" Main Menu Pop Ups")]
        [SerializeField] GameObject noCharacterSlotsPopUp;
        [SerializeField] Button noCharacterSlotsOkayButton;
        [SerializeField] GameObject deleteCharacterSlotPopUp;

        [Header("Character Creation Main Panel Buttons")]
        [SerializeField] Button characterNameButton;
        [SerializeField] Button characterClassButton;
        [SerializeField] Button StartGameButton;

        [Header("Character Creation Secondary Panel Buttons")]
        [SerializeField] Button[] characterClassButtons;

        [Header("Character Creation Secondary Panel Menus")]
        [SerializeField] GameObject characterClassMenu;
        [SerializeField] GameObject characterNameMenu;
        [SerializeField] TMP_InputField characterNameInputField;

        [Header("Character Slots")]
        public CharacterSlot currentSelectedSlot = CharacterSlot.No_Slot;

        [Header("Classes")]
        public CharacterClass[] startingClasses;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void StartNetworkAsHost()
        {
            NetworkManager.Singleton.StartHost();
        }

        public void AttemptToCreateNewCharacter()
        {
            if (WorldSaveGameManager.instance.HasFreeCharacterSlot())
            {
                OpenCharacterCreationMenu();
            }
            else
            {
                DisplayNoFreeCharacterSlotsPopUp();
            }
        }

        public void StartNewGame()
        {
            WorldSaveGameManager.instance.AttemptToCreateNewGame();
        }

        public void OpenLoadGameMenu()
        {
            //Close main menu
            titleScreenMainMenu.SetActive(false);

            //Open Load Menu
            titleScreenLoadMenu.SetActive(true);

            //Select The Return Button First
            //loadMenuReturnButton.Select();
        }

        public void CloseLoadGameMenu()
        {
            //Close Load menu
            titleScreenLoadMenu.SetActive(false);

            //Open Main Menu
            titleScreenMainMenu.SetActive(true);

            //Select The Load Button
            //mainMenuLoadGameButton.Select();
        }

        public void OpenCharacterCreationMenu()
        {
            titleScreenCharacterCreationMenu.SetActive(true);
        }

        public void CloseCharacterCreationMenu()
        {
            titleScreenCharacterCreationMenu.SetActive(false);
        }

        public void OpenChooseCharacterClassSubMenu() 
        {
            ToggleCharacterCreationScreenMainMenuButtons(false);
            characterClassMenu.SetActive(true);
        }

        public void CloseChooseCharacterClassSubMenu() 
        {
            ToggleCharacterCreationScreenMainMenuButtons(true);
            characterClassMenu.SetActive(false);
            characterClassButton.Select();
        }

        public void OpenChooseNameSubMenu()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            ToggleCharacterCreationScreenMainMenuButtons(false);

            characterNameButton.gameObject.SetActive(false);
            characterNameMenu.gameObject.SetActive(true);

            characterNameInputField.Select();
        }

        public void CloseChooseNameSubMenu()
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
            ToggleCharacterCreationScreenMainMenuButtons(true);
            characterNameButton.gameObject.SetActive(true);
            characterNameMenu.gameObject.SetActive(false);

            player.playerNetworkManager.characterName.Value = characterNameInputField.text;
        }

        private void ToggleCharacterCreationScreenMainMenuButtons(bool status) 
        {
            characterNameButton.enabled = status;
            characterClassButton.enabled = status;
            StartGameButton.enabled = status;
        }

        public void DisplayNoFreeCharacterSlotsPopUp() 
        {
            noCharacterSlotsPopUp.SetActive(true);
        }

        public void CloseNoFreeCharacterSlotsPopUp() 
        {
            noCharacterSlotsPopUp.SetActive(false);
        }

        public void SelectCharacterSlot(CharacterSlot characterSlot) 
        {
            currentSelectedSlot = characterSlot;
        }

        public void SelectNoSlot() 
        {
            currentSelectedSlot = CharacterSlot.No_Slot;
        }

        public void AttemptToDeleteCharacterSlot() 
        {
            if (currentSelectedSlot != CharacterSlot.No_Slot) 
            {
                deleteCharacterSlotPopUp.SetActive(true);
            }
        }

        public void DeleteCharacterSlot() 
        {
            deleteCharacterSlotPopUp.SetActive(false);
            WorldSaveGameManager.instance.DeleteGame(currentSelectedSlot);

            //We disable and then enable the load menu to do the refresh the slots
            //The deleted slots will now become inactive
            titleScreenLoadMenu.SetActive(false);
            titleScreenLoadMenu.SetActive(true);
        }
        public void CloseDeleteCharacterPopUp() 
        {
            deleteCharacterSlotPopUp?.SetActive(false);
        }

        public void SelectClass(int classID) 
        {
            PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

            if (startingClasses.Length <= 0)
                return;

            startingClasses[classID].SetClass(player);
            CloseChooseCharacterClassSubMenu();
        }

        public void SetCharacterClass(PlayerManager player, int vitality, int endurance, int mind, int strength, int dexterity, int intelligence, int faith, 
                                      WeaponItem[] mainHandWeapons, WeaponItem[] offHandWeapons, 
                                      HeadEquipmentItem headEquipment, BodyEquipmentItem bodyEquipment, LegEquipmentItem legEquipment, HandEquipmentItem handEquipment,
                                      QuickSlotItem[] quickSlotItem)
        {
            //Set the Stat 
            player.playerNetworkManager.vigor.Value = vitality;
            player.playerNetworkManager.endurance.Value = endurance;
            player.playerNetworkManager.mind.Value = mind;
            player.playerNetworkManager.strength.Value = strength;
            player.playerNetworkManager.dexterity.Value = dexterity;
            player.playerNetworkManager.intelligence.Value = intelligence;
            player.playerNetworkManager.faith.Value = faith;

            //Set the Weapon
            player.playerInventoryManager.weaponsInRightHandSlots[0] = Instantiate(mainHandWeapons[0]);
            player.playerInventoryManager.weaponsInRightHandSlots[1] = Instantiate(mainHandWeapons[1]);
            player.playerInventoryManager.weaponsInRightHandSlots[2] = Instantiate(mainHandWeapons[2]);
            player.playerInventoryManager.currentRightHandWeapon = player.playerInventoryManager.weaponsInRightHandSlots[0];
            player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponsInRightHandSlots[0].itemID;

            player.playerInventoryManager.weaponsInLeftHandSlots[0] = Instantiate(offHandWeapons[0]);
            player.playerInventoryManager.weaponsInLeftHandSlots[1] = Instantiate(offHandWeapons[1]);
            player.playerInventoryManager.weaponsInLeftHandSlots[2] = Instantiate(offHandWeapons[2]);
            player.playerInventoryManager.currentLeftHandWeapon = player.playerInventoryManager.weaponsInLeftHandSlots[0];
            player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponsInLeftHandSlots[0].itemID;

            //Set the armor
            if (headEquipment != null)
            {
                HeadEquipmentItem headEquipmentItem = Instantiate(headEquipment);
                player.playerInventoryManager.headEquipment = headEquipmentItem;
            }
            else 
            {
                player.playerInventoryManager.headEquipment = null;
            }

            if (bodyEquipment != null)
            {
                BodyEquipmentItem bodyEquipmentItem = Instantiate(bodyEquipment);
                player.playerInventoryManager.bodyEquipment = bodyEquipmentItem;
            }
            else
            {
                player.playerInventoryManager.bodyEquipment = null;
            }

            if (legEquipment != null)
            {
                LegEquipmentItem legEquipmentItem = Instantiate(legEquipment);
                player.playerInventoryManager.legEquipment = legEquipmentItem;
            }
            else
            {
                player.playerInventoryManager.legEquipment = null;
            }

            if (handEquipment != null)
            {
                HandEquipmentItem handEquipmentItem = Instantiate(handEquipment);
                player.playerInventoryManager.handEquipment = handEquipmentItem;
            }
            else
            {
                player.playerInventoryManager.handEquipment = null;
            }

            player.playerEquipmentManager.EquipArmor();

            player.playerInventoryManager.quickSlotItemIndex = 0;
            //Set the quick slot item
            if (quickSlotItem[0] != null)
                player.playerInventoryManager.quickSlotItemsInQuickSlots[0] = Instantiate(quickSlotItem[0]);

            if (quickSlotItem[1] != null)
                player.playerInventoryManager.quickSlotItemsInQuickSlots[1] = Instantiate(quickSlotItem[1]);

            if (quickSlotItem[2] != null)
                player.playerInventoryManager.quickSlotItemsInQuickSlots[2] = Instantiate(quickSlotItem[2]);

            player.playerEquipmentManager.LoadQuickSlotEquipment(player.playerInventoryManager.quickSlotItemsInQuickSlots[player.playerInventoryManager.quickSlotItemIndex]);

        }

    }
}
