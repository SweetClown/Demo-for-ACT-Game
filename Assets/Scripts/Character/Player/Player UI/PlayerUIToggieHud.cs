using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{
    public class PlayerUIToggieHud : MonoBehaviour
    {
        private void OnEnable()
        {
            //Hide the hud
            PlayerUIManager.instance.playerUIHudManager.ToggleHUD(false);
        }

        private void OnDisable()
        {
            //Open the hud
            PlayerUIManager.instance.playerUIHudManager.ToggleHUD(true);
        }
    }
}
