using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Character Effects/Instant Effects/Take Stamina Damage")]
    public class TakeStaminaDamageEffect : InstantCharacterEffect
    {
        public float staminaDamage;

        public override void ProcessEffect(CharacterManager character)
        {
            CalcuateStaminaDamage(character);
        }

        private void CalcuateStaminaDamage(CharacterManager character) 
        {
            if (character.IsOwner) 
            {
                Debug.Log("Character is taking " + staminaDamage + " Stamina Damage");
                character.characterNetworkManager.currentStamina.Value -= staminaDamage;
            }
        }
    }
}
