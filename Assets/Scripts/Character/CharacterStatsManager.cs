using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SG
{
    public class CharacterStatsManager : MonoBehaviour
    {
        CharacterManager character;

        [Header("Runes")]
        public int runesDroppedOnDeath = 50;

        [Header("Stamina Regeneration")]
        [SerializeField] float staminaRegenerationAmount = 2;
        private float staminaRegenerationTimer = 0;
        private float staminaTickTimer = 0;
        [SerializeField] float staminaRegenerationDelay = 2;

        [Header("Blocking Absorptions")]
        public float blockingPhysicalAbsorption;
        public float blockingFireAbsorption;
        public float blockingMagicAbsorption;
        public float blockingLightingAbsorption;
        public float blockingHolyAbsorption;
        public float blockingStability;

        [Header("Armor Absorption")]
        public float armorPhysicalDamageAbsorption;
        public float armorMagicDamageAbsorption;
        public float armorFireDamageAbsorption;
        public float armorLightingDamageAbsorption;
        public float armorHolyDamageAbsorption;

        [Header("Armor Resistances")]
        public float armorImmunity; //Resistance to rot and poison
        public float armorRobustness; //Resistance to bleed and frost
        public float armorFocus; //Resistance to madness and sleep
        public float armorVitality; //Resistance to death curse

        [Header("Poise")]
        public float totalPoiseDamage;           //How much poise damage we have taken
        public float offensivePoiseBonus;        //The poise bonus gained from using weapons (heavy weapons have a much larger bonus)
        public float basePoiseDefense;           //The poise bonus gained from armor/talismans ect
        public float defaultPoiseResetTime = 8;  //The time it takes for poise damage to reset (must not be hit in the time or it will reset)
        public float poiseResetTimer = 0;        //The current timer for poise reset

        protected virtual void Awake() 
        {
            character = GetComponent<CharacterManager>();
        }

        protected virtual void Start() 
        {

        }

        protected virtual void Update() 
        {
            HandlePoiseResetTImer();
        }

        public int CalculateHealthBasedOnVitalityLevel(int vitality)
        {
            float health = 0;

            health = vitality * 15;

            return Mathf.RoundToInt(health);
        }

        public int CalculateStaminaBasedOnEnduranceLevel(int endurance) 
        {
            float stamina = 0;

            stamina = endurance * 10;

            return Mathf.RoundToInt(stamina);
        }

        public int CalculateManaBasedOnMindLevel(int mind) 
        {
            int mana = 0;

            mana = mind * 10;

            return Mathf.RoundToInt(mana);
        }

        public int CalculateCharacterLevelBasedOnAttributes(bool calculateProjectedLevel = false)
        {

            if (calculateProjectedLevel) 
            {
                int totalProjectedAttributes = Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.vigorSlider.value) +
                      Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.mindSlider.value) +
                      Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.enduranceSlider.value) +
                      Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.strengthSlider.value) +
                      Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.dexteritySlider.value) +
                      Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.intelligenceSlider.value) +
                      Mathf.RoundToInt(PlayerUIManager.instance.playerUILevelUpManager.faithSlider.value);

                int projectedCharacterLevel = totalProjectedAttributes - 70 + 1;

                if (projectedCharacterLevel < 1)
                    projectedCharacterLevel = 1;

                return projectedCharacterLevel;
            }

            int totalAttributes = character.characterNetworkManager.vigor.Value +
                                  character.characterNetworkManager.mind.Value +
                                  character.characterNetworkManager.endurance.Value +
                                  character.characterNetworkManager.strength.Value +
                                  character.characterNetworkManager.dexterity.Value +
                                  character.characterNetworkManager.intelligence.Value +
                                  character.characterNetworkManager.faith.Value;

            int characterLevel = totalAttributes - 70 + 1;

            if (characterLevel < 1)
                characterLevel = 1;

            return characterLevel;
        }

        public virtual void RegenerateStamina()
        {
            if (!character.IsOwner)
            {
                return;
            }

            //we do not want to regenerate stamina if we are using it
            if (character.characterNetworkManager.isSprinting.Value)
            {
                return;
            }

            if (character.isPerformingAction)
            {
                return;
            }

            staminaRegenerationTimer += Time.deltaTime;

            if (staminaRegenerationTimer >= staminaRegenerationDelay)
            {
                if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
                {
                    staminaTickTimer += Time.deltaTime;

                    if (staminaTickTimer >= 0.1)
                    {
                        staminaTickTimer = 0;
                        character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                    }
                }
            }
        }

        public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount) 
        {
            // We only want to reset the regeneration if the action used stamina

            //We don.t want to reset the regeneration if we are already regenerating Stamina
            if (currentStaminaAmount < previousStaminaAmount)
            {
                staminaRegenerationTimer = 0;
            }
        }

        protected virtual void HandlePoiseResetTImer() 
        {
            if (poiseResetTimer > 0)
            {
                poiseResetTimer -= Time.deltaTime;
            }
            else 
            {
                totalPoiseDamage = 0;
            }
        }
    }
}
