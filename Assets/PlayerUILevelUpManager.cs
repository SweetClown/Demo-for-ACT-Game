using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System.Linq;
using UnityEngine.TextCore.Text;
using Unity.Loading;
using UnityEditor.Experimental.GraphView;

namespace SG
{
    public class PlayerUILevelUpManager : PlayerUIMenu
    {
        [Header("Levels")]
        [SerializeField] int[] playerLevels = new int[100];
        [SerializeField] int baseLevelCost = 83;
        [SerializeField] int totalLevelUpCost = 0;

        [Header("Character Stats")]
        [SerializeField] TextMeshProUGUI characterLevelText;
        [SerializeField] TextMeshProUGUI runesHeldText;
        [SerializeField] TextMeshProUGUI totalCostText;
        [SerializeField] TextMeshProUGUI vigorLevelText;
        [SerializeField] TextMeshProUGUI mindLevelText;
        [SerializeField] TextMeshProUGUI enduranceLevelText;
        [SerializeField] TextMeshProUGUI strengthLevelText;
        [SerializeField] TextMeshProUGUI dexterityLevelText;
        [SerializeField] TextMeshProUGUI intelligenceLevelText;
        [SerializeField] TextMeshProUGUI faithLevelText;

        [Header("Projected Character Stats")]
        [SerializeField] TextMeshProUGUI projectedCharacterLevelText;
        [SerializeField] TextMeshProUGUI projectedRunesHeldText;
        [SerializeField] TextMeshProUGUI projectedVigorLevelText;
        [SerializeField] TextMeshProUGUI projectedMindLevelText;
        [SerializeField] TextMeshProUGUI projectedEnduranceLevelText;
        [SerializeField] TextMeshProUGUI projectedStrengthLevelText;
        [SerializeField] TextMeshProUGUI projectedDexterityLevelText;
        [SerializeField] TextMeshProUGUI projectedIntelligenceLevelText;
        [SerializeField] TextMeshProUGUI projectedFaithLevelText;

        [Header("Sliders")]
        public CharacterAttribute currentSelectedAttribute;
        public Slider vigorSlider;
        public Slider mindSlider;
        public Slider enduranceSlider;
        public Slider strengthSlider;
        public Slider dexteritySlider;
        public Slider intelligenceSlider;
        public Slider faithSlider;

        [Header("Buttons")]
        [SerializeField] Button confirmLevelsButton;

        private void Awake()
        {
            SetAllLevelsCost();
        }

        public override void OpenMenu()
        {
            base.OpenMenu();
            SetCurrentStats();
        }

        private void SetCurrentStats()
        {
            characterLevelText.text = PlayerUIManager.instance.localPlayer.characterStatsManager.CalculateCharacterLevelBasedOnAttributes().ToString();
            projectedCharacterLevelText.text = PlayerUIManager.instance.localPlayer.characterStatsManager.CalculateCharacterLevelBasedOnAttributes().ToString();

            runesHeldText.text = PlayerUIManager.instance.localPlayer.playerStatsManager.runes.ToString();
            projectedRunesHeldText.text = PlayerUIManager.instance.localPlayer.playerStatsManager.runes.ToString();
            totalCostText.text = "0";

            vigorLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.vigor.Value.ToString();
            projectedVigorLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.vigor.Value.ToString();
            vigorSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.vigor.Value;

            mindLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.mind.Value.ToString();
            projectedMindLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.mind.Value.ToString();
            mindSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.mind.Value;

            enduranceLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.endurance.Value.ToString();
            projectedEnduranceLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.endurance.Value.ToString();
            enduranceSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.endurance.Value;

            strengthLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.strength.Value.ToString();
            projectedStrengthLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.strength.Value.ToString();
            strengthSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.strength.Value;

            dexterityLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.dexterity.Value.ToString();
            projectedDexterityLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.dexterity.Value.ToString();
            dexteritySlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.dexterity.Value;

            intelligenceLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.intelligence.Value.ToString();
            projectedIntelligenceLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.intelligence.Value.ToString();
            intelligenceSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.intelligence.Value;

            faithLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.faith.Value.ToString();
            projectedFaithLevelText.text = PlayerUIManager.instance.localPlayer.playerNetworkManager.faith.Value.ToString();
            faithSlider.minValue = PlayerUIManager.instance.localPlayer.playerNetworkManager.faith.Value;

            vigorSlider.Select();
            vigorSlider.OnSelect(null);
        }

