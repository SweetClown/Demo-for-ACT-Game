using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class WorldCharacterEffectsManager : MonoBehaviour
    {
        public static WorldCharacterEffectsManager instance;

        [Header("VFX")]
        public GameObject bloodSplatterVFX;
        public GameObject criticalBloodSplatterVFX;
        public GameObject healingPoisonVFX;
        public GameObject DeadSpotVFX;

        [Header("Damage")]
        public TakeDamageEffect takeDamageEffect;
        public TakeBlockedDamageEffect takeBlockedDamageEffect;
        public TakeCriticalDamageEffect takeCriticalDamageEffect;

        [Header("Two Hand")]
        public TwoHandEffect twoHandEffect;

        [Header("Instant Effects")]
        [SerializeField] List<InstantCharacterEffect> instantEffects;

        [Header("Static Effects")]
        [SerializeField] List<StaticCharacterEffect> staticEffects;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else 
            {
                Destroy(gameObject);
            }

            GenerateEffectIDs();
        }

        private void GenerateEffectIDs() 
        {
            for (int i = 0; i < instantEffects.Count; i++) 
            {
                instantEffects[i].instantEffectID = i;
            }

            for (int i = 0; i < staticEffects.Count; i++)
            {
                staticEffects[i].staticEffectID = i;
            }
        }
    }
}
