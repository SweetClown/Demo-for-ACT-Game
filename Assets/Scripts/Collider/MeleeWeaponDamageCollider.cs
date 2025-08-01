using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class MeleeWeaponDamageCollider : DamageCollider
    {
        [Header("Attacking Character")]
        public CharacterManager characterCausingDamage; // When calculating damage this is used to check for attackers damage modifiers,effects

        [Header("Weapon Attack Modifiers")]
        public float light_Attack_01_Modifier;
        public float light_Attack_02_Modifier;
        public float heavy_Attack_01_Modifier;
        public float heavy_Attack_02_Modifier;
        public float charge_Attack_01_Modifier;
        public float charge_Attack_02_Modifier;
        public float Run_Attack_01_Modifier;
        public float Roll_Attack_01_Modifier;
        public float BackStep_Attack_01_Modifier;
        public float light_Jumping_Attack_01_Modifier;
        public float heavy_Jumping_Attack_01_Modifier;
        protected override void Awake()
        {
            base.Awake();

            if (damageCollider == null) 
            {
                damageCollider = GetComponent<Collider>();
            }
            damageCollider.enabled = false; // Melee weapon colliders should be disabled at start, only enabled when animations allow
        }

        protected override void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();

            if (damageTarget != null)
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                if (damageTarget == characterCausingDamage)
                    return;

                if (!WorldUtilityManager.Instance.CanIDamageThisTarget(characterCausingDamage.characterGroup, damageTarget.characterGroup))
                    return;

                //Check if target is parrying
                CheckForParry(damageTarget);

                //Check if target is blocking
                CheckForBlock(damageTarget);

                if (!damageTarget.characterNetworkManager.isInvulnerable.Value) 
                {
                    DamageTarget(damageTarget);
                    WorldSoundFXManager.instance.AlertNearbyCharactersToSound(transform.position, 2);
                }
            }
        }

        protected override void CheckForParry(CharacterManager damageTarget)
        {
            if (charactersDamaged.Contains(damageTarget))
                return;

            if (characterCausingDamage.characterNetworkManager.isParryable.Value)
                return;

            if (!damageTarget.IsOwner)
                return;

            if (damageTarget.characterNetworkManager.isParrying.Value)
            {
                charactersDamaged.Add(damageTarget);
                damageTarget.characterNetworkManager.NotifyServerOfParryServerRpc(characterCausingDamage.NetworkObjectId);
            }
        }

        protected override void GetBlockingDotValues(CharacterManager damageTarget)
        {
            directionFromAttackToDamageTarget = characterCausingDamage.transform.position - damageTarget.transform.position;
            dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
        }

        protected override void DamageTarget(CharacterManager damageTarget)
        {
            //We don.t want to damage the same target more than once in a single attack

            //So we add them to a list that checks before applying damage
            if (charactersDamaged.Contains(damageTarget))
                return;

            charactersDamaged.Add(damageTarget);

            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeDamageEffect);
            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.lightningDamage = lightningDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.contactPoint = contactPoint;
            damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);

            switch (characterCausingDamage.characterCombatManager.currentAttackType)
            {
                case AttackType.LightAttack01:
                    ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightAttack02:
                    ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack01:
                    ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyAttack02:
                    ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack01:
                    ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.ChargedAttack02:
                    ApplyAttackDamageModifiers(charge_Attack_02_Modifier, damageEffect);
                    break;
                case AttackType.RunningAttack01:
                    ApplyAttackDamageModifiers(Run_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.RollingAttack01:
                    ApplyAttackDamageModifiers(Roll_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.BackStepAttack01:
                    ApplyAttackDamageModifiers(BackStep_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.LightJumpingAttack01:
                    ApplyAttackDamageModifiers(light_Jumping_Attack_01_Modifier, damageEffect);
                    break;
                case AttackType.HeavyJumpingAttack01:
                    ApplyAttackDamageModifiers(heavy_Jumping_Attack_01_Modifier, damageEffect);
                    break;
                default:
                    break;
            }

            damageTarget.characterEffectsManager.ProcesssInstantEffect(damageEffect);

            if (characterCausingDamage.IsOwner) 
            {
                damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                    damageTarget.NetworkObjectId,
                    characterCausingDamage.NetworkObjectId,
                    damageEffect.physicalDamage,
                    damageEffect.magicDamage,
                    damageEffect.fireDamage,
                    damageEffect.holyDamage,
                    damageEffect.lightningDamage,
                    damageEffect.poiseDamage,
                    damageEffect.angleHitFrom,
                    damageEffect.contactPoint.x,
                    damageEffect.contactPoint.y,
                    damageEffect.contactPoint.z);
            }
        }

        private void ApplyAttackDamageModifiers(float modifier, TakeDamageEffect damage) 
        {
            damage.physicalDamage *= modifier;
            damage.magicDamage *= modifier;
            damage.fireDamage *= modifier;                
            damage.holyDamage *= modifier;
            damage.lightningDamage *= modifier;
            damage.poiseDamage *= modifier;

            //If attack is a fully charged heavy, multiply by full charge modifier after normal modifier have benn calculated
        }


    }
}
