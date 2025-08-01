using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerStatsManager : CharacterStatsManager
    {
        PlayerManager player;

        [Header("Runes")]
        public int runes = 0;

        protected override void Awake()
        {
            base.Awake();

            player = GetComponent<PlayerManager>();
           
        }

        protected override void Start()
        {
            base.Start();

            //When we make a character creation menu, and set the stats depending on the class, this will be calculated
            CalculateHealthBasedOnVitalityLevel(player.playerNetworkManager.vigor.Value);
            CalculateStaminaBasedOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
            CalculateManaBasedOnMindLevel(player.playerNetworkManager.mind.Value);
        }

        public void CalculateTotalArmorAbsorption() 
        {
            //Reset All values to 0
            armorPhysicalDamageAbsorption = 0;
            armorMagicDamageAbsorption = 0;
            armorFireDamageAbsorption = 0;
            armorLightingDamageAbsorption = 0;
            armorHolyDamageAbsorption = 0;

            armorImmunity = 0;
            armorRobustness = 0;
            armorFocus = 0;
            armorVitality = 0;

            basePoiseDefense = 0;

            //Head Equipment
            if (player.playerInventoryManager.headEquipment != null) 
            {
                //Damage Resistance
                armorPhysicalDamageAbsorption += player.playerInventoryManager.headEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.headEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.headEquipment.fireDamageAbsorption;
                armorLightingDamageAbsorption += player.playerInventoryManager.headEquipment.lightingDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.headEquipment.holyDamageAbsorption;

                //Status Effect Resistance
                armorRobustness += player.playerInventoryManager.headEquipment.robustness;
                armorImmunity += player.playerInventoryManager.headEquipment.immunity;
                armorFocus += player.playerInventoryManager.headEquipment.focus;
                armorVitality += player.playerInventoryManager.headEquipment.vitality;

                //Poise
                basePoiseDefense += player.playerInventoryManager.headEquipment.poise;
            }

            //Body Equipment
            if (player.playerInventoryManager.bodyEquipment != null)
            {
                //Damage Resistance
                armorPhysicalDamageAbsorption += player.playerInventoryManager.bodyEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.bodyEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.bodyEquipment.fireDamageAbsorption;
                armorLightingDamageAbsorption += player.playerInventoryManager.bodyEquipment.lightingDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.bodyEquipment.holyDamageAbsorption;

                //Status Effect Resistance
                armorRobustness += player.playerInventoryManager.bodyEquipment.robustness;
                armorImmunity += player.playerInventoryManager.bodyEquipment.immunity;
                armorFocus += player.playerInventoryManager.bodyEquipment.focus;
                armorVitality += player.playerInventoryManager.bodyEquipment.vitality;

                //Poise
                basePoiseDefense += player.playerInventoryManager.bodyEquipment.poise;

            }

            //Leg Equipment
            if (player.playerInventoryManager.legEquipment != null)
            {
                //Damage Resistance
                armorPhysicalDamageAbsorption += player.playerInventoryManager.legEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.legEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.legEquipment.fireDamageAbsorption;
                armorLightingDamageAbsorption += player.playerInventoryManager.legEquipment.lightingDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.legEquipment.holyDamageAbsorption;

                //Status Effect Resistance
                armorRobustness += player.playerInventoryManager.legEquipment.robustness;
                armorImmunity += player.playerInventoryManager.legEquipment.immunity;
                armorFocus += player.playerInventoryManager.legEquipment.focus;
                armorVitality += player.playerInventoryManager.legEquipment.vitality;

                //Poise
                basePoiseDefense += player.playerInventoryManager.legEquipment.poise;

            }

            //Hand Equipment
            if (player.playerInventoryManager.handEquipment != null)
            {
                //Damage Resistance
                armorPhysicalDamageAbsorption += player.playerInventoryManager.handEquipment.physicalDamageAbsorption;
                armorMagicDamageAbsorption += player.playerInventoryManager.handEquipment.magicDamageAbsorption;
                armorFireDamageAbsorption += player.playerInventoryManager.handEquipment.fireDamageAbsorption;
                armorLightingDamageAbsorption += player.playerInventoryManager.handEquipment.lightingDamageAbsorption;
                armorHolyDamageAbsorption += player.playerInventoryManager.handEquipment.holyDamageAbsorption;

                //Status Effect Resistance
                armorRobustness += player.playerInventoryManager.handEquipment.robustness;
                armorImmunity += player.playerInventoryManager.handEquipment.immunity;
                armorFocus += player.playerInventoryManager.handEquipment.focus;
                armorVitality += player.playerInventoryManager.handEquipment.vitality;

                //Poise
                basePoiseDefense += player.playerInventoryManager.handEquipment.poise;

            }
        }

        public void AddRunes(int runesToAdd) 
        {
            runes += runesToAdd;
            PlayerUIManager.instance.playerUIHudManager.SetRunesCount(runesToAdd);
        }
    }
}
