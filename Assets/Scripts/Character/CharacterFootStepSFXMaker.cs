using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class CharacterFootStepSFXMaker : MonoBehaviour
    {
        CharacterManager character;

        AudioSource audioSource;
        GameObject steppedOnObject;

        public bool hasTouchedGround = false;
        public bool hasPlayedFootStepSFX = false;
        [SerializeField] float distanceToGround = 0.05f;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();

            character = GetComponentInParent<CharacterManager>();
        }

        private void FixedUpdate()
        {
            CheckForFootSteps();
        }

        private void CheckForFootSteps() 
        {
            if (character == null)
                return;

            if (!character.characterNetworkManager.isMoving.Value)
                return;

            RaycastHit hit;

            if (Physics.Raycast(transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.Instance.GetEnviroLayers()))
            {
                hasTouchedGround = true;

                if (!hasPlayedFootStepSFX)
                    steppedOnObject = hit.transform.gameObject;
            }
            else 
            {
                hasTouchedGround = false;
                hasPlayedFootStepSFX = false;
                steppedOnObject = null;
            }

            if (hasTouchedGround && !hasPlayedFootStepSFX) 
            {
                hasPlayedFootStepSFX = true;
                PlayFootStepSoundFX();
            }
        }

        private void PlayFootStepSoundFX() 
        {
            // audioSource.PlayOneShot(WorldSoundFXManager.instance.ChooseRandomFootStepSoundBasedOnGround(steppedOnObject, character));

            character.characterSoundFXManager.PlayFootStepSoundFX();
        }
    }
}
