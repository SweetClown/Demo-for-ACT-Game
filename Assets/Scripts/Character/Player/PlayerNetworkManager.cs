using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

namespace SweetClown
{
    public class PlayerNetworkManager : CharacterNetworkManager
    {
        PlayerManager player;
        public NetworkVariable<FixedString64Bytes> characterName = new NetworkVariable<FixedString64Bytes>("Character",
                                                                                                        NetworkVariableReadPermission.Everyone,
                                                                                                        NetworkVariableWritePermission.Owner);

        [Header("Poisons")]
        public NetworkVariable<int> remainingHealthPoison = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> remainingManaPoison = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone,
                                                                                 NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isChugging = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                                                                 NetworkVariableWritePermission.Owner);


        [Header("Equipment")]
        public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, 
                                                                                         NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentRightHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, 
                                                                                           NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentLeftHandWeaponID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, 
                                                                                          NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentSpellID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
                                                                                          NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> currentQuickSlotID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
                                                                                  NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);

        [Header("Two Handing")]
        public NetworkVariable<int> currentWeaponBeingTwoHanded = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);

        public NetworkVariable<bool> isTwoHandingWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isTwoHandingRightWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);
        public NetworkVariable<bool> isTwoHandingLeftWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,
                                                                                         NetworkVariableWritePermission.Owner);

        [Header("Armor")]
        public NetworkVariable<bool> isMale = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> headEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> bodyEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> legEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
        public NetworkVariable<int> handEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);


        protected override void Awake()
        {
            base.Awake();
            player = GetComponent<PlayerManager>();
        }

        public void SetCharacterActionHand(bool rightHandedAction) 
        {
            if (rightHandedAction) 
            {
                isUsingLeftHand.Value = false;
                isUsingRightHand.Value = true;
            }
            else
            {
                isUsingLeftHand.Value = true;
                isUsingRightHand.Value = false;
            }
        }

        public override void OnIsDeadChanged(bool oldStatus, bool newStatus)
        {
            base.OnIsDeadChanged(oldStatus, newStatus);

            if (player.isDead.Value) 
            {
                player.playerCombatManager.CreateDeadSpot(player.transform.position, player.playerStatsManager.runes);
            }
        }

        public void SetNewMaxHealthValue(int oldVitality, int newVitality) 
        {
            maxHealth.Value = player.playerStatsManager.CalculateHealthBasedOnVitalityLevel(newVitality);
            PlayerUIManager.instance.playerUIHudManager.SetMaxHealthValue(maxHealth.Value);
            currentHealth.Value = maxHealth.Value;
        }

        public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
        {
            maxStamina.Value = player.playerStatsManager.CalculateStaminaBasedOnEnduranceLevel(newEndurance);
            PlayerUIManager.instance.playerUIHudManager.SetMaxStaminaValue(maxStamina.Value);
            currentStamina.Value = maxStamina.Value;
        }

        public void SetNewMaxManaValue(int oldMind, int newMind)
        {
            maxMana.Value = player.playerStatsManager.CalculateManaBasedOnMindLevel(newMind);
            PlayerUIManager.instance.playerUIHudManager.SetMaxManaValue(maxMana.Value);
            currentMana.Value = maxMana.Value;
        }

        public void OnCurrentRightHandWeaponIDChange(int oldID, int newID) 
        {
            if (!player.IsOwner) 
            {
                WeaponItem newWeapon = Instantiate(WorldItemDataBase.Instance.GetWeaponByID(newID));
                player.playerInventoryManager.currentRightHandWeapon = newWeapon;
            }

            player.playerEquipmentManager.LoadRightWeapon();

            if (player.IsOwner) 
            {
                PlayerUIManager.instance.playerUIHudManager.SetRightWeaponQuickSlotIcon(newID);
            }
        }

        public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
        {
            if (!player.IsOwner)
            {
                WeaponItem newWeapon = Instantiate(WorldItemDataBase.Instance.GetWeaponByID(newID));
                player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
            }

            player.playerEquipmentManager.LoadLeftWeapon();

            if (player.IsOwner)
            {
                PlayerUIManager.instance.playerUIHudManager.SetLeftWeaponQuickSlotIcon(newID);
            }
        }

        public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
        {
            WeaponItem newWeapon = Instantiate(WorldItemDataBase.Instance.GetWeaponByID(newID));
            player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

            //We don.t need to run this code if we are the owner because we are already done so locally
            if (player.IsOwner)
                return;

            if (player.playerCombatManager.currentWeaponBeingUsed != null)
                player.playerAnimatorManager.UpdateAnimatorController(player.playerCombatManager.currentWeaponBeingUsed.weaponAnimator);
        }

        public void OnCurrentSpellIDChange(int oldID, int newID)
        {
            SpellItem newSpell = null;

            if (WorldItemDataBase.Instance.GetSpellByID(newID))
                newSpell = Instantiate(WorldItemDataBase.Instance.GetSpellByID(newID));

            if (newSpell != null)
            {
                player.playerInventoryManager.currentSpell = newSpell;

                if (player.IsOwner)
                    PlayerUIManager.instance.playerUIHudManager.SetSpellQuickSlotIcon(newID);
            }
        }

        public void OnCurrentQuickSlotIDChange(int oldID, int newID)
        {
            QuickSlotItem quickSlotItem = null;

            if (WorldItemDataBase.Instance.GetQuickSlotItemByID(newID))
                quickSlotItem = Instantiate(WorldItemDataBase.Instance.GetQuickSlotItemByID(newID));

            if (quickSlotItem != null)
            {
                player.playerInventoryManager.currentQuickSlotItem = quickSlotItem;
            }
            else 
            {
                player.playerInventoryManager.currentQuickSlotItem = null;
            }

            if (player.IsOwner)
                PlayerUIManager.instance.playerUIHudManager.SetQuickSlotItemQuickSlotIcon(player.playerInventoryManager.currentQuickSlotItem);
        }

        public void OnMaxManaChanged(int oldMana, int newMana)
        {
            if (player.IsOwner)
                PlayerUIManager.instance.playerUIHudManager.SetMaxManaValue(newMana);
        }

        public void OnManaChanged(int oldMana, int newMana) 
        {
            if (player.IsOwner)
                PlayerUIManager.instance.playerUIHudManager.SetNewManaValue(oldMana, newMana);
        }

        public override void OnIsBlockingChanged(bool old, bool newStatus)
        {
            base.OnIsBlockingChanged(old, newStatus);

            if (IsOwner) 
            {
                player.playerStatsManager.blockingPhysicalAbsorption = player.playerCombatManager.currentWeaponBeingUsed.physicalBaseDamageAbsorption;
                player.playerStatsManager.blockingFireAbsorption = player.playerCombatManager.currentWeaponBeingUsed.fireBaseDamageAbsorption;
                player.playerStatsManager.blockingMagicAbsorption = player.playerCombatManager.currentWeaponBeingUsed.magicBaseDamageAbsorption;
                player.playerStatsManager.blockingHolyAbsorption = player.playerCombatManager.currentWeaponBeingUsed.holyBaseDamageAbsorption;
                player.playerStatsManager.blockingLightingAbsorption = player.playerCombatManager.currentWeaponBeingUsed.lightingBaseDamageAbsorption;
                player.playerStatsManager.blockingStability = player.playerCombatManager.currentWeaponBeingUsed.stability;
            }
        }

        public void OnIsTwoHandingWeaponChanged(bool oldStatus, bool newStatus) 
        {
            if (!isTwoHandingWeapon.Value)
            {
                if (IsOwner)
                {
                    isTwoHandingLeftWeapon.Value = false;
                    isTwoHandingRightWeapon.Value = false;
                }

                player.playerEquipmentManager.UnTwoHandWeapon();
                player.playerEffectsManager.RemoveStaticEffect(WorldCharacterEffectsManager.instance.twoHandEffect.staticEffectID);
            }
            else 
            {
                StaticCharacterEffect twoHandEffect = Instantiate(WorldCharacterEffectsManager.instance.twoHandEffect);
                player.playerEffectsManager.AddStaticEffect(twoHandEffect);
            }

            player.animator.SetBool("isTwoHandingWeapon", isTwoHandingWeapon.Value);
        }

        public void OnIsTwoHandingRightWeaponChanged(bool oldStatus, bool newStatus) 
        {
            if (!isTwoHandingRightWeapon.Value)
                return;

            if (IsOwner) 
            {
                currentWeaponBeingTwoHanded.Value = currentRightHandWeaponID.Value;
                isTwoHandingWeapon.Value = true;
            }

            player.playerInventoryManager.currentTwoHandWeapon = player.playerInventoryManager.currentRightHandWeapon;
            player.playerEquipmentManager.TwoHandRightWeapon();
        }

        public void OnIsTwoHandingLeftWeaponChanged(bool oldStatus, bool newStatus)
        {
            if (!isTwoHandingLeftWeapon.Value)
                return;

            if (IsOwner)
            {
                currentWeaponBeingTwoHanded.Value = currentLeftHandWeaponID.Value;
                isTwoHandingWeapon.Value = true;
            }

            player.playerInventoryManager.currentTwoHandWeapon = player.playerInventoryManager.currentLeftHandWeapon;
            player.playerEquipmentManager.TwoHandLeftWeapon();
        }

        public void OnIsChuggingChanged(bool oldStatus, bool newStatus) 
        {
            player.animator.SetBool("isChuggingPoison", isChugging.Value);
        }

        public void OnHeadEquipmentChanged(int oldValue, int newValue) 
        {
            //We already run the logic on the owners side, so theres no point in running it again
            if (IsOwner)
                return;

            HeadEquipmentItem equipment = WorldItemDataBase.Instance.GetHeadEquipmentByID(headEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadHeadEquipment(Instantiate(equipment));
            }
            else 
            {
                player.playerEquipmentManager.LoadHeadEquipment(null);
            }
        }

        public void OnBodyEquipmentChanged(int oldValue, int newValue)
        {
            //We already run the logic on the owners side, so theres no point in running it again
            if (IsOwner)
                return;

            BodyEquipmentItem equipment = WorldItemDataBase.Instance.GetBodyEquipmentByID(bodyEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadBodyEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadBodyEquipment(null);
            }
        }

        public void OnLegEquipmentChanged(int oldValue, int newValue)
        {
            //We already run the logic on the owners side, so theres no point in running it again
            if (IsOwner)
                return;

            LegEquipmentItem equipment = WorldItemDataBase.Instance.GetLegEquipmentByID(legEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadLegEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadLegEquipment(null);
            }
        }

        public void OnHandEquipmentChanged(int oldValue, int newValue)
        {
            //We already run the logic on the owners side, so theres no point in running it again
            if (IsOwner)
                return;

            HandEquipmentItem equipment = WorldItemDataBase.Instance.GetHandEquipmentByID(handEquipmentID.Value);

            if (equipment != null)
            {
                player.playerEquipmentManager.LoadHandEquipment(Instantiate(equipment));
            }
            else
            {
                player.playerEquipmentManager.LoadHandEquipment(null);
            }
        }


        [ServerRpc]
        public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID) 
        {
            if (IsServer) 
            {
                NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
            }
        }

        [ClientRpc]
        private void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
        {
            // We do not play the action again for the character who called it , because they already played it on local
            if (clientID != NetworkManager.Singleton.LocalClientId) 
            {
                PerformWeaponBaseAction(actionID, weaponID);
            }
        }

        private void PerformWeaponBaseAction(int actionID, int weaponID) 
        {
            WeaponItemAction weaponAction = WorldActionManager.instance.GetWeaponItemActionByID(actionID);

            if (weaponAction != null)
            {
                weaponAction.AttemptToPerfromAction(player, WorldItemDataBase.Instance.GetWeaponByID(weaponID));
            }
            else 
            {
                Debug.LogError("Action is null, cannot perform");
            }
        }

        [ServerRpc]
        public void HideWeaponsServerRpc() 
        {
            if (IsServer)
                HideWeaponsClientRpc();
        }

        [ClientRpc]
        private void HideWeaponsClientRpc()
        {
            if (player.playerEquipmentManager.rightHandWeaponModel != null)
            {
                player.playerEquipmentManager.rightHandWeaponModel.SetActive(false);
            }

            if (player.playerEquipmentManager.leftHandWeaponModel != null)
            {
                player.playerEquipmentManager.leftHandWeaponModel.SetActive(false);
            }
        }

        [ServerRpc]
        public void NotifyServerOfQuickSlotItemActionServerRpc(ulong clientID, int quickSlotItemID) 
        {
            NotifyServerOfQuickSlotItemActionClientRpc(clientID, quickSlotItemID);
        }

        [ClientRpc]
        public void NotifyServerOfQuickSlotItemActionClientRpc(ulong clientID, int quickSlotItemID)
        {
            if (clientID != NetworkManager.Singleton.LocalClientId) 
            {
                QuickSlotItem item = WorldItemDataBase.Instance.GetQuickSlotItemByID(quickSlotItemID);
                item.AttemptToUseItem(player);
            }
        }

    }
}
