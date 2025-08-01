using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Blocked Damage")]
    public class TakeBlockedDamageEffect : InstantCharacterEffect
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
        private int finalDamageDealt = 0; //The damage the character take after all calculations have been made

        [Header("Poise")]
        public float poiseDamage = 0;
        public bool poiseIsBroken = false; //If a character's poise is broken, they will be "Stunned" and play a damage animation

        [Header("Stamina")]
        public float staminaDamage = 0;
        public float finalStaminaDamage = 0;

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

            Debug.Log("Hit was blocked");

            //If the character is dead, no additional damage effects should be processed
            if (character.isDead.Value)
            {
                return;
            }

            //Calculate Damage and Stamina Damage
            CalculateDamage(character);
            CalculateStaminaDamage(character);
            //Check which directional damage came from
            PlayDirectionalBasedBlockingAnimation(character);
            //Play a Damage animation

            //Check for build ups (poison, bleed ect)

            PlayDamageSFX(character);
            PlayDamageVFX(character);
            // If character is ai, check for new target if character causing damage is present

            CheckForGuardBreak(character);
        }

        private void CalculateDamage(CharacterManager character)
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

            Debug.Log("Original Physical Damage: " + physicalDamage);

            physicalDamage -= (physicalDamage * (character.characterStatsManager.blockingPhysicalAbsorption / 100));
            magicDamage -= (magicDamage * (character.characterStatsManager.blockingMagicAbsorption / 100));
            fireDamage -= (fireDamage * (character.characterStatsManager.blockingFireAbsorption / 100));
            lightningDamage -= (lightningDamage * (character.characterStatsManager.blockingLightingAbsorption / 100));
            holyDamage -= (holyDamage * (character.characterStatsManager.blockingHolyAbsorption / 100));

            finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

            if (finalDamageDealt <= 0)
            {
                finalDamageDealt = 1;
            }

            Debug.Log("Final Damage Given" + physicalDamage);

            character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;
        }

        private void CalculateStaminaDamage(CharacterManager character) 
        {
            if (!character.IsOwner)
                return;

            finalStaminaDamage = staminaDamage;

            float staminaDamageAbsorption = finalStaminaDamage * (character.characterStatsManager.blockingStability / 100);
            float staminaDamageAfterAbsorption = finalStaminaDamage - staminaDamageAbsorption;

            character.characterNetworkManager.currentStamina.Value -= staminaDamageAfterAbsorption;
        }

        private void CheckForGuardBreak(CharacterManager character) 
        {
            //if (character.characterNetworkManager.currentStamina.Value <= 0)
            //Play sfx
            if (!character.IsOwner)
                return;

            if (character.characterNetworkManager.currentStamina.Value <= 0) 
            {
                character.characterAnimatorManager.PlayTargetActionAnimation("Guard_Break_01", true);
                character.characterNetworkManager.isBlocking.Value = false;
            }
        }

        private void PlayDamageVFX(CharacterManager character)
        {
            //If we have fire damage, play fire particles
            //Lighting damage, play light particles

            //Get vfx based on blocking weapon
        }

        private void PlayDamageSFX(CharacterManager character)
        {
            //If fire damage is greater than 0, play burn sfx
            //If lighting damage is greater than 0, play zap sfx

            //Get sfx based on blocking weapon
            character.characterSoundFXManager.PlayBlockSoundFX();
        }

        private void PlayDirectionalBasedBlockingAnimation(CharacterManager character)
        {
            if (!character.IsOwner)
                return;

            if (character.isDead.Value)
                return;

            DamageIntensity damageIntensity = WorldUtilityManager.Instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);

            //Todo: check for doing two hand status
            switch (damageIntensity)
            {
                case DamageIntensity.Ping:
                    damageAnimation = "Block_Ping_01";
                    break;
                case DamageIntensity.Light:
                    damageAnimation = "Block_Light_01";
                    break;
                case DamageIntensity.Medium:
                    damageAnimation = "Block_Medium_01";
                    break;
                case DamageIntensity.Heavy:
                    damageAnimation = "Block_Heavy_01";
                    break;
                case DamageIntensity.Colossal:
                    damageAnimation = "Block_Colossal_01";
                    break;
                default:
                    break;
            }

            character.characterAnimatorManager.LastDamageAnimationPlayed = damageAnimation;
            character.characterAnimatorManager.PlayTargetActionAnimation(damageAnimation, true);
        }
    }
}
