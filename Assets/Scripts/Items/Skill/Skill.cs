using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    public class Skill : Item
    {
        [Header("Skill Information")]
        public WeaponClass[] usableWeaponClasses;

        [Header("Costs")]
        public int focusPointCost = 20;
        public int staminaCost = 20;

        public virtual void AttemptToPerformAction(PlayerManager playerPerformingAction) 
        {
            Debug.Log("Performed!");
        }

        public virtual bool CanIUseThisAbility(PlayerManager playerPerformingAction) 
        {
            return false;
        }

        protected virtual void DeductStaminaCost(PlayerManager playerPerformingAction) 
        {
            playerPerformingAction.playerNetworkManager.currentStamina.Value -= staminaCost;
        }
        protected virtual void DeductFocusCost(PlayerManager playerPerformingAction)
        {

        }
    }
}
