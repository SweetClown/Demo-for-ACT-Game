using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AIMonsterCombatManager : AICharacterCombatManager
    {
        [Header("Damage Colliders")]
        [SerializeField] MonsterHandDamageCollider rightHandDamageCollider;
        [SerializeField] MonsterHandDamageCollider leftHandDamageCollider;

        [Header("Damage")]
        [SerializeField] int baseDamage = 25;
        [SerializeField] int basePoiseDamage = 25;
        [SerializeField] float attack01DamageModifier = 1.0f;
        [SerializeField] float attack02DamageModifier = 1.5f;

        public void SetAttack01Damage() 
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;

            rightHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
            leftHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;

            rightHandDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
            leftHandDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
        }

        public void OpenRightHandDamageCollider() 
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            rightHandDamageCollider.EnableDamageCollider();
        }

        public void DisableRightHandDamageCollider()
        {
            rightHandDamageCollider.DisableDamageCollider();
        }

        public void OpenLeftHandDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            leftHandDamageCollider.EnableDamageCollider();
        }

        public void DisableLeftHandDamageCollider()
        {
            leftHandDamageCollider.DisableDamageCollider();
        }

        public override void CloseAllDamageColliders()
        {
            base.CloseAllDamageColliders();

            rightHandDamageCollider.DisableDamageCollider();
            leftHandDamageCollider.DisableDamageCollider();
        }
    }
}
