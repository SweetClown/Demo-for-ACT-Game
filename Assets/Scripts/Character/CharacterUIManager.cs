using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterUIManager : MonoBehaviour
    {
        [Header("UI")]
        public bool hasFloatingHPBar = true;
        public UI_Character_HP_Bar characterHPBar;

        public void OnHPChanged(int oldValue, int newValue) 
        {
            characterHPBar.OldHealthValue = oldValue;
            characterHPBar.SetStat(newValue);
        }

        public void ResetCharacterHPBar() 
        {
            if (characterHPBar == null)
                return;

            characterHPBar.currentDamageTaken = 0;

        }
    }
}
