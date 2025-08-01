using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG
{
    public class WorldSaveGameManager : MonoBehaviour
    {
        public static WorldSaveGameManager instance;

        public PlayerManager player;

        [Header("Save/Load")]
        [SerializeField] bool saveGame;
        [SerializeField] bool loadGame;

        [Header("Wrold Scene Index")]
        [SerializeField] int worldSceneIndex = 1;

        [Header("Save Data Writer")]
        private SaveFileDataWriter saveFileDataWriter;

        [Header("Current Character Data")]
        public CharacterSlot currentCharacterSlotBeingUsed;
        public CharacterSaveData currentCharacterData;
        private string savefileName;

        [Header("Character Slots")]
        public CharacterSaveData characterSlot01;
        public CharacterSaveData characterSlot02;
        public CharacterSaveData characterSlot03;
        public CharacterSaveData characterSlot04;
        public CharacterSaveData characterSlot05;
        public CharacterSaveData characterSlot06;
        public CharacterSaveData characterSlot07;
        public CharacterSaveData characterSlot08;
        public CharacterSaveData characterSlot09;
        public CharacterSaveData characterSlot10;


        private void Awake()
        {
            //There Can only be one instance of this script at one time, if another exists, Destroy it.
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            LoadAllCharacterProfiles();
        }

        private void Update()
        {
            if (saveGame)
            {
                saveGame = false;
                SaveGame();
            }

            if (loadGame)
            {
                loadGame = false;
                LoadGame();
            }
        }

        public bool HasFreeCharacterSlot() 
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.savedDataDirectoryPath = Application.persistentDataPath;

            //Check to see if we can create a new save file (check for other exisiting files first)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            //Create a new File, with a file name Depending on which slot we are using
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                return true;
            }

            return false;
        }

        public string DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot characterSlot)
        {
            string fileName = "";

            switch (characterSlot)
            {
                case CharacterSlot.CharacterSlot_01:
                    fileName = "CharacterSlot_01";
                    break;
                case CharacterSlot.CharacterSlot_02:
                    fileName = "CharacterSlot_02";
                    break;
                case CharacterSlot.CharacterSlot_03:
                    fileName = "CharacterSlot_03";
                    break;
                case CharacterSlot.CharacterSlot_04:
                    fileName = "CharacterSlot_04";
                    break;
                case CharacterSlot.CharacterSlot_05:
                    fileName = "CharacterSlot_05";
                    break;
                case CharacterSlot.CharacterSlot_06:
                    fileName = "CharacterSlot_06";
                    break;
                case CharacterSlot.CharacterSlot_07:
                    fileName = "CharacterSlot_07";
                    break;
                case CharacterSlot.CharacterSlot_08:
                    fileName = "CharacterSlot_08";
                    break;
                case CharacterSlot.CharacterSlot_09:
                    fileName = "CharacterSlot_09";
                    break;
                case CharacterSlot.CharacterSlot_10:
                    fileName = "CharacterSlot_10";
                    break;
                default:
                    break;
            }

            return fileName;
        }

        public void AttemptToCreateNewGame()
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.savedDataDirectoryPath = Application.persistentDataPath;

            //Check to see if we can create a new save file (check for other exisiting files first)
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);

            if (!saveFileDataWriter.CheckToSeeIfFileExists()) 
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_01;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            //Create a new File, with a file name Depending on which slot we are using
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_02;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_03;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_04;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_05;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_06;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_07;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_08;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_09;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);

            if (!saveFileDataWriter.CheckToSeeIfFileExists())
            {
                //If this profile slot is not taken, make a new one using this slot
                currentCharacterSlotBeingUsed = CharacterSlot.CharacterSlot_10;
                currentCharacterData = new CharacterSaveData();
                NewGame();
                return;
            }

            //If there are no free slots, notify the player
            TitleScreenManager.instance.DisplayNoFreeCharacterSlotsPopUp();

        }

        public void LoadGame()
        {
            //Load a previous file, with a file name Depending on which slot we are using
            savefileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

            saveFileDataWriter = new SaveFileDataWriter();
            //Generally works on multiple machine types
            saveFileDataWriter.savedDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = savefileName;
            currentCharacterData = saveFileDataWriter.LoadSaveFile();

            LoadWorldScene(worldSceneIndex);
        }

        public void SaveGame()
        {
            //Save the current file under a file name depending on which slot we are using
            savefileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(currentCharacterSlotBeingUsed);

            saveFileDataWriter = new SaveFileDataWriter();
            //Generally works on multiple machine types
            saveFileDataWriter.savedDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = savefileName;

            //Pass the player info, from game , to their save file
            player.SaveGameDataToCurrentCharacterData(ref currentCharacterData);

            //Write that into a json file, saved to this machine
            saveFileDataWriter.CreateNewCharacterSaveFile(currentCharacterData);
        }

        private void NewGame() 
        {
            SaveGame();
            LoadWorldScene(worldSceneIndex);
        }

        public void DeleteGame(CharacterSlot characterSlot) 
        {
            // Choose File based on name

            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.savedDataDirectoryPath = Application.persistentDataPath;
            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(characterSlot);
            saveFileDataWriter.DeleteSaveFile();
        }

        //Load All character profiles on device when starting game
        private void LoadAllCharacterProfiles() 
        {
            saveFileDataWriter = new SaveFileDataWriter();
            saveFileDataWriter.savedDataDirectoryPath = Application.persistentDataPath;

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_01);
            characterSlot01 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_02);
            characterSlot02 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_03);
            characterSlot03 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_04);
            characterSlot04 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_05);
            characterSlot05 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_06);
            characterSlot06 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_07);
            characterSlot07 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_08);
            characterSlot08 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_09);
            characterSlot09 = saveFileDataWriter.LoadSaveFile();

            saveFileDataWriter.saveFileName = DecideCharacterFileNameBasedOnCharacterSlotBeingUsed(CharacterSlot.CharacterSlot_10);
            characterSlot10 = saveFileDataWriter.LoadSaveFile();
        }

        public void LoadWorldScene(int buildIndex) 
        {
            PlayerUIManager.instance.playerUILoadingScreenManager.ActiveateLoadingScreen();
            string worldScene = SceneUtility.GetScenePathByBuildIndex(buildIndex);
            NetworkManager.Singleton.SceneManager.LoadScene(worldScene, LoadSceneMode.Single);

            player.LoadGameDataToCurrentCharacterData(ref currentCharacterData);
        }


        public int GetWorldSceneIndex() 
        {
            return worldSceneIndex;
        }

        public SerializableWeapon GetSerializableWeaponFromWeaponItem(WeaponItem weapon) 
        {
            SerializableWeapon serializableWeapon = new SerializableWeapon();

            //Get Weapon ID
            serializableWeapon.itemID = weapon.itemID;

            //Get Skill ID If one is present (There should always be one by default)
            if (weapon.skillAction != null)
            {
                serializableWeapon.skillID = weapon.skillAction.itemID;
            }
            else 
            {
                //We use an invalid id if there is no skill, so the value will be null if it tries to search for one using the ID
                serializableWeapon.skillID = -1;
            }

            return serializableWeapon;
        }

        public SerializablePoison GetSerializablePoisonFromPoisonItem(PoisonItem poison)
        {
            SerializablePoison serializedFlask = new SerializablePoison();

            serializedFlask.itemID = poison.itemID;

            if (poison != null)
            {
                serializedFlask.itemID = poison.itemID;
            }
            else
            {
                serializedFlask.itemID = -1;
            }

            return serializedFlask;
        }

        public SerializableQuickSlotItem GetSerializableQuickSlotItemFromQuickSlotItem(QuickSlotItem quickSlotItem)
        {
            SerializableQuickSlotItem serializableQuickSlotItem = new SerializableQuickSlotItem();

            if (quickSlotItem != null)
            {
                serializableQuickSlotItem.itemID = quickSlotItem.itemID;
                serializableQuickSlotItem.itemAmount = quickSlotItem.itemAmount;
            }
            else
            {
                serializableQuickSlotItem.itemID = -1;
            }

            return serializableQuickSlotItem;
        }
    }
}
