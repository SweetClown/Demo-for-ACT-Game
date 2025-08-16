using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace SweetClown
{
    public class WorldObjectManager : MonoBehaviour
    {
        public static WorldObjectManager instance;


        [Header("Objects")]
        [SerializeField] List<NetworkObjectSpawner> networkObjectSpawner;
        [SerializeField] List<GameObject> spawnedInObjects;

        [Header("Fog Walls")]
        [SerializeField] public List<FogWallInteractable> fogWalls;

        [Header("Baceon")]
        [SerializeField] public List<BaceonInteractable> baceons;

        // 2. Spawn in those fogwalls as network Objects during start of game (Must have a spawner object)
        // 3. Create general object spawner script and prefab
        // 4. When the fog walls are spawned, add them to the world fog wall list
        // 5. Grab the correct fogwall from the list on the boss manager when the boss is being initialized

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

        public void SpawnObject(NetworkObjectSpawner networkObjectSpawner)
        {
            if (NetworkManager.Singleton.IsServer)
            {
                this.networkObjectSpawner.Add(networkObjectSpawner);
                networkObjectSpawner.AttemptToSpawnCharacter();
            }
        }

        public void AddFogWallToList(FogWallInteractable fogWall) 
        {
            if (!fogWalls.Contains(fogWall)) 
            {
                fogWalls.Add(fogWall);
            }
        }

        public void RemoveFogWallFromList(FogWallInteractable fogWall)
        {
            if (fogWalls.Contains(fogWall))
            {
                fogWalls.Remove(fogWall);
            }
        }

        public void AddBaceonToList(BaceonInteractable baceon)
        {
            if (!baceons.Contains(baceon))
            {
                baceons.Add(baceon);
            }
        }

        public void RemoveBaceonFromList(BaceonInteractable baceon)
        {
            if (baceons.Contains(baceon))
            {
                baceons.Remove(baceon);
            }
        }
    }
}
