using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Damage")]
    public class TakeDamageEffect : InstantCharacterEffect
    {
        [Header("Character Causing Damage")]
        public CharacterManager characterCausingDamage; //If the damage is caused by another characters attack it will be stored here

        [Header("Damage")]
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Final Damage")]
        public int finalDamageDealt = 0; //The damage the character take after all calculations have been made

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false; //If a character's poise is broken, they will be "Stunned" and play a damage animation

        //(TO DO) Build UPS
        //build up effect amounts

        [Header("Animation")]
        public bool playDamageAnimation = true;
        public bool manuallySelectDamageAnimation = false;
        public string damageAnimation;

        [Header("Sound FX")]
        public bool willPlayDamageSFX = true;
        public AudioClip elementalDamageSoundFX; //Used on top of regular SFX If there is Elemental Damage present (Magic/Fire/Holy/Lighting)

        [Header("Direction Damage Taken From")]
        public float angleHitFrom;              //Used to determine what damage animation to play (Move backwards, to the left, to the right ect)
        public Vector3 contactPoint;            //Used to determine where the blood fx instantiate

        public override void ProcessEffect(CharacterManager character)
        {
            if (character.characterNetworkManager.isInvulnerable.Value)
                return;

            base.ProcessEffect(character);

            //If the character is dead, no additional damage effects should be processed
            if (character.isDead.Value) 
            {
                return;
            }

            //Calculate Damage
            CalculateDamage(character);
            //Check which directional damage came from
            PlayDirectionalBasedDamageAnimation(character);
            //Play a Damage animation

            //Check for build ups (poison, bleed ect)

            PlayDamageSFX(character);
            PlayDamageVFX(character);

            //Run this after all other functions that would attempt to play an animation upon being damaged & After Poise/Stance damage calculated.
            CalculateStanceDamage(character);
            // If character is ai, check for new target if character causing damage is present
        }

        protected virtual void CalculateDamage(CharacterManager character) 
        {
            if (!character.IsOwner)
                return;

            if (characterCausingDamage != null) 
            {
                //Check for damage modifiers and modify base damage
                // physical *= physicalModifier ect
            }

            //Check character for flat defenses and subtract them from the damage

            //Check character for armor absorptions, and subtract the percentage from the damage

            //Add all damage types together, and apply Final Damage
            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0) 
            {
                finalDamageDealt = 1;
            }

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

            //We subject poise damage from the characters total
            character.characterStatsManager.totalPoiseDamage -= poiseDamage; //Is should be a negative number.
            
            //We store the previous poise damage taken for other interactions
            character.characterCombatManager.previousPoiseDamageTaken = poiseDamage;

            float remainingPoise = character.characterStatsManager.basePoiseDefense + 
                                   character.characterStatsManager.offensivePoiseBonus + 
                                   character.characterStatsManager.totalPoiseDamage;

            if (remainingPoise <= 0)
                poiseIsBroken = true;

            //Since the character has been hit, we reset the poise timer
            character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTime;
        }

        protected void CalculateStanceDamage(CharacterManager character) 
        {
            AICharacterManager aiCharacter = character as AICharacterManager;

            int stanceDamage = Mathf.RoundToInt(poiseDamage);

            if (aiCharacter != null)
            {
                aiCharacter.AICharacterCombatManager.DamageStance(stanceDamage);
            }
        }

        protected void PlayDamageVFX(CharacterManager character) 
        {
            //If we have fire damage, play fire particles
            //Lighting damage, play light particles

            character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
        }

        protected void PlayDamageSFX(CharacterManager character) 
        {
            AudioClip physicalDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

            character.characterSoundFXManager.PlaySoundFX(physicalDamageSFX);
            character.characterSoundFXManager.PlayDamageGruntSoundFX();
            //If fire damage is greater than 0, play burn sfx
            //If lighting damage is greater than 0, play zap sfx
        }

        protected void PlayDirectionalBasedDamageAnimation(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if (character.isDead.Value)
                return;

            if (poiseIsBroken)
            {
                if (angleHitFrom >= 145 && angleHitFrom <= 180)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
                }
                else if (angleHitFrom <= -145 && angleHitFrom >= -180)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
                }
                else if (angleHitFrom >= -45 && angleHitFrom <= 45)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
                }
                else if (angleHitFrom >= -144 && angleHitFrom <= -45)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
                }
                else if (angleHitFrom >= 45 && angleHitFrom <= 144)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
                }
            }
            else 
            {
                if (angleHitFrom >= 145 && angleHitFrom <= 180)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Ping_Damage);
                }
                else if (angleHitFrom <= -145 && angleHitFrom >= -180)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Ping_Damage);
                }
                else if (angleHitFrom >= -45 && angleHitFrom <= 45)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Ping_Damage);
                }
                else if (angleHitFrom >= -144 && angleHitFrom <= -45)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Ping_Damage);
                }
                else if (angleHitFrom >= 45 && angleHitFrom <= 144)
                {
                    damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Ping_Damage);
                }
            }

            character.characterAnimatorManager.LastDamageAnimationPlayed = damageAnimation;

            if (poiseIsBroken)
            {
                //If we are poise broken restrict our movement and actions
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
                character.characterCombatManager.DestroyAllCurrentActionFX();
            }
            else 
            {
                //If we are not poise broken simply play an Upperbidy animation without restricting
                character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, false, false, true, true);
            }
        }
    }
}
