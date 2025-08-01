using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

namespace SG
{
    public class CharacterManager : NetworkBehaviour
    {
        [Header("Status")]
        public NetworkVariable<bool> isDead = new NetworkVariable<bool>
                                              (false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        [HideInInspector] public CharacterController characterController;
        [HideInInspector] public Animator animator;

        [HideInInspector] public CharacterNetworkManager characterNetworkManager;
        [HideInInspector] public CharacterEffectsManager characterEffectsManager;
        [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
        [HideInInspector] public CharacterCombatManager characterCombatManager;
        [HideInInspector] public CharacterStatsManager characterStatsManager;
        [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
        [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
        [HideInInspector] public CharacterUIManager characterUIManager;

        [Header("Character Group")]
        public CharacterGroup characterGroup;

        [Header("Flags")]
        public bool isPerformingAction = false;
        public bool applyRootMotion = false;

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            characterController = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            characterNetworkManager = GetComponent<CharacterNetworkManager>();
            characterEffectsManager = GetComponent<CharacterEffectsManager>();
            characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
            characterCombatManager = GetComponent<CharacterCombatManager>();
            characterStatsManager = GetComponent<CharacterStatsManager>();
            characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
            characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
            characterUIManager = GetComponent<CharacterUIManager>();
        }

        protected virtual void Start() 
        {
            IgnoreMyOwnColliders();
        }

        protected virtual void Update()
        {
            animator.SetBool("isGrounded", characterLocomotionManager.isGrounded);
            // If this character is being controlled from our side, then assign its network position to the position of our transform
            if (IsOwner)
            {
                characterNetworkManager.networkPosition.Value = transform.position;
                characterNetworkManager.networkRotation.Value = transform.rotation;
            }
            // If this character is being controlled from else where, then assign its position here locally by the position of its network transform
            else
            {
                // Position
                transform.position = Vector3.SmoothDamp(transform.position, characterNetworkManager.networkPosition.Value,
                                                                            ref characterNetworkManager.networkPositionVelocity,
                                                                            characterNetworkManager.networkPositionSmoothTime);

                //Rotation
                transform.rotation = Quaternion.Slerp(transform.rotation, characterNetworkManager.networkRotation.Value,
                                                                          characterNetworkManager.networkRotationSmoothTime);
            }
        }

        protected virtual void FixedUpdate() 
        {

        }

        protected virtual void LateUpdate()
        {

        }

        protected virtual void OnEnable() 
        {

        }

        protected virtual void OnDisable()
        {

        }


        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();

            characterNetworkManager.OnIsMovingChanged(false, characterNetworkManager.isMoving.Value);
            characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);
            isDead.OnValueChanged += characterNetworkManager.OnIsDeadChanged;
            characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
        }

        public override void OnNetworkDespawn()
        {
            base.OnNetworkDespawn();
            isDead.OnValueChanged -= characterNetworkManager.OnIsDeadChanged;
            characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
            characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
        }
        public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false) 
        {
            if (IsOwner)
            {
                characterNetworkManager.currentHealth.Value = 0;
                isDead.Value = true;

                // Reset any flags here that need to be reset

                // If we are not grounded, play an aerial death animation

                if (!manuallySelectDeathAnimation && !characterNetworkManager.isBeingCriticallyDamage.Value)
                {
                    characterAnimatorManager.PlayTargetActionAnimation("Dead_01", true);
                }
            }
                //Play some Death SFX

                yield return new WaitForSeconds(5);

                //Award players with runes

                //Disable Character
        }

        public virtual void ReviveCharacter() 
        {

        }

        protected virtual void IgnoreMyOwnColliders() 
        {
            Collider characterControllerCollider = GetComponent<Collider>();
            Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();
            List<Collider> ignoreColliders = new List<Collider>();

            //Add all of our damageable character colliders, to the list that will be used to ignore collisions
            foreach (var collider in damageableCharacterColliders) 
            {
                ignoreColliders.Add(collider);
            }

            //Add our character controller collider to the list that will be used to ignore collisions
            ignoreColliders.Add(characterControllerCollider);

            //Goes through every collider on the list, and ignores collision with each other
            foreach (var collider in ignoreColliders) 
            {
                foreach (var otherCollider in ignoreColliders) 
                {
                    Physics.IgnoreCollision(collider, otherCollider, true);
                }
            }
        }

    }
}
