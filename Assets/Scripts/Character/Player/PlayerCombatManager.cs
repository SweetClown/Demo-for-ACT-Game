using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System;

namespace SG
{
    public class PlayerCombatManager : CharacterCombatManager
    {
        PlayerManager player;

        public WeaponItem currentWeaponBeingUsed;

        [Header("Flags")]
        public bool canComboWithMainHandWeapon = false;
        public bool isUsingItem = false;
        //public bool canComboWithOffHandWeapon = false;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
        }

        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            
            SceneManager.activeSceneChanged -= OnSceneChanged;
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            //Only create this if the world scene loads

            //Dead Spot
            if (WorldSaveGameManager.instance.currentCharacterData.hasDeadSpot) 
            {
                Vector3 deadSpotPosition = new Vector3(WorldSaveGameManager.instance.currentCharacterData.deadSpotPositionX,
                                                       WorldSaveGameManager.instance.currentCharacterData.deadSpotPositionY,
                                                       WorldSaveGameManager.instance.currentCharacterData.deadSpotPositionZ);

                CreateDeadSpot(deadSpotPosition, WorldSaveGameManager.instance.currentCharacterData.deadSpotRuneCount, false);
            }
        }

        public void CreateDeadSpot(Vector3 position, int runesCount, bool removePlayersRunes = true) 
        {
            if (!player.IsHost)
                return;

            //Spawn The Dead Spot VFX
            GameObject deadSpotFX = Instantiate(WorldCharacterEffectsManager.instance.DeadSpotVFX);
            deadSpotFX.GetComponent<NetworkObject>().Spawn();

            //Set its position
            deadSpotFX.transform.position = position;

            //Set the rune count
            PickUpRunesInteractable pickUpRunes = deadSpotFX.GetComponent<PickUpRunesInteractable>();
            pickUpRunes.runeCount = runesCount;

            if (removePlayersRunes)
                player.playerStatsManager.AddRunes(-player.playerStatsManager.runes);

            WorldSaveGameManager.instance.currentCharacterData.hasDeadSpot = true;
            WorldSaveGameManager.instance.currentCharacterData.deadSpotRuneCount = pickUpRunes.runeCount;
            WorldSaveGameManager.instance.currentCharacterData.deadSpotPositionX = position.x;
            WorldSaveGameManager.instance.currentCharacterData.deadSpotPositionY = position.y;
            WorldSaveGameManager.instance.currentCharacterData.deadSpotPositionZ = position.z;
        }

        public void PerformWeaponBaseAction(WeaponItemAction weaponAction, WeaponItem weaponPerformingAction) 
        {
            if (player.IsOwner) 
            {
                //Perform the action
                weaponAction.AttemptToPerfromAction(player, weaponPerformingAction);

                //Notify the server we have performed the action, so we perform it from there perspective also
                player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId, 
                                                                                   weaponAction.actionID, 
                                                                                   weaponPerformingAction.itemID);

            }


        }

        public override void CloseAllDamageColliders()
        {
            base.CloseAllDamageColliders();

            player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
            player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }

        public override void AttempRiposte(RaycastHit hit)
        {
            CharacterManager targetCharacter = hit.transform.gameObject.GetComponent<CharacterManager>();

            if (targetCharacter == null)
                return;

            if (!targetCharacter.characterNetworkManager.isRipostable.Value)
                return;

            if (targetCharacter.characterNetworkManager.isBeingCriticallyDamage.Value)
                return;

            //You can only riposte with a melee weapon item
            MeleeWeaponItem riposteWeapon;
            MeleeWeaponDamageCollider riposteCollider;

            if (player.playerNetworkManager.isTwoHandingLeftWeapon.Value)
            {
                riposteWeapon = player.playerInventoryManager.currentLeftHandWeapon as MeleeWeaponItem;
                riposteCollider = player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider;
            }
            else
            {
                riposteWeapon = player.playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
                riposteCollider = player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider;
            }

            character.characterAnimatorManager.PlayTargetActionAnimationInstantly("Riposte_01", true);

            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;

            TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeCriticalDamageEffect);

            //Apply all of the damage stats from the collider to the damage effect
            damageEffect.physicalDamage = riposteCollider.physicalDamage;
            damageEffect.holyDamage = riposteCollider.holyDamage;
            damageEffect.fireDamage = riposteCollider.fireDamage;
            damageEffect.lightningDamage = riposteCollider.lightningDamage;
            damageEffect.magicDamage = riposteCollider.magicDamage;
            damageEffect.poiseDamage = riposteCollider.poiseDamage;

            //Multiply damage by weapons riposte modifier
            damageEffect.physicalDamage *= riposteWeapon.riposte_Attack_01_Modifier;
            damageEffect.holyDamage *= riposteWeapon.riposte_Attack_01_Modifier;
            damageEffect.fireDamage *= riposteWeapon.riposte_Attack_01_Modifier;
            damageEffect.lightningDamage *= riposteWeapon.riposte_Attack_01_Modifier;
            damageEffect.magicDamage *= riposteWeapon.riposte_Attack_01_Modifier;
            damageEffect.poiseDamage *= riposteWeapon.riposte_Attack_01_Modifier;

            targetCharacter.characterNetworkManager.NotifyTheServerOfRiposteServerRpc(
                targetCharacter.NetworkObjectId, 
                character.NetworkObjectId,
                "Riposted_01",
                riposteWeapon.itemID,
                damageEffect.physicalDamage,
                damageEffect.holyDamage,
                damageEffect.fireDamage,
                damageEffect.lightningDamage,
                damageEffect.magicDamage,
                damageEffect.poiseDamage);

        }

        public override void AttempBackstab(RaycastHit hit)
        {
            CharacterManager targetCharacter = hit.transform.gameObject.GetComponent<CharacterManager>();

            if (targetCharacter == null)
                return;

            if (!targetCharacter.characterCombatManager.canBeBackstabbed)
                return;

            if (targetCharacter.characterNetworkManager.isBeingCriticallyDamage.Value)
                return;

            //You can only backSteb with a melee weapon item
            MeleeWeaponItem backStebWeapon;
            MeleeWeaponDamageCollider backStebCollider;

            if (player.playerNetworkManager.isTwoHandingLeftWeapon.Value)
            {
                backStebWeapon = player.playerInventoryManager.currentLeftHandWeapon as MeleeWeaponItem;
                backStebCollider = player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider;
            }
            else 
            {
                backStebWeapon = player.playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
                backStebCollider = player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider;
            }

            character.characterAnimatorManager.PlayTargetActionAnimationInstantly("Backstab_01", true);

            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;

            TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeCriticalDamageEffect);

            //Apply all of the damage stats from the collider to the damage effect
            damageEffect.physicalDamage = backStebCollider.physicalDamage;
            damageEffect.holyDamage = backStebCollider.holyDamage;
            damageEffect.fireDamage = backStebCollider.fireDamage;
            damageEffect.lightningDamage = backStebCollider.lightningDamage;
            damageEffect.magicDamage = backStebCollider.magicDamage;
            damageEffect.poiseDamage = backStebCollider.poiseDamage;

            //Multiply damage by weapons riposte modifier
            damageEffect.physicalDamage *= backStebWeapon.backstab_Attack_01_Modifier;
            damageEffect.holyDamage *= backStebWeapon.backstab_Attack_01_Modifier;
            damageEffect.fireDamage *= backStebWeapon.backstab_Attack_01_Modifier;
            damageEffect.lightningDamage *= backStebWeapon.backstab_Attack_01_Modifier;
            damageEffect.magicDamage *= backStebWeapon.backstab_Attack_01_Modifier;
            damageEffect.poiseDamage *= backStebWeapon.backstab_Attack_01_Modifier;

            targetCharacter.characterNetworkManager.NotifyTheServerOfBackstabServerRpc(
                targetCharacter.NetworkObjectId,
                character.NetworkObjectId,
                "Backstabbed_01",
                backStebWeapon.itemID,
                damageEffect.physicalDamage,
                damageEffect.holyDamage,
                damageEffect.fireDamage,
                damageEffect.lightningDamage,
                damageEffect.magicDamage,
                damageEffect.poiseDamage);

        }

        public virtual void DrainStaminaBasedOnAttack() 
        {
            if (!player.IsOwner)
                return;

            float staminaDeducted = 0;

            switch (currentAttackType) 
            {
                case AttackType.LightAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargedAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                case AttackType.ChargedAttack02:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargeAttackStaminaCostMultiplier;
                    break;
                case AttackType.RunningAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.RunAttackStaminaCostMultiplier;
                    break;
                case AttackType.RollingAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.RollAttackStaminaCostMultiplier;
                    break;
                case AttackType.BackStepAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.BackStepAttackStaminaCostMultiplier;
                    break;
                case AttackType.LightJumpingAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStaminaCostMultiplier;
                    break;
                case AttackType.HeavyJumpingAttack01:
                    staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStaminaCostMultiplier;
                    break;
                default:
                    break;
            }

            Debug.Log("Stamina Deducte: " + staminaDeducted);
            player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
        }

        public override void SetTarget(CharacterManager newTarget)
        {
            base.SetTarget(newTarget);

            if (player.IsOwner) 
            {
                PlayerCamera.instance.SetLockCameraHeight();
            }
        }

        //Animation Event Calls
        public override void EnableCanDoCombo()
        {
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerCombatManager.canComboWithMainHandWeapon = true;
            }
            else
            {

            }
        }

        public override void DisableCanDoCombo()
        {
            player.playerCombatManager.canComboWithMainHandWeapon = false;
            //player.playerCombatManager.canComboWithOffHandWeapon = false;
        }

        public void InstantiateSpellWarmUpFX() 
        {
            if (player.playerInventoryManager.currentSpell == null)
                return;

            player.playerInventoryManager.currentSpell.InstantiateWarmUpSpellFX(player);
        }

        public void SuccessfullyCastSpell()
        {
            if (player.playerInventoryManager.currentSpell == null)
                return;

            player.playerInventoryManager.currentSpell.SuccessfullyCastSpell(player);
        }

        public void SuccessfullyUseQuickSlotItem()
        {
            if (player.playerInventoryManager.currentQuickSlotItem != null)
                player.playerInventoryManager.currentQuickSlotItem.SuccessfullyToUseItem(player);
        }

        public WeaponItem SelectWeaponToPerformSkill() 
        {
            WeaponItem selectedWeapon = player.playerInventoryManager.currentLeftHandWeapon;
            player.playerNetworkManager.SetCharacterActionHand(false);
            player.playerNetworkManager.currentWeaponBeingUsed.Value = selectedWeapon.itemID;
            return selectedWeapon;
        }

    }

}
