using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Analytics;
using UnityEditorInternal;

namespace SG
{
    public class CharacterCombatManager : NetworkBehaviour
    {
        protected CharacterManager character;

        [Header("Last Attack Animation Performed")]
        public string lastAttackAnimationPerformed;

        [Header("Previous Poise Damage Taken")]
        public float previousPoiseDamageTaken;

        [Header("Attack Target")]
        public CharacterManager currentTarget;

        [Header("Attack Type")]
        public AttackType currentAttackType;

        [Header("Lock On Transform")]
        public Transform lockOnTransform;

        [Header("Attack Flags")]
        public bool canPerformRollingAttack = false;
        public bool canDoBackStepAttack = false;
        public bool canBlock = true;
        public bool canBeBackstabbed = true;

        [Header("Critical Attack")]
        private Transform riposteReveiverTransform;
        private Transform backstabReveiverTransform;
        [SerializeField] float criticalAttackDistanceCheck = 0.7f;
        public int pendingCriticalDamage;

        protected virtual void Awake() 
        {
            character = GetComponent<CharacterManager>();
        }

        public virtual void SetTarget(CharacterManager newTarget) 
        {
            if (character.IsOwner) 
            {
                if (newTarget != null)
                {
                    currentTarget = newTarget;
                    character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
                }
                else 
                {
                    currentTarget = null;
                }
            }
        }

        //Used to attempt a backstab/Ripsote
        public virtual void AttemptCriticalAttack() 
        {
            if (character.isPerformingAction)
                return;

            if (character.characterNetworkManager.currentStamina.Value <= 0)
                return;

            RaycastHit[] hits = Physics.RaycastAll(character.characterCombatManager.lockOnTransform.position, character.transform.TransformDirection(Vector3.forward)
                                                                                  , 0.7f, WorldUtilityManager.Instance.GetCharacterLayers());

            for (int i = 0; i < hits.Length; i++) 
            {
                //Check each of the hits 1 by 1,giving them their own variable
                RaycastHit hit = hits[i];

                CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

                if (targetCharacter != null) 
                {
                    //If the character is the one attempting the critical strike, go to the next hit in the array of total hits
                    if (targetCharacter == character)
                        continue;

                    //If we cannot damage the character that is targeted continue to check the next hit in he array of hits
                    if (!WorldUtilityManager.Instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
                        continue;

                    Vector3 directionFromCharacterToTarget = character.transform.position - targetCharacter.transform.position;
                    float targetViewableAngle = Vector3.SignedAngle(directionFromCharacterToTarget, targetCharacter.transform.forward, Vector3.up);

                    if (targetCharacter.characterNetworkManager.isRipostable.Value)
                    {
                        if (targetViewableAngle >= -60 && targetViewableAngle <= 60) 
                        {
                            AttempRiposte(hit);
                            return;
                        }
                    }

                    if (targetCharacter.characterCombatManager.canBeBackstabbed) 
                    {
                        if (targetViewableAngle <= 180 && targetViewableAngle >= 145) 
                        {
                            AttempBackstab(hit);
                            return;
                        }

                        if (targetViewableAngle >= -180 && targetViewableAngle <= 145)
                        {
                            AttempBackstab(hit);
                            return;
                        }

                    }
                }
            }
        }

        public virtual void AttempRiposte(RaycastHit hit) 
        {

        }

        public virtual void AttempBackstab(RaycastHit hit)
        {

        }

        public virtual void ApplyCriticalDamage() 
        {
            character.characterEffectsManager.PlayCriticalBloodSplatterVFX(character.characterCombatManager.lockOnTransform.position);
            character.characterSoundFXManager.PlayCriticalStrikeSoundFX();

            if (character.IsOwner)
                character.characterNetworkManager.currentHealth.Value -= pendingCriticalDamage;
        }

        public IEnumerator ForceMoveEnemyCharacterToRipostePosition(CharacterManager enemyCharacter, Vector3 ripostePosition) 
        {
            float timer = 0;

            while (timer < 0.2f) 
            {
                timer += Time.deltaTime;

                if (riposteReveiverTransform == null) 
                {
                    GameObject riposteTransformObject = new GameObject("Riposte Transform");
                    riposteTransformObject.transform.parent = transform;
                    riposteTransformObject.transform.position = Vector3.zero;
                    riposteReveiverTransform = riposteTransformObject.transform;
                }

                riposteReveiverTransform.localPosition = ripostePosition;
                enemyCharacter.transform.position = riposteReveiverTransform.position;
                transform.rotation = Quaternion.LookRotation(-enemyCharacter.transform.forward);
                yield return null;
            }
        }

        public IEnumerator ForceMoveEnemyCharacterToBackstabPosition(CharacterManager enemyCharacter, Vector3 backstabPosition)
        {
            float timer = 0;

            while (timer < 0.2f)
            {
                timer += Time.deltaTime;

                if (riposteReveiverTransform == null)
                {
                    GameObject backstabTransformObject = new GameObject("Backstab Transform");
                    backstabTransformObject.transform.parent = transform;
                    backstabTransformObject.transform.position = Vector3.zero;
                    backstabReveiverTransform = backstabTransformObject.transform;
                }

                backstabReveiverTransform.localPosition = backstabPosition;
                enemyCharacter.transform.position = backstabReveiverTransform.position;
                transform.rotation = Quaternion.LookRotation(enemyCharacter.transform.forward);
                yield return null;
            }
        }

        public void EnableIsInvulnerable() 
        {
            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = true;
        }

        public void DisableIsInvulnerable() 
        {
            if (character.IsOwner)
                character.characterNetworkManager.isInvulnerable.Value = false;
        }

        public void EnableIsParrying()
        {
            if (character.IsOwner)
                character.characterNetworkManager.isParrying.Value = true;
        }

        public void DisableIsParrying()
        {
            if (character.IsOwner)
                character.characterNetworkManager.isParrying.Value = false;
        }

        public void EnableIsRipostable() 
        {
            if (character.IsOwner)
                character.characterNetworkManager.isRipostable.Value = true;
        }

        public void EnableCanDoRollingAttack()
        {
            if (character.IsOwner)
                canPerformRollingAttack = true;
                
        }

        public void DisableCanDoRollingAttack()
        {
            if (character.IsOwner)
                canPerformRollingAttack = false;

        }

        public void EnableCanDoBackStepAttack()
        {
            if (character.IsOwner)
                canDoBackStepAttack = true;

        }

        public void DisableCanDoBackStepAttack()
        {
            if (character.IsOwner)
                canDoBackStepAttack = false;

        }

        public virtual void EnableCanDoCombo()
        {

        }

        public virtual void DisableCanDoCombo()
        {

        }

        public virtual void CloseAllDamageColliders() 
        {

        }

        //Used to destory things like a Spell Warm Up Fx when the character is poise broken
        public void DestroyAllCurrentActionFX() 
        {
            character.characterNetworkManager.DestroyAllCurrentActionFXServerRpc();
        }
    }
}
