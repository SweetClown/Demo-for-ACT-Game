using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    [CreateAssetMenu(menuName = "Character Effects/Static Effects/Two Handing Effect")]
    public class TwoHandEffect : StaticCharacterEffect
    {
        [SerializeField] int strengthGainedFromTwoHandingWeapon;

        public override void ProcessStaticEffect(CharacterManager character)
        {
            base.ProcessStaticEffect(character);

            if (character.IsOwner) 
            {
                strengthGainedFromTwoHandingWeapon = Mathf.RoundToInt(character.characterNetworkManager.strength.Value / 2);
                Debug.Log("Strength Gained" + strengthGainedFromTwoHandingWeapon);
                character.characterNetworkManager.strengthModifier.Value += strengthGainedFromTwoHandingWeapon;
            }
        }

        public override void RemoveStaticEffect(CharacterManager character)
        {
            base.RemoveStaticEffect(character);

            if (character.IsOwner)
            {
                character.characterNetworkManager.strengthModifier.Value -= strengthGainedFromTwoHandingWeapon;
            }
        }
    }
}
