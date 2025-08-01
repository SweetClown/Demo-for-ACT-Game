using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldUtilityManager : MonoBehaviour
    {
        public static WorldUtilityManager Instance;

        [Header("Layers")]
        [SerializeField] LayerMask characterLayers;
        [SerializeField] LayerMask enviroLayers;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else 
            {
                Destroy(gameObject);
            }
        }

        public LayerMask GetCharacterLayers() 
        {
            return characterLayers;
        }

        public LayerMask GetEnviroLayers() 
        {
            return enviroLayers;
        }

        public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter) 
        {
            if (attackingCharacter == CharacterGroup.Team01)
            {
                switch (targetCharacter) 
                {
                    case CharacterGroup.Team01: return false; 
                    case CharacterGroup.Team02: return true;
                    default:
                        break;
                }
            }
            else if (attackingCharacter == CharacterGroup.Team02)
            {
                switch (targetCharacter)
                {
                    case CharacterGroup.Team01: return true;
                    case CharacterGroup.Team02: return false;
                    default:
                        break;
                }
            }

            return false;
        }

        public float GetAngleOfTarget(Transform characterTransform, Vector3 targetsDirection) 
        {
            targetsDirection.y = 0;
            float viewableAngle = Vector3.Angle(characterTransform.forward, targetsDirection);
            Vector3 cross = Vector3.Cross(characterTransform.forward, targetsDirection);

            if (cross.y < 0) viewableAngle = -viewableAngle;

            return viewableAngle;
        }

        public DamageIntensity GetDamageIntensityBasedOnPoiseDamage(float poiseDamage) 
        {
            //Throwing Stone, small items
            DamageIntensity damageIntensity = DamageIntensity.Ping;

            //Daggers
            if (poiseDamage >= 10)
                damageIntensity = DamageIntensity.Light;

            //Standard weapons
            if (poiseDamage >= 30)
                damageIntensity = DamageIntensity.Medium;

            //Great Sword, Hammer
            if (poiseDamage >= 70)
                damageIntensity = DamageIntensity.Heavy;

            //Ultra Sword
            if (poiseDamage >= 120)
                damageIntensity = DamageIntensity.Colossal;


            return damageIntensity;
        }

        public Vector3 GetRipostingPositionBasedOnWeaponClass(WeaponClass weaponClass) 
        {
            Vector3 position = new Vector3(0.11f, 0, 1.7f);

            switch (weaponClass)
            {
                case WeaponClass.StraightSword:
                    break;
                case WeaponClass.NightSkySword:
                    break;
                case WeaponClass.Fist:
                    break;
            }

            return position;
        }

        public Vector3 GetBackstabbingPositionBasedOnWeaponClass(WeaponClass weaponClass) 
        {
            Vector3 position = new Vector3(0.12f, 0, 1f);

            switch (weaponClass)
            {
                case WeaponClass.StraightSword:
                    break;
                case WeaponClass.NightSkySword:
                    break;
                case WeaponClass.Fist:
                    break;
            }

            return position;
        }
    }
}
