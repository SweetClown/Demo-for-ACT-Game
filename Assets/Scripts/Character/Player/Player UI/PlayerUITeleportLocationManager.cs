using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace SG
{
    public class PlayerUITeleportLocationManager : PlayerUIMenu
    {
        [SerializeField] GameObject[] teleportLocations;

        public override void OpenMenu()
        {
            base.OpenMenu();

            CheckForUnlockedTeleports();
        }

        private void CheckForUnlockedTeleports() 
        {
            bool hasFirstSelectedButton = false;

            for (int i = 0; i < teleportLocations.Length; i++) 
            {
                for (int s = 0; s < WorldObjectManager.instance.baceons.Count; s++) 
                {
                    if (WorldObjectManager.instance.baceons[s].BaceonID == i) 
                    {
                        if (WorldObjectManager.instance.baceons[s].isActivated.Value)
                        {
                            teleportLocations[i].SetActive(true);

                            if (!hasFirstSelectedButton) 
                            {
                                hasFirstSelectedButton = true;
                                teleportLocations[i].GetComponent<Button>().Select();
                                teleportLocations[i].GetComponent<Button>().OnSelect(null);
                            }
                        }
                        else 
                        {
                            teleportLocations[i].SetActive(false);
                        }
                    }
                }
            }
        }

        public void TeleportToBaceon(int baceonID) 
        {
            for (int i = 0; i < WorldObjectManager.instance.baceons.Count; i++) 
            {
                if (WorldObjectManager.instance.baceons[i].BaceonID == baceonID) 
                {
                    //Teleport
                    WorldObjectManager.instance.baceons[i].TeleportToBaceon();
                    return;
                }
            }
        }
    }
}
