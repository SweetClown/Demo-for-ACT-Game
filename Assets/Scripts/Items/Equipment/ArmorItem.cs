using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    public class ArmorItem : EquipmentItem
    {
        [Header("Equipment Absorption Bonus")]
        public float physicalDamageAbsorption;
        public float magicDamageAbsorption;
        public float fireDamageAbsorption;
        public float lightingDamageAbsorption;
        public float holyDamageAbsorption;

        [Header("Equipment Resistance Bonus")]
        public float immunity; //Resistance to rot and poison
        public float robustness; //Resistance to bleed and frost
        public float focus; //Resistance to madness and sleep
        public float vitality; //Resistance to death curse

        [Header("Poise")]
        public float poise;

        public EquipmentModel[] equipmentModels;

    }
}
