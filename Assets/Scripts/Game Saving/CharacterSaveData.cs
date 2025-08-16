using System.Collections;
using System.Collections.Generic;
using System.Security;
using UnityEngine;

namespace SweetClown
{
    [System.Serializable]
    //Since we want to refgerence this data for every save file, This script is not a MonoBehavior and is instead serializable
    public class CharacterSaveData 
    {
        [Header("Scene Index")]
        public int sceneIndex;

        [Header("Character Name")]
        public string characterName = "Test Player";

        [Header("Time Played")]
        public float secondsPlayed;

        [Header("Dead Spot")]
        public bool hasDeadSpot = false;
        public float deadSpotPositionX;
        public float deadSpotPositionY;
        public float deadSpotPositionZ;
        public int deadSpotRuneCount;

        [Header("World Coordinates")]
        public float xPosition;
        public float yPosition;
        public float zPosition;

        [Header("Resources")]
        public int currentHealth;
        public float currentStamina;
        public int currentMana;
        public int runes;

        [Header("Stats")]
        public int vitality;
        public int mind;
        public int endurance;
        public int strength;
        public int dexterity;
        public int intelligence;
        public int faith;

        [Header("Equipment")]
        public int headEquipment;
        public int bodyEquipment;
        public int legEquipment;
        public int handEquipment;

        public int rightWeaponIndex;
        public SerializableWeapon rightWeapon01;
        public SerializableWeapon rightWeapon02;
        public SerializableWeapon rightWeapon03;

        public int leftWeaponIndex;
        public SerializableWeapon leftWeapon01;
        public SerializableWeapon leftWeapon02;
        public SerializableWeapon leftWeapon03;

        public int quickSlotIndex;
        public SerializableQuickSlotItem quickSlotItem01;
        public SerializableQuickSlotItem quickSlotItem02;
        public SerializableQuickSlotItem quickSlotItem03;

        public int currentHealthPosionRemaining = 3;
        public int currentManaPosionRemaining = 3;

        [Header("Inventory")]
        public List<SerializableWeapon> weaponsInInventory;
        public List<int> headEquipmentInInventory;
        public List<int> bodyEquipmentInInventory;
        public List<int> legEquipmentInInventory;
        public List<int> handEquipmentInInventory;
        public List<SerializableQuickSlotItem> quickSlotItemsInInventory;


        public int currentSpell;

        [Header("World Items")]
        public SerializableDictionary<int, bool> worldItemsLooted; //The int is the Item id, the bool is the Looted Status

        [Header("Baceon")]
        public int lastBaceonRestedAt = 0;
        public SerializableDictionary<int, bool> baceon; //The int is the baceon ID, the bool is the "Activated" Status

        [Header("Bosses")]
        public SerializableDictionary<int, bool> bossesAwakened; //The int is the boss ID, the bool is the awakened status
        public SerializableDictionary<int, bool> bossesDefeated; //The int is the boss ID, the bool is the Defeated status

        public CharacterSaveData() 
        {
            baceon = new SerializableDictionary<int, bool>();
            bossesAwakened = new SerializableDictionary<int, bool>();
            bossesDefeated = new SerializableDictionary<int, bool>();
            worldItemsLooted = new SerializableDictionary<int, bool>();

            weaponsInInventory = new List<SerializableWeapon>();
            quickSlotItemsInInventory = new List<SerializableQuickSlotItem>();
            headEquipmentInInventory = new List<int>();
            bodyEquipmentInInventory = new List<int>();
            legEquipmentInInventory = new List<int>();
            handEquipmentInInventory = new List<int>();
        }

    }
}
