using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SweetClown
{
    public class AIBossSoundFXManager : CharacterSoundFXManager
    {
        [Header("Sword Whooshes")]
        public AudioClip[] swordWhooshes;

        [Header("Sword Impacts")]
        public AudioClip[] swordImpacts;

        [Header("Stomp Impacts")]
        public AudioClip[] stompImpacts;

        public virtual void PlaySwordImpactSoundFX() 
        {
            if (swordImpacts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(swordImpacts));
        }

        public virtual void PlayStompImpactSoundFX()
        {
            if (stompImpacts.Length > 0)
                PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(stompImpacts));
        }

    }
}
