using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace SG
{
    //Performs Identically to the The UI_Stat Bar, Except this bar appears and disappears in world Space (Will Always Face Camera)
    public class UI_Character_HP_Bar : UI_StatBar
    {
        private CharacterManager character;
        private AICharacterManager aiCharacter;
        private PlayerManager playerCharacter;

        [SerializeField] bool displayCharacterNameOnDamage = false;
        [SerializeField] float defaultTimeBeforeBarHides = 3;
        [SerializeField] float hideTimer = 0;
        public int currentDamageTaken = 0;
        [SerializeField] TextMeshProUGUI characterName;
        [SerializeField] TextMeshProUGUI characterDamage;
        [HideInInspector] public int OldHealthValue = 0;

        protected override void Awake()
        {
            base.Awake();

            character = GetComponentInParent<CharacterManager>();

            if (character != null)
            {
                aiCharacter = character as AICharacterManager;
                playerCharacter = character as PlayerManager;
            }
        }

        protected override void Start()
        {
            base.Start();

            gameObject.SetActive(false);
        }

        private void Update()
        {
            transform.LookAt(transform.position + Camera.main.transform.forward);

            if (hideTimer > 0)
            {
                hideTimer -= Time.deltaTime;
            }
            else 
            {
                gameObject.SetActive(false);
            }
        }

        private void OnDisable()
        {
            currentDamageTaken = 0;
        }

        public override void SetStat(int newValue)
        {
            if (displayCharacterNameOnDamage) 
            {
                characterName.enabled = true;

                if (aiCharacter != null)
                    characterName.text = aiCharacter.characterName;

                if (playerCharacter != null)
                    characterName.text = playerCharacter.playerNetworkManager.characterName.Value.ToString();
            }

            //Call This here incase max health changes from a character effect/buff ect
            slider.maxValue = character.characterNetworkManager.maxHealth.Value;

            //Todo : Run secondary bar logic

            //Total the damage taken whilst the bar is active
            currentDamageTaken = Mathf.RoundToInt(currentDamageTaken + (OldHealthValue - newValue));

            if (currentDamageTaken < 0)
            {
                currentDamageTaken = Mathf.Abs(currentDamageTaken);
                characterDamage.text = "+ " + currentDamageTaken.ToString();
            }
            else 
            {
                characterDamage.text = "- " + currentDamageTaken.ToString();
            }

            slider.value = newValue;

            if (character.characterNetworkManager.currentHealth.Value != character.characterNetworkManager.maxHealth.Value) 
            {
                hideTimer = defaultTimeBeforeBarHides;
                gameObject.SetActive(true);
            }
        }

    }
}
