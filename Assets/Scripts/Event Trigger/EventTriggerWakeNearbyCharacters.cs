using System.Collections.Generic;
using Unity.Loading;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;

namespace SweetClown
{
    public class EventTriggerWakeNearbyCharacters : MonoBehaviour
    {
        [SerializeField] float awakenRadius = 8;

        private void OnTriggerEnter(Collider other)
        {
            if (!NetworkManager.Singleton.IsServer)
                return;

            PlayerManager player = other.GetComponent<PlayerManager>();

            if (player == null)
                return;

            Collider[] creaturesInRadus = Physics.OverlapSphere(transform.position, awakenRadius, WorldUtilityManager.Instance.GetCharacterLayers());
            List<AICharacterManager> creaturesToWake = new List<AICharacterManager>();

            for (int i = 0; i < creaturesInRadus.Length; i++) 
            {
                AICharacterManager aiCharacter = creaturesInRadus[i].GetComponentInParent<AICharacterManager>();

                if (aiCharacter == null) 
                    continue;

                if (aiCharacter.isDead.Value)
                    continue;

                if (aiCharacter.AICharacterNetworkManager.isAwake.Value)
                    continue;

                if (!creaturesToWake.Contains(aiCharacter))
                    creaturesToWake.Add(aiCharacter);
            }

            for (int i = 0; i < creaturesToWake.Count; i++) 
            {
                creaturesToWake[i].AICharacterCombatManager.SetTarget(player);
            }
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(transform.position, awakenRadius);
        }
    }
}
