using System.Collections;
using System.Collections.Generic;
using UnityEditor.Build;
using UnityEngine;

namespace SweetClown
{
    public class FireBallManager : SpellManager { 
        [Header("Colliders")]
        public FireBallDamageCollider damageColider;

        [Header("Instantiated FX")]
        private GameObject instantiatedDestructionFX;

        private bool hasCollided = false;
        public Rigidbody fireBallRigidbody;
        private Coroutine destructionFXCoroutine;

        protected override void Awake()
        {
            base.Awake();

            fireBallRigidbody = GetComponent<Rigidbody>();
        }

        protected override void Update()
        {
            base.Update();

            if (spellTarget != null) 
            {
                transform.LookAt(spellTarget.characterCombatManager.lockOnTransform.position);
            }
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.layer == 6)
                return;

            if (!hasCollided)
            {
                hasCollided = true;
                InstantiateSpellDestructionFX();
            }
        }

        public void InitializeFireBall(CharacterManager spellCaster) 
        {
            damageColider.spellCaster = spellCaster;

            damageColider.fireDamage = 150;
        }

        public void InstantiateSpellDestructionFX() 
        {
            instantiatedDestructionFX = Instantiate(impactParticle, transform.position, Quaternion.identity);

            WorldSoundFXManager.instance.AlertNearbyCharactersToSound(transform.position, 5);

            Destroy(gameObject);
        }

        public void WaitThenInstantiateSpellDestructionFX(float timeToWait) 
        {
            if (destructionFXCoroutine != null)
                StopCoroutine(destructionFXCoroutine);

            destructionFXCoroutine = StartCoroutine(WaitThenInstantiateFX(timeToWait));
            StartCoroutine(WaitThenInstantiateFX(timeToWait));
        }

        private IEnumerator WaitThenInstantiateFX(float timeToWait) 
        {
            yield return new WaitForSeconds(timeToWait);

            InstantiateSpellDestructionFX();
        }
    }
}
