using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SG
{
    public class SpellItem : Item
    {
        [Header("Spell Class")] 
        public SpellClass SpellClass;

        [Header("Spell Modifiers")]
        //public float fullChargeEffectMultiplier = 2;

        [Header("Spell Costs")]
        public int spellSlotsUsed = 1;
        public int staminaCost = 25;
        public int manaCost = 25;

        [Header("Spell FX")]
        [SerializeField] protected GameObject spellCastWarmUpFX;
        [SerializeField] protected GameObject spellCastReleaseFX;

        [Header("Animation")]
        [SerializeField] protected string mainHandSpellAnimation;
        [SerializeField] protected string offHandSpellAnimation;

        [Header("Sound FX")]
        public AudioClip warmUpSoundFX;
        public AudioClip releaseSoundFX;

        //Warm up Animation
        public virtual void AttemptToCastSpell(PlayerManager player) 
        {

        }

        //Throw or cast Animation
        public virtual void SuccessfullyCastSpell(PlayerManager player) 
        {
            if (player.IsOwner) 
            {
                player.playerNetworkManager.currentMana.Value -= manaCost;
                player.playerNetworkManager.currentStamina.Value -= staminaCost;
            }
        }

        //Spell FX in warm up
        public virtual void InstantiateWarmUpSpellFX(PlayerManager player) 
        {

        }

        //Spell FX in release
        public virtual void InstantiateReleaseFX(PlayerManager player)
        {

        }

        public virtual bool CanICastThisSpell(PlayerManager player) 
        {
            if (player.playerNetworkManager.currentMana.Value <= manaCost)
                return false;

            if (player.playerNetworkManager.currentMana.Value <= staminaCost)
                return false;

            if (player.isPerformingAction)
                return false;

            if (player.playerNetworkManager.isJumping.Value)
                return false;

            return true;
        }
    }
}
