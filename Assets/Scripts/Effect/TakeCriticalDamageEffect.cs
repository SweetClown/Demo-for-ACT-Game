using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    //Used for backstab and riposte at specific animation frames
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Critical Damage Effect")]
    public class TakeCriticalDamageEffect : TakeDamageEffect
    {
        public override void ProcessEffect(CharacterManager character)
        {
            if (character.characterNetworkManager.isInvulnerable.Value)
                return;

            //If the character is dead, no additional damage effects should be processed
            if (character.isDead.Value)
            {
                return;
            }

            //Calculate Damage
            CalculateDamage(character);

            character.characterCombatManager.pendingCriticalDamage = finalDamageDealt;
        }

        protected override void CalculateDamage(CharacterManager character)
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
    }
}
