using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterEffectsManager : MonoBehaviour
    {
        //Process instant effects (Take Damage, heal)

        //Process timed effects (Poison, Build ups)

        //Process static effects (Adding/Removing buffs)

        CharacterManager character;

        [Header("VFX")]
        [SerializeField] GameObject bloodSplatterVFX;
        [SerializeField] GameObject criticalBloodSplatterVFX;

        [Header("Static Effects")]
        public List<StaticCharacterEffect> staticCharacterEffects = new List<StaticCharacterEffect>();

        [Header("Currect Active FX")]
        public GameObject activeQuickSlotItemFX;
        public GameObject activeSpellWarmUpFX;

        protected virtual void Awake() 
        {
            character = GetComponent<CharacterManager>();
        }

        public void ProcesssInstantEffect(InstantCharacterEffect effect) 
        {
            //Take in an effect
            //Process it
            effect.ProcessEffect(character);
        }

        public void PlayBloodSplatterVFX(Vector3 contactPoint) 
        {
            //If we manually have placed a blood splatter vfx on this model, play its version
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            //else, use the default version we have elsewhere
            else 
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

        public void PlayCriticalBloodSplatterVFX(Vector3 contactPoint)
        {
            //If we manually have placed a blood splatter vfx on this model, play its version
            if (bloodSplatterVFX != null)
            {
                GameObject bloodSplatter = Instantiate(criticalBloodSplatterVFX, contactPoint, Quaternion.identity);
            }
            //else, use the default version we have elsewhere
            else
            {
                GameObject bloodSplatter = Instantiate(WorldCharacterEffectsManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            }
        }

        public void AddStaticEffect(StaticCharacterEffect effect) 
        {
            //Add a static effect to the character
            staticCharacterEffects.Add(effect);

            //Process its effect
            effect.ProcessStaticEffect(character);

            //Check for null entries in the list and remove them
            for (int i = staticCharacterEffects.Count - 1; i > -1; i--) 
            {
                if (staticCharacterEffects[i] == null) 
                {
                    staticCharacterEffects.RemoveAt(i);
                }
            }
        }

        public void RemoveStaticEffect(int effectID) 
        {
            StaticCharacterEffect effect;

            for (int i = 0; i < staticCharacterEffects.Count; i++) 
            {
                if (staticCharacterEffects[i] != null) 
                {
                    if (staticCharacterEffects[i].staticEffectID == effectID) 
                    {
                        effect = staticCharacterEffects[i];
                        //Remove a static effect from the character
                        effect.RemoveStaticEffect(character);
                        //Remove a static effect from the List
                        staticCharacterEffects.Remove(effect);
                    }
                }
            }
        }
    }
}
