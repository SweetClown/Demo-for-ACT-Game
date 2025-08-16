using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace SweetClown
{
    public class WorldGameSessionManager : MonoBehaviour
    {
        public static WorldGameSessionManager instance;

        [Header("Active Players In Session")]
        public List<PlayerManager> players = new List<PlayerManager>();

        private Coroutine revivalCoroutien;

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

        public void WaitThenReviveHost() 
        {
            if (revivalCoroutien != null)
                StopCoroutine(revivalCoroutien);

            revivalCoroutien = StartCoroutine(ReviveHostCoroutine(5));
        }

        private IEnumerator ReviveHostCoroutine(float delay) 
        {
            yield return new WaitForSeconds(delay);

            PlayerUIManager.instance.playerUILoadingScreenManager.ActiveateLoadingScreen();

            PlayerUIManager.instance.localPlayer.ReviveCharacter();

            for (int i = 0; i < WorldObjectManager.instance.baceons.Count; i++) 
            {
                if (WorldObjectManager.instance.baceons[i].BaceonID == WorldSaveGameManager.instance.currentCharacterData.lastBaceonRestedAt) 
                {
                    WorldObjectManager.instance.baceons[i].TeleportToBaceon();
                    break;
                }
            }
        }
        public void AddPlayerToActivePlayersList(PlayerManager player)
        {
            //Check the list , if it does not already contain the player, add then
            if (!players.Contains(player))
            {
                players.Add(player);
            }

            //Check the list for null slots, and remove the null slots

            for (int i = players.Count - 1; i > -1; i--) 
            {
                if (players[i] == null) 
                {
                    players.RemoveAt(i);
                }
            }
        }
        

        public void RemovePlayerFromActivePlayersList(PlayerManager player) 
        {
            //Check the list , if it does contain the player, remove then
            if (!players.Contains(player))
            {
                players.Remove(player);
            }

            //Check the list for null slots, and remove the null slots
            for (int i = players.Count - 1; i > -1; i--)
            {
                if (players[i] == null)
                {
                    players.RemoveAt(i);
                }
            }
        }
    }
}
