using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEditor.ShaderGraph.Serialization;

namespace SG
{
    public class AICharacterSpawner : MonoBehaviour
    {
        [Header("Character")]
        [SerializeField] GameObject characterGameObject;
        [SerializeField] GameObject instantiatedGameObject;
        private AICharacterManager aiCharacter;

        [Header("Patrol")]
        [SerializeField] bool hasPatrolPath = false;
        [SerializeField] int patrolPathID = 0;

        [Header("Sleep")]
        [SerializeField] bool isSleeping = false;

        private void Awake()
        {
        }

        private void Start()
        {
            WorldAIManager.instance.SpawnCharacters(this);
            gameObject.SetActive(false);
        }

        public void AttemptToSpawnCharacter()
        {
            if (characterGameObject != null)
            {
                instantiatedGameObject = Instantiate(characterGameObject);
                instantiatedGameObject.transform.position = transform.position;
                instantiatedGameObject.transform.rotation = transform.rotation;
                instantiatedGameObject.GetComponent<NetworkObject>().Spawn();
                aiCharacter = instantiatedGameObject.GetComponent<AICharacterManager>();

                if (aiCharacter == null)
                    return;

                WorldAIManager.instance.AddCharacterToSpawnedCharacterList(aiCharacter);

                if (hasPatrolPath)
                    aiCharacter.idle.aiPatrolPath = WorldAIManager.instance.GetAIPatrolPathByID(patrolPathID);

                if (isSleeping)
                    aiCharacter.AICharacterNetworkManager.isAwake.Value = false;

                aiCharacter.AICharacterNetworkManager.isActive.Value = false;
            }
        }

        public void ResetCharacter()
        {
            if (instantiatedGameObject == null)
                return;

            if (aiCharacter == null)
                return;

            instantiatedGameObject.transform.position = transform.position;
            instantiatedGameObject.transform.rotation = transform.rotation;
            aiCharacter.AICharacterNetworkManager.currentHealth.Value = aiCharacter.AICharacterNetworkManager.maxHealth.Value;
            aiCharacter.AICharacterCombatManager.SetTarget(null);

            if (aiCharacter.isDead.Value)
            {
                aiCharacter.isDead.Value = false;
                aiCharacter.characterAnimatorManager.PlayTargetActionAnimation("Empty", false, false, true, true, true, true);
                aiCharacter.currentState.SwitchState(aiCharacter, aiCharacter.idle);
            }

            aiCharacter.characterUIManager.ResetCharacterHPBar();
        }
    }
}
