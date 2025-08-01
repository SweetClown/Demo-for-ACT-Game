using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class AIBossCombatManager : AICharacterCombatManager
    {
        AIV50CharacterManager aiV50Manager;

        [Header("Damage Colliders")]
        [SerializeField] BossSwordDamageCollider SwordDamageCollider;

        [Header("Damage")]
        [SerializeField] int baseDamage = 15;
        [SerializeField] int basePoiseDamage = 25;
        [SerializeField] float attack01DamageModifier = 1.5f;
        [SerializeField] float attack02DamageModifier = 1.5f;

        protected override void Awake()
        {
            base.Awake();

            aiV50Manager = GetComponent<AIV50CharacterManager>();
        }

        public void SetAttack01Damage()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            SwordDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;
            SwordDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            SwordDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
            SwordDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
        }

        public void OpenSwordDamageCollider()
        {
            SwordDamageCollider.EnableDamageCollider();
            aiV50Manager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(aiV50Manager.V50soundFXManager.swordWhooshes));
        }

        public void DisableSwordDamageCollider()
        {
            SwordDamageCollider.DisableDamageCollider();
        }

        public override void PivotTowardsTarget(AICharacterManager aiCharacter)
        {
            base.PivotTowardsTarget(aiCharacter);
        }
    }
}
