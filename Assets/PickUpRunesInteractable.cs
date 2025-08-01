using System.Collections;
using UnityEngine;


namespace SG
{
    public class PickUpRunesInteractable : Interactable
    {
        public int runeCount = 0;

        public override void Interact(PlayerManager player)
        {
            if (player.isDead.Value)
                return;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            WorldSaveGameManager.instance.currentCharacterData.hasDeadSpot = false;

            //Play sfx
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemSFX);

            //Play An animation
            player.playerAnimatorManager.PlayTargetActionAnimation("Pick_Up_Item_01", true);

            //Add runes
            player.playerStatsManager.AddRunes(runeCount);

            Destroy(gameObject);
        }
    }
}