        //This is called every time a level slider is change
        public void UpdateSliderBasedOnCurrentSelectedAttribut()
        {
            PlayerManager player = PlayerUIManager.instance.localPlayer;

            switch (currentSelectedAttribute)
            {
                case CharacterAttribute.Vigor:
                    projectedVigorLevelText.text = vigorSlider.value.ToString();
                    break;
                case CharacterAttribute.Mind:
                    projectedMindLevelText.text = mindSlider.value.ToString();
                    break;
                case CharacterAttribute.Endurance:
                    projectedEnduranceLevelText.text = enduranceSlider.value.ToString();
                    break;
                case CharacterAttribute.Strength:
                    projectedStrengthLevelText.text = strengthSlider.value.ToString();
                    break;
                case CharacterAttribute.Dexterity:
                    projectedDexterityLevelText.text = dexteritySlider.value.ToString();
                    break;
                case CharacterAttribute.Intelligence:
                    projectedIntelligenceLevelText.text = intelligenceSlider.value.ToString();
                    break;
                case CharacterAttribute.Faith:
                    projectedFaithLevelText.text = faithSlider.value.ToString();
                    break;
            }

            CalculateLevelCost(player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes()
                              , player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes(true));

            projectedCharacterLevelText.text = player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes(true).ToString();
            totalCostText.text = totalLevelUpCost.ToString();

            //Check Cost
            if (totalLevelUpCost > player.playerStatsManager.runes)
            {
                //Disable Confirm Button so you can.t level up
                //Change Level up fields text to red
                confirmLevelsButton.interactable = false;
            }
            else
            {
                confirmLevelsButton.interactable = true;
            }

            //Changes projected stats text colors to reflect feec back depending on if we can afford the level or not
            ChangeTextColorsDependingOnCosts();
        }

        public void ConfirmLevels()
        {
            PlayerManager player = PlayerUIManager.instance.localPlayer;

            player.playerStatsManager.runes -= totalLevelUpCost;

            player.playerNetworkManager.vigor.Value = Mathf.RoundToInt(vigorSlider.value);
            player.playerNetworkManager.mind.Value = Mathf.RoundToInt(mindSlider.value);
            player.playerNetworkManager.endurance.Value = Mathf.RoundToInt(enduranceSlider.value);
            player.playerNetworkManager.strength.Value = Mathf.RoundToInt(strengthSlider.value);
            player.playerNetworkManager.dexterity.Value = Mathf.RoundToInt(dexteritySlider.value);
            player.playerNetworkManager.intelligence.Value = Mathf.RoundToInt(intelligenceSlider.value);
            player.playerNetworkManager.faith.Value = Mathf.RoundToInt(faithSlider.value);

            SetCurrentStats();
            ChangeTextColorsDependingOnCosts();

            //Save Game after upgrade the level
            WorldSaveGameManager.instance.SaveGame();
        }

        private void SetAllLevelsCost()
        {
            for (int i = 0; i < playerLevels.Length; i++)
            {
                if (i == 0)
                    continue;

                playerLevels[i] = baseLevelCost + (50 * i);
            }
        }

