using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Linq;
using Unity.VisualScripting;

namespace SweetClown
{
    public class WorldAIManager : MonoBehaviour
    {
        public static WorldAIManager instance;

        [Header("Loading")]
        public bool isPerformingLoadingOperation = false;

        [Header("Characters")]
        [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
        [SerializeField] List<AICharacterManager> spawnedInCharacters;
        private Coroutine spawnAllCharactersCoroutine;
        private Coroutine despawnAllCharactersCoroutine;
        private Coroutine resetAllCharactersCoroutine;

        [Header("Range Prefab")]
        public GameObject rangeGameObject;

        [Header("Bosses")]
        [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

        [Header("Patrol Paths")]
        [SerializeField] List<AIPatrolPath> aiPatrolPaths = new List<AIPatrolPath>();


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
        }

        public void SpawnCharacters(AICharacterSpawner aiCharacterSpawner)
        {
            if (NetworkManager.Singleton.IsServer) 
            {
                aiCharacterSpawners.Add(aiCharacterSpawner);
                aiCharacterSpawner.AttemptToSpawnCharacter();
            }
        }

        public void AddCharacterToSpawnedCharacterList(AICharacterManager character) 
        {
            if (spawnedInCharacters.Contains(character))
                return;

            spawnedInCharacters.Add(character);

            AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

            if (bossCharacter != null) 
            {
                if (spawnedInBosses.Contains(bossCharacter))
                    return;

                spawnedInBosses.Add(bossCharacter);
            }
        }

        public AIBossCharacterManager GetBossCharacterByID(int ID) 
        {
            return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);
        }

        public void SpawnAllCharacters() 
        {
            isPerformingLoadingOperation = true;

            if (spawnAllCharactersCoroutine != null)
                StopCoroutine(spawnAllCharactersCoroutine);

            spawnAllCharactersCoroutine = StartCoroutine(SpawnAllCharacterCoroutine());
        }

        public IEnumerator SpawnAllCharacterCoroutine()
        {
            for (int i = 0; i < aiCharacterSpawners.Count; i++) 
            {
                yield return new WaitForFixedUpdate();

                aiCharacterSpawners[i].AttemptToSpawnCharacter();

                yield return null;
            }

            isPerformingLoadingOperation = false;

            yield return null;
        }

        public void ResetAllCharacters() 
        {
            isPerformingLoadingOperation = true;

            if (resetAllCharactersCoroutine != null)
                StopCoroutine(resetAllCharactersCoroutine);

            resetAllCharactersCoroutine = StartCoroutine(ResetAllCharactersCoroutine());
        }

        private IEnumerator ResetAllCharactersCoroutine()
        {
            for (int i = 0; i < aiCharacterSpawners.Count; i++)
            {
                yield return new WaitForFixedUpdate();

                aiCharacterSpawners[i].ResetCharacter();

                yield return null;
            }

            isPerformingLoadingOperation = false;

            yield return null;

        }

        private void DespawnAllCharacters() 
        {
            isPerformingLoadingOperation = true;

            if (despawnAllCharactersCoroutine != null)
                StopCoroutine(despawnAllCharactersCoroutine);

            spawnAllCharactersCoroutine = StartCoroutine(DespawnAllCharactersCoroutine());

        }

        private IEnumerator DespawnAllCharactersCoroutine()
        {
            for (int i = 0; i < spawnedInCharacters.Count; i++)
            {
                yield return new WaitForFixedUpdate();

                spawnedInCharacters[i].GetComponent<NetworkObject>().Despawn();

                yield return null;
            }

            spawnedInCharacters.Clear();
            isPerformingLoadingOperation = false;

            yield return null;

        }

        private void DisableAllCharacter() 
        {

        }

        // Patrol Paths
        public void AddPatrolPathToList(AIPatrolPath patrolPath) 
        {
            if (aiPatrolPaths.Contains(patrolPath))
                return;

            aiPatrolPaths.Add(patrolPath);
        }

        public AIPatrolPath GetAIPatrolPathByID(int patrolPathID) 
        {
            AIPatrolPath patrolPath = null;

            for (int i = 0; i < aiPatrolPaths.Count; i++) 
            {
                if (aiPatrolPaths[i].patrolPathID == patrolPathID)
                    patrolPath = aiPatrolPaths[i];
            }

            return patrolPath;
        }
    }
}
