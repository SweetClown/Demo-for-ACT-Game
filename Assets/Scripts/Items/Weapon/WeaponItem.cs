using UnityEngine;

namespace SweetClown
{
    public class WeaponItem : EquipmentItem
    {
        [Header("Animations")]
        public AnimatorOverrideController weaponAnimator;

        [Header("Model Instantiation")]
        public WeaponModelType weaponModelType;

        [Header("Weapon Model")]
        public GameObject weaponModel;

        [Header("Wepaon Class")]
        public WeaponClass weaponClass;

        [Header("Weapon Requirements")]
        public int strengthREQ = 0;
        public int dexREQ = 0;
        public int intREQ = 0;
        public int faithREQ = 0;

        [Header("Weapon Base Damage")]
        public int physicalDamage = 0;
        public int magicDamage = 0;
        public int fireDamage = 0;
        public int holyDamage = 0;
        public int lightingDamage = 0;

        [Header("Weapon Poise")]
        public float poiseDamage = 10;

        [Header("Attack Modifiers")]
        public float light_Attack_01_Modifier = 1.0f;
        public float light_Attack_02_Modifier = 1.2f;
        public float heavy_Attack_01_Modifier = 1.4f;
        public float heavy_Attack_02_Modifier = 1.7f;
        public float charge_Attack_01_Modifier = 2.0f;
        public float charge_Attack_02_Modifier = 2.3f;
        public float Run_Attack_01_Modifier = 1.2f;
        public float Roll_Attack_01_Modifier = 1.2f;
        public float Backstep_Attack_01_Modifier = 1.2f;
        public float light_Jumping_Attack_01_Modifier = 1.3f;
        public float heavy_Jumping_Attack_01_Modifier = 1.8f;

        [Header("Stamina Costs Modifiers")]
        public int baseStaminaCost = 20;
        public float lightAttackStaminaCostMultiplier = 1f;
        public float heavyAttackStaminaCostMultiplier = 1.2f;
        public float chargeAttackStaminaCostMultiplier = 1.5f;
        public float RunAttackStaminaCostMultiplier = 1f;
        public float RollAttackStaminaCostMultiplier = 1f;
        public float BackStepAttackStaminaCostMultiplier = 1f;

        [Header("Weapon Blocking Absorption")]
        public float physicalBaseDamageAbsorption = 50;
        public float fireBaseDamageAbsorption = 50;
        public float lightingBaseDamageAbsorption = 50;
        public float holyBaseDamageAbsorption = 50;
        public float magicBaseDamageAbsorption = 50;
        public float stability = 50; //Reduce stamina lost from block

        //Item Based Actions (Left click, right click)
        [Header("Actions")]
        public WeaponItemAction oh_LeftClick_Action; //One hand Left Click action 
        public WeaponItemAction oh_RightClick_Action; //One hand Right Trigger action 
        public WeaponItemAction oh_RKey_Action; //One hand Rkey trigger action
        public Skill skillAction;

        //Blocking sounds
        [Header("SFX")]
        public AudioClip[] whooshes;
        public AudioClip[] blocking;
    }

}
