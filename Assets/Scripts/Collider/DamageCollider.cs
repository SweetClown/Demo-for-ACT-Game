using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class DamageCollider : MonoBehaviour
    {
        [Header("Collider")]
        public Collider damageCollider;

        [Header("Damage")]
        public float physicalDamage = 0;
        public float magicDamage = 0;
        public float fireDamage = 0;
        public float lightningDamage = 0;
        public float holyDamage = 0;

        [Header("Poise")]
        public float poiseDamage = 0;

        [Header("Contact Point")]
        public Vector3 contactPoint;

        [Header("Characters Damaged")]
        protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

        [Header("Block")]
        protected Vector3 directionFromAttackToDamageTarget;
        protected float dotValueFromAttackToDamageTarget;

        protected virtual void Awake() 
        {

        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            CharacterManager damageTarget = other.GetComponent<CharacterManager>();

            if (damageTarget != null) 
            {
                contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

                //Check if target is blocking
                CheckForBlock(damageTarget);

                //Check if target is parrying
                CheckForParry(damageTarget);

                if (!damageTarget.characterNetworkManager.isInvulnerable.Value)
                    DamageTarget(damageTarget);
            }
        }

        protected virtual void CheckForBlock(CharacterManager damageTarget) 
        {
            //If this character has already been damaged, do not proceed
            if (charactersDamaged.Contains(damageTarget))
                return;

            GetBlockingDotValues(damageTarget);

            if (damageTarget.characterNetworkManager.isBlocking.Value && dotValueFromAttackToDamageTarget > 0.3f) 
            {
                //If the character is blocking, check if they are facing in the correct direction to block successfully
                charactersDamaged.Add(damageTarget);

                TakeBlockedDamageEffect damageEffect = Instantiate(WorldCharacterEffectsManager.instance.takeBlockedDamageEffect);

                damageEffect.physicalDamage = physicalDamage;
                damageEffect.magicDamage = magicDamage;
                damageEffect.fireDamage = fireDamage;
                damageEffect.holyDamage = holyDamage;
                damageEffect.lightningDamage = lightningDamage;
                damageEffect.poiseDamage = poiseDamage;
                damageEffect.staminaDamage = poiseDamage; 
                damageEffect.contactPoint = contactPoint;

                damageTarget.characterEffectsManager.ProcesssInstantEffect(damageEffect);
            }
        }
        protected virtual void CheckForParry(CharacterManager damageTarget)
        {

        }

        protected virtual void GetBlockingDotValues(CharacterManager damageTarget) 
        {
            directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
            dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
        }

        protected virtual void DamageTarget(CharacterManager damageTarget) 
        {
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

            damageTarget.characterEffectsManager.ProcesssInstantEffect(damageEffect);
        }

        public virtual void EnableDamageCollider() 
        {
            damageCollider.enabled = true;
        }

        public virtual void DisableDamageCollider()
        {
            damageCollider.enabled = false;
            charactersDamaged.Clear(); //We reset the characters that have been hit when we reset the collider, so they may be hit again
        }

       
    }
}
