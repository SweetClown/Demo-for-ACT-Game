using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using Unity.VisualScripting;
using Unity.PlasticSCM.Editor.WebApi;
using System;

namespace SG
{
    public class PlayerManager : CharacterManager
    {
        [Header("Debug Menu")]
        [SerializeField] bool respawnCharacter = false;
        [SerializeField] bool switchRightWeapon = false;

        [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
        [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
        [HideInInspector] public PlayerNetworkManager playerNetworkManager;
        [HideInInspector] public PlayerStatsManager playerStatsManager;
        [HideInInspector] public PlayerInventoryManager playerInventoryManager;
        [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
        [HideInInspector] public PlayerCombatManager playerCombatManager;
        [HideInInspector] public PlayerInteractionManager playerInteractionManager;
        [HideInInspector] public PlayerEffectsManager playerEffectsManager;
        protected override void Awake()
        {
            base.Awake();

            playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
            playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
            playerNetworkManager = GetComponent<PlayerNetworkManager>();
            playerStatsManager = GetComponent<PlayerStatsManager>();
            playerInventoryManager = GetComponent<PlayerInventoryManager>();
            playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
            playerCombatManager = GetComponent<PlayerCombatManager>();
            playerInteractionManager = GetComponent<PlayerInteractionManager>();
            playerEffectsManager = GetComponent<PlayerEffectsManager>();    
        }

        protected override void Update()
        {
            base.Update();

            // If we do not own this gameobject, we do not control or edit it
            if (!IsOwner)
                return;

            //Handle Movement
            playerLocomotionManager.HandleAllMovement();

            // Regen stamina
            playerStatsManager.RegenerateStamina();

            DebugMenu();
        }

        protected override void LateUpdate()
        {
            if (!IsOwner) 
            {
                return;
            }
            base.LateUpdate();

            PlayerCamera.instance.HandleAllCameraActions();
        }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnEnable();
        }

        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallback;

            if (IsOwner) 
            {
                PlayerCamera.instance.player = this;
                PlayerInputManager.instance.player = this;
                WorldSaveGameManager.instance.player = this;
                PlayerUIManager.instance.localPlayer = this;

                //update the total amount of health or stamina when the stat linked to either changes
                playerNetworkManager.vigor.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;
                playerNetworkManager.mind.OnValueChanged += playerNetworkManager.SetNewMaxManaValue;

                //Updates ui stat bars when a stat changes (health or stamina)
                playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentMana.OnValueChanged += PlayerUIManager.instance.playerUIHudManager.SetNewManaValue;
                playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
            }

            if (!IsOwner)
                characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPChanged;

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;
            playerNetworkManager.maxMana.OnValueChanged += playerNetworkManager.OnMaxManaChanged;
            playerNetworkManager.currentMana.OnValueChanged += playerNetworkManager.OnManaChanged;

            // Lock on
            playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

            // Equipment
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            playerNetworkManager.currentQuickSlotID.OnValueChanged += playerNetworkManager.OnCurrentQuickSlotIDChange;
            playerNetworkManager.isChugging.OnValueChanged += playerNetworkManager.OnIsChuggingChanged;
            playerNetworkManager.currentSpellID.OnValueChanged += playerNetworkManager.OnCurrentSpellIDChange;
            playerNetworkManager.isBlocking.OnValueChanged += playerNetworkManager.OnIsBlockingChanged;
            playerNetworkManager.headEquipmentID.OnValueChanged += playerNetworkManager.OnHeadEquipmentChanged;
            playerNetworkManager.bodyEquipmentID.OnValueChanged += playerNetworkManager.OnBodyEquipmentChanged;
            playerNetworkManager.legEquipmentID.OnValueChanged += playerNetworkManager.OnLegEquipmentChanged;
            playerNetworkManager.handEquipmentID.OnValueChanged += playerNetworkManager.OnHandEquipmentChanged;

            //Two Hand
            playerNetworkManager.isTwoHandingWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingWeaponChanged;
            playerNetworkManager.isTwoHandingRightWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingRightWeaponChanged;
            playerNetworkManager.isTwoHandingLeftWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingLeftWeaponChanged;

            //Flags
            playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackChanged;

            // Upon connecting, if we are the owner of this character, but we are not the server, reload our character data to this newly instantiated character
            //Don.t run this if we are the server, because since they are the host, they are already loaded in and don.t need to reload their data
            if (IsOwner && !IsServer) 
            {
                LoadGameDataToCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
            }
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnectedCallback;

            if (IsOwner)
            {
                //update the total amount of health or stamina when the stat linked to either changes
                playerNetworkManager.vigor.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
                playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue; 
                playerNetworkManager.mind.OnValueChanged -= playerNetworkManager.SetNewMaxManaValue;

                //Updates ui stat bars when a stat changes (health or stamina)
                playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewHealthValue;
                playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewStaminaValue;
                playerNetworkManager.currentMana.OnValueChanged -= PlayerUIManager.instance.playerUIHudManager.SetNewManaValue;
                playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
            }

            //Only update floating HP bar if this character is not the local player character (You don.t want to see the hp bar is your own head)
            if (!IsOwner)
                characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPChanged;

            // Stats
            playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;
            playerNetworkManager.maxMana.OnValueChanged -= playerNetworkManager.OnMaxManaChanged;
            playerNetworkManager.currentMana.OnValueChanged -= playerNetworkManager.OnManaChanged;

            // Lock on
            playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
            playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

            // Equipment
            playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
            playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
            playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
            playerNetworkManager.currentSpellID.OnValueChanged -= playerNetworkManager.OnCurrentSpellIDChange;
            playerNetworkManager.currentQuickSlotID.OnValueChanged -= playerNetworkManager.OnCurrentQuickSlotIDChange;
            playerNetworkManager.isChugging.OnValueChanged -= playerNetworkManager.OnIsChuggingChanged;
            playerNetworkManager.headEquipmentID.OnValueChanged -= playerNetworkManager.OnHeadEquipmentChanged;
            playerNetworkManager.bodyEquipmentID.OnValueChanged -= playerNetworkManager.OnBodyEquipmentChanged;
            playerNetworkManager.legEquipmentID.OnValueChanged -= playerNetworkManager.OnLegEquipmentChanged;
            playerNetworkManager.handEquipmentID.OnValueChanged -= playerNetworkManager.OnHandEquipmentChanged;

            //Two Hand
            playerNetworkManager.isTwoHandingWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingWeaponChanged;
            playerNetworkManager.isTwoHandingRightWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingRightWeaponChanged;
            playerNetworkManager.isTwoHandingLeftWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingLeftWeaponChanged;

            //Flags
            playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackChanged;
        }

        private void OnClientConnectedCallback(ulong clientID) 
        {
            WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

            //If we are the server, we are the host, so we dont need to load players to sync then
            //You only need to load other players gear to sync it if you join a game thats already been active without you being present
            if (!IsServer && IsOwner) 
            {
                foreach (var player in WorldGameSessionManager.instance.players) 
                {
                    if (player != this) 
                    {
                        player.LoadOtherPlayerCharacterWhenJoiningServer();
                    }
                }
            }
        }

        public override void ReviveCharacter()
        {
            base.ReviveCharacter();

            if (IsOwner) 
            {
                isDead.Value = false;
                respawnCharacter = false;
                playerNetworkManager.currentHealth.Value = playerNetworkManager.maxHealth.Value;
                playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;
                //Restore focus points

                //Play Rebirth Effects
                playerAnimatorManager.PlayTargetActionAnimation("Empty", false);
            }
        }

        public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
        {
            if (IsOwner) 
            {
                PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
            }

            //Check for players that are alive, if 0 respawn characters
            WorldGameSessionManager.instance.WaitThenReviveHost();

            return base.ProcessDeathEvent(manuallySelectDeathAnimation);
        }

        public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            currentCharacterData.sceneIndex = SceneManager.GetActiveScene().buildIndex;
            currentCharacterData.characterName = playerNetworkManager.characterName.Value.ToString();

            currentCharacterData.yPosition = transform.position.y;
            currentCharacterData.xPosition = transform.position.x;
            currentCharacterData.zPosition = transform.position.z;

            currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
            currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;
            currentCharacterData.currentMana = playerNetworkManager.currentMana.Value;
            currentCharacterData.runes = playerStatsManager.runes;

            currentCharacterData.vitality = playerNetworkManager.vigor.Value;
            currentCharacterData.endurance = playerNetworkManager.endurance.Value;
            currentCharacterData.mind = playerNetworkManager.mind.Value;
            currentCharacterData.strength = playerNetworkManager.strength.Value;
            currentCharacterData.dexterity = playerNetworkManager.dexterity.Value;
            currentCharacterData.intelligence = playerNetworkManager.intelligence.Value;
            currentCharacterData.faith = playerNetworkManager.faith.Value;
            currentCharacterData.currentHealthPosionRemaining = playerNetworkManager.remainingHealthPoison.Value;
            currentCharacterData.currentManaPosionRemaining = playerNetworkManager.remainingManaPoison.Value;

            currentCharacterData.headEquipment = playerNetworkManager.headEquipmentID.Value;
            currentCharacterData.bodyEquipment = playerNetworkManager.bodyEquipmentID.Value;
            currentCharacterData.legEquipment = playerNetworkManager.legEquipmentID.Value;
            currentCharacterData.handEquipment = playerNetworkManager.handEquipmentID.Value;

            currentCharacterData.rightWeaponIndex = playerInventoryManager.rightHandWeaponIndex;
            currentCharacterData.rightWeapon01 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInRightHandSlots[0]); //This should never be null
            currentCharacterData.rightWeapon02 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInRightHandSlots[1]);//Default should be the unarmed
            currentCharacterData.rightWeapon03 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInRightHandSlots[2]);


            currentCharacterData.leftWeaponIndex = playerInventoryManager.leftHandWeaponIndex;
            currentCharacterData.leftWeapon01 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInLeftHandSlots[0]); //This should never be null
            currentCharacterData.leftWeapon02 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInLeftHandSlots[1]);//Default should be the unarmed
            currentCharacterData.leftWeapon03 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponsInLeftHandSlots[2]);

            currentCharacterData.quickSlotIndex = playerInventoryManager.quickSlotItemIndex;
            currentCharacterData.quickSlotItem01 = WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(playerInventoryManager.quickSlotItemsInQuickSlots[0]);
            currentCharacterData.quickSlotItem02 = WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(playerInventoryManager.quickSlotItemsInQuickSlots[1]);
            currentCharacterData.quickSlotItem03 = WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(playerInventoryManager.quickSlotItemsInQuickSlots[2]);

            if (playerInventoryManager.currentSpell != null)
                currentCharacterData.currentSpell = playerInventoryManager.currentSpell.itemID;

            //Clear List Before Save
            currentCharacterData.weaponsInInventory = new List<SerializableWeapon>();
            currentCharacterData.quickSlotItemsInInventory = new List<SerializableQuickSlotItem>();
            currentCharacterData.headEquipmentInInventory = new List<int>();
            currentCharacterData.bodyEquipmentInInventory = new List<int>();
            currentCharacterData.legEquipmentInInventory = new List<int>();
            currentCharacterData.handEquipmentInInventory = new List<int>();

            for (int i = 0; i < playerInventoryManager.itemInIvnventory.Count; i++) 
            {
                if (playerInventoryManager.itemInIvnventory[i] == null)
                    continue;

                WeaponItem weaponInInventory = playerInventoryManager.itemInIvnventory[i] as WeaponItem;
                HeadEquipmentItem headEquipmentInInventory = playerInventoryManager.itemInIvnventory[i] as HeadEquipmentItem;
                BodyEquipmentItem bodyEquipmentInInventory = playerInventoryManager.itemInIvnventory[i] as BodyEquipmentItem;
                LegEquipmentItem legEquipmentInInventory = playerInventoryManager.itemInIvnventory[i] as LegEquipmentItem;
                HandEquipmentItem handEquipmentInInventory = playerInventoryManager.itemInIvnventory[i] as HandEquipmentItem;
                QuickSlotItem quickSlotItemInInventory = playerInventoryManager.itemInIvnventory[i] as QuickSlotItem;

                if (weaponInInventory != null)
                    currentCharacterData.weaponsInInventory.Add(WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(weaponInInventory));

                if (headEquipmentInInventory != null)
                    currentCharacterData.headEquipmentInInventory.Add(headEquipmentInInventory.itemID);

                if (bodyEquipmentInInventory != null)
                    currentCharacterData.bodyEquipmentInInventory.Add(bodyEquipmentInInventory.itemID);

                if (legEquipmentInInventory != null)
                    currentCharacterData.legEquipmentInInventory.Add(legEquipmentInInventory.itemID);

                if (handEquipmentInInventory != null)
                    currentCharacterData.handEquipmentInInventory.Add(handEquipmentInInventory.itemID);

                if (quickSlotItemInInventory != null)
                    currentCharacterData.quickSlotItemsInInventory.Add(WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(quickSlotItemInInventory));
            }

        }

        public void LoadGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
        {
            playerNetworkManager.characterName.Value = currentCharacterData.characterName;
            Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
            transform.position = myPosition;

            //Stats
            playerNetworkManager.vigor.Value = currentCharacterData.vitality;
            playerNetworkManager.endurance.Value = currentCharacterData.endurance;
            playerNetworkManager.mind.Value = currentCharacterData.mind;
            playerNetworkManager.strength.Value = currentCharacterData.strength;
            playerNetworkManager.dexterity.Value = currentCharacterData.dexterity;
            playerNetworkManager.intelligence.Value = currentCharacterData.intelligence;
            playerNetworkManager.faith.Value = currentCharacterData.faith;

            playerNetworkManager.maxHealth.Value = playerStatsManager.CalculateHealthBasedOnVitalityLevel(playerNetworkManager.vigor.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(playerNetworkManager.maxHealth.Value);
            playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(playerNetworkManager.endurance.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);
            playerNetworkManager.maxMana.Value = playerStatsManager.CalculateManaBasedOnMindLevel(playerNetworkManager.mind.Value);
            PlayerUIManager.instance.playerUIHudManager.SetMaxManaValue(playerNetworkManager.maxMana.Value);

            playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
            playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;
            playerNetworkManager.currentMana.Value = currentCharacterData.currentMana;
            playerStatsManager.runes = currentCharacterData.runes;
            PlayerUIManager.instance.playerUIHudManager.SetRunesCount(currentCharacterData.runes);

            playerNetworkManager.remainingHealthPoison.Value = currentCharacterData.currentHealthPosionRemaining;
            playerNetworkManager.remainingManaPoison.Value = currentCharacterData.currentManaPosionRemaining;

            //Equipment

            if (WorldItemDataBase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipment))
            {
                HeadEquipmentItem headEquipment = Instantiate(WorldItemDataBase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipment));
                playerInventoryManager.headEquipment = headEquipment;
            }
            else 
            {
                playerInventoryManager.headEquipment = null;
            }

            if (WorldItemDataBase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipment))
            {
                BodyEquipmentItem bodyEquipment = Instantiate(WorldItemDataBase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipment));
                playerInventoryManager.bodyEquipment = bodyEquipment;
            }
            else
            {
                playerInventoryManager.bodyEquipment = null;
            }

            if (WorldItemDataBase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipment))
            {
                LegEquipmentItem legEquipment = Instantiate(WorldItemDataBase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipment));
                playerInventoryManager.legEquipment = legEquipment;
            }
            else
            {
                playerInventoryManager.legEquipment = null;
            }

            if (WorldItemDataBase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipment))
            {
                HandEquipmentItem handEquipment = Instantiate(WorldItemDataBase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipment));
                playerInventoryManager.handEquipment = handEquipment;
            }
            else
            {
                playerInventoryManager.legEquipment = null;
            }

            //Weapons
            playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightWeaponIndex;
            playerInventoryManager.weaponsInRightHandSlots[0] = currentCharacterData.rightWeapon01.GetWeapon();
            playerInventoryManager.weaponsInRightHandSlots[1] = currentCharacterData.rightWeapon02.GetWeapon();
            playerInventoryManager.weaponsInRightHandSlots[2] = currentCharacterData.rightWeapon03.GetWeapon();

            playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftWeaponIndex;
            playerInventoryManager.weaponsInLeftHandSlots[0] = currentCharacterData.leftWeapon01.GetWeapon();
            playerInventoryManager.weaponsInLeftHandSlots[1] = currentCharacterData.leftWeapon02.GetWeapon();
            playerInventoryManager.weaponsInLeftHandSlots[2] = currentCharacterData.leftWeapon03.GetWeapon();

            //Quick Slot Items
            playerInventoryManager.quickSlotItemIndex = currentCharacterData.quickSlotIndex;
            playerInventoryManager.quickSlotItemsInQuickSlots[0] = currentCharacterData.quickSlotItem01.GetQuickSlotItem();
            playerInventoryManager.quickSlotItemsInQuickSlots[1] = currentCharacterData.quickSlotItem02.GetQuickSlotItem();
            playerInventoryManager.quickSlotItemsInQuickSlots[2] = currentCharacterData.quickSlotItem03.GetQuickSlotItem();

            //This Refreshes the Hud
            playerEquipmentManager.LoadQuickSlotEquipment(playerInventoryManager.quickSlotItemsInQuickSlots[playerInventoryManager.quickSlotItemIndex]);


            if (currentCharacterData.rightWeaponIndex >= 0)
            {
                playerInventoryManager.currentRightHandWeapon = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightWeaponIndex];
                playerNetworkManager.currentRightHandWeaponID.Value = playerInventoryManager.weaponsInRightHandSlots[currentCharacterData.rightWeaponIndex].itemID;
            }
            else 
            {
                playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
            }

            if (currentCharacterData.leftWeaponIndex >= 0)
            {
                playerInventoryManager.currentLeftHandWeapon = playerInventoryManager.weaponsInLeftHandSlots[currentCharacterData.leftWeaponIndex];
                playerNetworkManager.currentLeftHandWeaponID.Value = playerInventoryManager.weaponsInLeftHandSlots[currentCharacterData.leftWeaponIndex].itemID;
            }
            else
            {
                playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDataBase.Instance.unarmedWeapon.itemID;
            }

            if (WorldItemDataBase.Instance.GetSpellByID(currentCharacterData.currentSpell))
            {
                SpellItem currentSpell = Instantiate(WorldItemDataBase.Instance.GetSpellByID(currentCharacterData.currentSpell));
                playerNetworkManager.currentSpellID.Value = currentSpell.itemID;
            }
            else
            {
                playerNetworkManager.currentSpellID.Value = -1; //-1 sets spell to null as its not a valid id
            }

            for (int i = 0; i < currentCharacterData.weaponsInInventory.Count; i++) 
            {
                WeaponItem weapon = currentCharacterData.weaponsInInventory[i].GetWeapon();
                playerInventoryManager.AddItemToInventory(weapon);
            }

            for (int i = 0; i < currentCharacterData.headEquipmentInInventory.Count; i++) 
            {
                EquipmentItem equipment = WorldItemDataBase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipmentInInventory[i]);
                playerInventoryManager.AddItemToInventory(equipment);
            }

            for (int i = 0; i < currentCharacterData.bodyEquipmentInInventory.Count; i++)
            {
                EquipmentItem equipment = WorldItemDataBase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipmentInInventory[i]);
                playerInventoryManager.AddItemToInventory(equipment);
            }

            for (int i = 0; i < currentCharacterData.legEquipmentInInventory.Count; i++)
            {
                EquipmentItem equipment = WorldItemDataBase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipmentInInventory[i]);
                playerInventoryManager.AddItemToInventory(equipment);
            }

            for (int i = 0; i < currentCharacterData.handEquipmentInInventory.Count; i++)
            {
                EquipmentItem equipment = WorldItemDataBase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipmentInInventory[i]);
                playerInventoryManager.AddItemToInventory(equipment);
            }

            for (int i = 0; i < currentCharacterData.quickSlotItemsInInventory.Count; i++)
            {
                QuickSlotItem quickSlotItem = currentCharacterData.quickSlotItemsInInventory[i].GetQuickSlotItem();
                playerInventoryManager.AddItemToInventory(quickSlotItem);
            }


            playerEquipmentManager.EquipArmor();

        }

        public void LoadOtherPlayerCharacterWhenJoiningServer() 
        {
            //Sync Weapons
            playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
            playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);
            playerNetworkManager.OnCurrentSpellIDChange(0, playerNetworkManager.currentSpellID.Value);

            //Sync Armor
            playerNetworkManager.OnHeadEquipmentChanged(0, playerNetworkManager.headEquipmentID.Value);
            playerNetworkManager.OnBodyEquipmentChanged(0, playerNetworkManager.bodyEquipmentID.Value);
            playerNetworkManager.OnLegEquipmentChanged(0, playerNetworkManager.legEquipmentID.Value);
            playerNetworkManager.OnHandEquipmentChanged(0, playerNetworkManager.handEquipmentID.Value);

            //Sync Two Hand Status
            playerNetworkManager.OnIsTwoHandingRightWeaponChanged(false, playerNetworkManager.isTwoHandingRightWeapon.Value);
            playerNetworkManager.OnIsTwoHandingLeftWeaponChanged(false, playerNetworkManager.isTwoHandingLeftWeapon.Value);

            //Sync Block Status
            playerNetworkManager.OnIsBlockingChanged(false, playerNetworkManager.isBlocking.Value);

            //Armor

            //Lock On
            if (playerNetworkManager.isLockedOn.Value) 
            {
                playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
            }
        }

        private void DebugMenu() 
        {
            if (respawnCharacter) 
            {
                ReviveCharacter();
            }

            if (switchRightWeapon) 
            {
                switchRightWeapon = false;
                playerEquipmentManager.SwitchRightWeapon();
            }
        }
    }
}