        private void CalculateLevelCost(int currentLevel, int projectedLevel)
        {
            //We don.t want to charge for levels wea lready paid for
            int totalCost = 0;

            for (int i = 0; i < projectedLevel; i++)
            {
                //Do not charge until we get past our current level
                if (i < currentLevel)
                    continue;

                if (i > playerLevels.Length)
                    continue;

                totalCost += playerLevels[i];
            }

            totalLevelUpCost = totalCost;

            projectedRunesHeldText.text = (PlayerUIManager.instance.localPlayer.playerStatsManager.runes - totalCost).ToString();

            if (totalCost > PlayerUIManager.instance.localPlayer.playerStatsManager.runes)
            {
                projectedRunesHeldText.color = Color.red;
            }
            else 
            {
                projectedRunesHeldText.color= Color.white;
            }
        }

        //To red (if you can.t afford and the state is higher than the current level)
        //To blue (if the stat is higher can you can afford it)
        //To white (if the stat is unchanged)
        private void ChangeTextColorsDependingOnCosts() 
        {
            PlayerManager player = PlayerUIManager.instance.localPlayer;

            int projectedVigorLevel = Mathf.RoundToInt(vigorSlider.value);
            int projectedMindLevel = Mathf.RoundToInt(mindSlider.value);
            int projectedEnduranceLevel = Mathf.RoundToInt(enduranceSlider.value);
            int projectedStrengthLevel = Mathf.RoundToInt(strengthSlider.value);
            int projectedDexterityLevel = Mathf.RoundToInt(dexteritySlider.value);
            int projectedIntelligenceLevel = Mathf.RoundToInt(intelligenceSlider.value);
            int projectedFaithLevel = Mathf.RoundToInt(faithSlider.value);

            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedVigorLevelText, player.playerNetworkManager.vigor.Value, projectedVigorLevel);
            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedMindLevelText, player.playerNetworkManager.mind.Value, projectedMindLevel);
            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedEnduranceLevelText, player.playerNetworkManager.endurance.Value, projectedEnduranceLevel);
            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedStrengthLevelText, player.playerNetworkManager.strength.Value, projectedStrengthLevel);
            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedDexterityLevelText, player.playerNetworkManager.dexterity.Value, projectedDexterityLevel);
            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedIntelligenceLevelText, player.playerNetworkManager.intelligence.Value, projectedIntelligenceLevel);
            ChangeTextFieldToSpecificColorBasedOnStat(player, projectedFaithLevelText, player.playerNetworkManager.faith.Value, projectedFaithLevel);

            int projectedPlayerLevel = player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes(true);
            int playerLevel = player.characterStatsManager.CalculateCharacterLevelBasedOnAttributes();

            if (projectedPlayerLevel == playerLevel)
            {
                projectedCharacterLevelText.color = Color.white;
                projectedRunesHeldText.color = Color.white;
                totalCostText.color = Color.white;
            }

            if (totalLevelUpCost <= player.playerStatsManager.runes)
            {
                totalCostText.color = Color.white;

                if (projectedPlayerLevel > playerLevel) 
                {
                    projectedRunesHeldText.color = Color.red;
                    projectedCharacterLevelText.color = Color.blue ;
                }

            }
            else 
            {
                totalCostText.color = Color.red;

                if (projectedPlayerLevel > playerLevel)
                {
                    projectedCharacterLevelText.color = Color.red;
                }
            }
        }

        private void ChangeTextFieldToSpecificColorBasedOnStat(PlayerManager player,TextMeshProUGUI textField, int stat, int projectedStat) 
        {
            if (projectedStat == stat)
                textField.color = Color.white;

            //Can Afford It
            if (totalLevelUpCost <= player.playerStatsManager.runes)
            {
                //If our projected stat is higher, give the player feedback by changing its color indicating its new potential
                if (projectedStat > stat)
                {
                    textField.color = Color.blue;
                }
                else
                {
                    textField.color = Color.white;
                }
            }
            //Can.t Afford It
            else
            {
                if (projectedStat > stat)
                {
                    textField.color = Color.red;
                }
                else
                {
                    textField.color = Color.white;
                }
            }
        }
    }

}
