using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

namespace SweetClown
{
    public class PlayerUIHudManager : MonoBehaviour
    {
        [SerializeField] CanvasGroup[] canvasGroup;

        [Header("Stat Bars")]
        [SerializeField] UI_StatBar healthBar;
        [SerializeField] UI_StatBar staminaBar;
        [SerializeField] UI_StatBar ManaBar;

        [Header("Runes")]
        [SerializeField] float runeUpdateCountDelayTimer = 2.5f;
        private int pendingRunesToAdd = 0;
        private Coroutine waitThenAddRunesCoroutine;
        [SerializeField] TextMeshProUGUI runesCountText;
        [SerializeField] TextMeshProUGUI runesToAddText;

        [Header("Quick Slots")]
        [SerializeField] Image rightWeaponQuickSlotIcon;
        [SerializeField] Image leftWeaponQuickSlotIcon;
        [SerializeField] Image spellQuickSlotIcon;
        [SerializeField] Image quickSlotItemQuickSlotIcon;
        [SerializeField] TextMeshProUGUI quickSlotItemCount;

        [Header("Boss Health Bar")]
        public Transform bossHealthBarParent;
        public GameObject bossHealthBarObject;

        public void ToggleHUD(bool status)
        {
            if (status)
            {
                foreach (var canvas in canvasGroup)
                {
                    canvas.alpha = 1.0f;
                }
            }
            else
            {
                foreach (var canvas in canvasGroup)
                {
                    canvas.alpha = 0f;
                }
            }
        }

        public void RefreshHUD()
        {
            healthBar.gameObject.SetActive(false);
            healthBar.gameObject.SetActive(true);
            staminaBar.gameObject.SetActive(false);
            staminaBar.gameObject.SetActive(true);
            ManaBar.gameObject.SetActive(false);
            ManaBar.gameObject.SetActive(true);
        }

        public void SetRunesCount(int runesToAdd) 
        {
            pendingRunesToAdd = runesToAdd;

            if (waitThenAddRunesCoroutine != null)
                StopCoroutine(waitThenAddRunesCoroutine);

            waitThenAddRunesCoroutine = StartCoroutine(WaitThenUpdateRuneCount());
        }

        private IEnumerator WaitThenUpdateRuneCount()
        {

            //Wait for timer to reach 0 Incase More Runes are Qued Up
            float timer = runeUpdateCountDelayTimer;
            int runesToAdd = pendingRunesToAdd;

            if (runesToAdd >= 0)
            {
                runesToAddText.text = "+ " + runesToAdd.ToString();
            }
            else 
            {
                runesToAddText.text = "- " + Mathf.Abs(runesToAdd).ToString();
            }

            runesToAddText.enabled = true;

            while (timer > 0)
            {
                timer -= Time.deltaTime;

                //If more runes are qued up. Re-Update Total New Runes Count
                if (runesToAdd != pendingRunesToAdd) 
                {
                    runesToAdd = pendingRunesToAdd;
                    runesToAddText.text += "+ " + runesToAdd.ToString();
                }

                yield return null;
            }

            // Update Rune count and Hide Pending Runes
            runesToAddText.enabled = false;
            pendingRunesToAdd = 0 ;
            runesCountText.text = PlayerUIManager.instance.localPlayer.playerStatsManager.runes.ToString() ;

            yield return null;
        }

        public void SetNewHealthValue(int oldValue, int newValue)
        {
            healthBar.SetStat(newValue);
        }

        public void SetMaxHealthValue(int maxHealth)
        {
            healthBar.SetMaxStat(maxHealth);
        }

        public void SetNewStaminaValue(float oldValue, float newValue)
        {
            staminaBar.SetStat(Mathf.RoundToInt(newValue));
        }

        public void SetMaxStaminaValue(int maxStamina)
        {
            staminaBar.SetMaxStat(maxStamina);
        }

        public void SetNewManaValue(int oldValue, int newValue)
        {
            ManaBar.SetStat(newValue);
        }

        public void SetMaxManaValue(int maxMana)
        {
            ManaBar.SetMaxStat(maxMana);
        }

        public void SetRightWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDataBase.Instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("Item has no icon");
                rightWeaponQuickSlotIcon.enabled = false;
                rightWeaponQuickSlotIcon.sprite = null;
                return;
            }

            rightWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            rightWeaponQuickSlotIcon.enabled = true;

        }

        public void SetLeftWeaponQuickSlotIcon(int weaponID)
        {
            WeaponItem weapon = WorldItemDataBase.Instance.GetWeaponByID(weaponID);

            if (weapon == null)
            {
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            if (weapon.itemIcon == null)
            {
                Debug.Log("Item has no icon");
                leftWeaponQuickSlotIcon.enabled = false;
                leftWeaponQuickSlotIcon.sprite = null;
                return;
            }

            leftWeaponQuickSlotIcon.sprite = weapon.itemIcon;
            leftWeaponQuickSlotIcon.enabled = true;

        }

        public void SetSpellQuickSlotIcon(int spellID)
        {
            SpellItem spell = WorldItemDataBase.Instance.GetSpellByID(spellID);

            if (spell == null)
            {
                spellQuickSlotIcon.enabled = false;
                spellQuickSlotIcon.sprite = null;
                return;
            }

            if (spell.itemIcon == null)
            {
                Debug.Log("Item has no icon");
                spellQuickSlotIcon.enabled = false;
                spellQuickSlotIcon.sprite = null;
                return;
            }

            spellQuickSlotIcon.sprite = spell.itemIcon;
            spellQuickSlotIcon.enabled = true;

        }

        public void SetQuickSlotItemQuickSlotIcon(QuickSlotItem quickSlotItem)
        {

            if (quickSlotItem == null)
            {
                quickSlotItemQuickSlotIcon.enabled = false;
                quickSlotItemQuickSlotIcon.sprite = null;
                quickSlotItemCount.enabled = false;
                return;
            }

            if (quickSlotItem.itemIcon == null)
            {
                Debug.Log("Item has no icon");
                quickSlotItemQuickSlotIcon.enabled = false;
                quickSlotItemQuickSlotIcon.sprite = null;
                return;
            }

            quickSlotItemQuickSlotIcon.sprite = quickSlotItem.itemIcon;
            quickSlotItemQuickSlotIcon.enabled = true;

            if (quickSlotItem.isConsumable)
            {
                PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
                quickSlotItemCount.enabled = true;
                quickSlotItemCount.text = quickSlotItem.GetCurrentAmount(player).ToString();
            }
            else 
            {
                quickSlotItemCount.enabled = false;
            }

        }
    }
}
