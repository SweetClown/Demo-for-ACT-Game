using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Items/Spells/FireBall Spell")]
    public class FireBallSpell : SpellItem
    {
        [Header("Projecttile Velocity")]
        [SerializeField] float upwardVelocity = 3;
        [SerializeField] float forwardVelocity = 15;

        public override void AttemptToCastSpell(PlayerManager player)
        {
            base.AttemptToCastSpell(player);

            if (!CanICastThisSpell(player))
                return;

            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerAnimatorManager.PlayTargetActionAnimation(mainHandSpellAnimation, true);
            }
            else
            {
                player.playerAnimatorManager.PlayTargetActionAnimation(offHandSpellAnimation, true);
            }
        }

        public override void InstantiateWarmUpSpellFX(PlayerManager player)
        {
            base.InstantiateWarmUpSpellFX(player);
            //Determine which hand player is using
            SpellInstantiationLocation spellInstantiationLocation;
            GameObject instantiatedWarmUpSpellFX = Instantiate(spellCastWarmUpFX);
            //Right Hand
            if (player.playerNetworkManager.isUsingRightHand.Value)
            { 
                //instantiate warm up fx on the correct place.
                spellInstantiationLocation = player.playerEquipmentManager.rightWeaponManager.GetComponentInChildren<SpellInstantiationLocation>();
            }
            //Left Hand
            else
            {
                //instantiate warm up fx on the correct place.
                spellInstantiationLocation = player.playerEquipmentManager.leftWeaponManager.GetComponentInChildren<SpellInstantiationLocation>();
            }

            instantiatedWarmUpSpellFX.transform.parent = spellInstantiationLocation.transform;
            instantiatedWarmUpSpellFX.transform.localPosition = Vector3.zero;
            instantiatedWarmUpSpellFX.transform.localRotation = new Quaternion(-60,-176,-258, 1);

            //Save the warm up fx as a viriable so it can be destoryed if the player is knocked out of the animation.
            player.playerEffectsManager.activeSpellWarmUpFX = instantiatedWarmUpSpellFX;
        }

        public override void SuccessfullyCastSpell(PlayerManager player)
        {
            base.SuccessfullyCastSpell(player);

            if (player.IsOwner)
                player.playerCombatManager.DestroyAllCurrentActionFX();

            //Determine which hand player is using
            SpellInstantiationLocation spellInstantiationLocation;
            GameObject instantiatedReleasedSpellFX = Instantiate(spellCastReleaseFX);

            //Right Hand
            if (player.playerNetworkManager.isUsingRightHand.Value)
            {
                //instantiate warm up fx on the correct place.
                spellInstantiationLocation = player.playerEquipmentManager.rightWeaponManager.GetComponentInChildren<SpellInstantiationLocation>();
            }
            //Left Hand
            else
            {
                //instantiate warm up fx on the correct place.
                spellInstantiationLocation = player.playerEquipmentManager.leftWeaponManager.GetComponentInChildren<SpellInstantiationLocation>();
            }

            instantiatedReleasedSpellFX.transform.parent = spellInstantiationLocation.transform;
            instantiatedReleasedSpellFX.transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion(-60, -176, -258, -1));
            instantiatedReleasedSpellFX.transform.parent = null;

            FireBallManager fireBallManager = instantiatedReleasedSpellFX.GetComponent<FireBallManager>();
            fireBallManager.InitializeFireBall(player);

            //Collider[] characterColliders = player.GetComponentsInChildren<Collider>();
            //Collider characterCollisionCollider = player.GetComponent<Collider>();


            //Use the list of colliders from the caster and now apply the ignore physics with the colliders from the projectile
            //Physics.IgnoreCollision(characterCollisionCollider, fireBallManager.damageColider.damageCollider, true);

            //foreach (var collider in characterColliders) 
            //{
            //    Physics.IgnoreCollision(collider, fireBallManager.damageColider.damageCollider, true);
            //}

            //Set the projectiles velocity and direction

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                instantiatedReleasedSpellFX.transform.LookAt(player.playerCombatManager.currentTarget.transform.position);
            }
            else 
            {
                Vector3 forwardDirection = player.transform.forward;
                instantiatedReleasedSpellFX.transform.forward = forwardDirection;
            }

            Rigidbody spellRigidbody = instantiatedReleasedSpellFX.GetComponent<Rigidbody>();
            Vector3 upwardVelocityVector = instantiatedReleasedSpellFX.transform.up * upwardVelocity;
            Vector3 forwardVelocityVector = instantiatedReleasedSpellFX.transform.forward * forwardVelocity;
            Vector3 totalVelocity = upwardVelocityVector + forwardVelocityVector;
            spellRigidbody.linearVelocity = totalVelocity;
            //Debug.Log(spellRigidbody.velocity);


        }

        public override bool CanICastThisSpell(PlayerManager player)
        {
            if (player.isPerformingAction)
                return false;

            if (player.playerNetworkManager.isJumping.Value)
                return false;

            if (player.playerNetworkManager.currentStamina.Value <= 0)
                return false;


            return true;
        }
    }
}   
