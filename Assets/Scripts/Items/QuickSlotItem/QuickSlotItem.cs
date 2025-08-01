using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class QuickSlotItem : Item
    {
        [Header("Item Model")]
        [SerializeField] protected GameObject itemModel;

        [Header("Animation")]
        [SerializeField] protected string useItemAnimation;

        //Not all quick slot items are consumables
        [Header("Consumable")]
        public bool isConsumable = true;
        public int itemAmount = 1;

        public virtual void AttemptToUseItem(PlayerManager player) 
        {
            if (!CanIuseThisItem(player))
                return;

            player.playerAnimatorManager.PlayTargetActionAnimation(useItemAnimation, true);
        }

        public virtual void SuccessfullyToUseItem(PlayerManager player) 
        {

        }

        public virtual bool CanIuseThisItem(PlayerManager player) 
        {
            return true;
        }

        public virtual int GetCurrentAmount(PlayerManager player) 
        {
            return 0;
        }
    }
}
