using UnityEngine;

namespace SweetClown {
    public class AIKnightCombatManager : AICharacterCombatManager
    {
        [Header("Damage Colliders")]
        [SerializeField] ManualDamageCollider SwordDamageCollider;

        [Header("Damage Modifiers")]
        [SerializeField] float attack01DamageModifier = 1.0f;
        [SerializeField] float attack02DamageModifier = 1.5f;

        public void SetAttack01Damage()
        {
            SwordDamageCollider.physicalDamage = baseDamage * attack01DamageModifier;

            SwordDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifier;
        }

        public void SetAttack02Damage()
        {
            SwordDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;

            SwordDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
        }

        public void OpenSwordDamageCollider()
        {
            aiCharacter.characterSoundFXManager.PlayAttackGruntSoundFX();
            SwordDamageCollider.EnableDamageCollider();
        }

        public void DisableSwordDamageCollider()
        {
            SwordDamageCollider.DisableDamageCollider();
        }

        public override void CloseAllDamageColliders()
        {
            base.CloseAllDamageColliders();

            SwordDamageCollider.DisableDamageCollider();
        }
    }
}
