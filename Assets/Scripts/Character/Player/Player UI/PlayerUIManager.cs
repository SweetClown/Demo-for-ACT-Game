using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace SweetClown
{
    public class PlayerUIManager : MonoBehaviour
    {
        public static PlayerUIManager instance;
        [HideInInspector] public PlayerManager localPlayer;

        [Header("NETWORK JOIN")]
        [SerializeField] bool startGameAsClient;

        [HideInInspector] public PlayerUIHudManager playerUIHudManager;
        [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
        [HideInInspector] public PlayerUICharacterMenuManager playerUICharacterMenuManager;
        [HideInInspector] public PlayerUIEquipmentManager playerUIEquipmentManager;
        [HideInInspector] public PlayerUIBaceonManager playerUIBaceonManager;
        [HideInInspector] public PlayerUITeleportLocationManager playerUITeleportLocationManager;
        [HideInInspector] public PlayerUILoadingScreenManager playerUILoadingScreenManager;
        [HideInInspector] public PlayerUILevelUpManager playerUILevelUpManager;

        [Header("UI Flags")]
        public bool menuWindowIsOpen = false; //Inventory Screen, Equipment Menu, BlackSmith Menu
        public bool popUpWindowIsOpen = false; //Item Pick Up, Dialogue pop up 

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

            playerUIHudManager = GetComponentInChildren<PlayerUIHudManager>();
            playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
            playerUICharacterMenuManager = GetComponentInChildren<PlayerUICharacterMenuManager>();
            playerUIEquipmentManager = GetComponentInChildren<PlayerUIEquipmentManager>();
            playerUIBaceonManager = GetComponentInChildren<PlayerUIBaceonManager>();
            playerUITeleportLocationManager = GetComponentInChildren<PlayerUITeleportLocationManager>();
            playerUILoadingScreenManager = GetComponentInChildren<PlayerUILoadingScreenManager>();
            playerUILevelUpManager = GetComponentInChildren<PlayerUILevelUpManager>();
        }

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void Update()
        {
            if (startGameAsClient) 
            {
                startGameAsClient = false;
                // We must first shutdown, Because we have started as a host during the title Screen
                NetworkManager.Singleton.Shutdown();
                // We then restart, as a client
                NetworkManager.Singleton.StartClient();
            }
        }

        public void CloseAllMenuWindows() 
        {
            playerUICharacterMenuManager.CloseMenuAfterFixedFrame();
            playerUIEquipmentManager.CloseMenuAfterFixedFrame();
            playerUIBaceonManager.CloseMenuAfterFixedFrame();
            playerUITeleportLocationManager.CloseMenuAfterFixedFrame();
            playerUILevelUpManager.CloseMenuAfterFixedFrame();
        }
    }
}
