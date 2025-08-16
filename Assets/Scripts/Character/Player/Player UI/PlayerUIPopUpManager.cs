using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Threading;
using UnityEngine.UI;

namespace SweetClown
{
    public class PlayerUIPopUpManager : MonoBehaviour
    {
        [Header("Message Pop Up")]
        [SerializeField] GameObject popUpMessageGameObject;
        [SerializeField] TextMeshProUGUI popUpMessageText;

        [Header("Item Pop Up")]
        [SerializeField] GameObject itemPopUpGameObject;
        [SerializeField] Image itemIcon;
        [SerializeField] TextMeshProUGUI itemName;
        [SerializeField] TextMeshProUGUI itemAmount;


        [Header("You Died Pop Up")]
        [SerializeField] GameObject youDiedPopUpGameObject;
        [SerializeField] TextMeshProUGUI youDiedPopUpBackGroundText;
        [SerializeField] TextMeshProUGUI youDiedPopUpBackText;
        [SerializeField] CanvasGroup youDiedPopUpCanvasGroup; //Allow us to set the alpoha to fade over time

        [Header("Boss Defeated Pop Up")]
        [SerializeField] GameObject bossDefeatedPopUpGameObject;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpBackGroundText;
        [SerializeField] TextMeshProUGUI bossDefeatedPopUpText;
        [SerializeField] CanvasGroup bossDefeatedPopUpCanvasGroup; //Allow us to set the alpoha to fade over time

        [Header("Baceon Restored Pop Up")]
        [SerializeField] GameObject baceonPopUpGameObject;
        [SerializeField] TextMeshProUGUI baceonPopUpBackGroundText;
        [SerializeField] TextMeshProUGUI baceonPopUpText;
        [SerializeField] CanvasGroup baceonPopUpCanvasGroup;

        public void CloseAllPopUpWindows() 
        {
            popUpMessageGameObject.SetActive(false);
            itemPopUpGameObject.SetActive(false);

            PlayerUIManager.instance.popUpWindowIsOpen = false;
        }

        public void SendPlayerMessagePopUp(string messageText) 
        {
            PlayerUIManager.instance.popUpWindowIsOpen = true;
            popUpMessageText.text = messageText;
            popUpMessageGameObject.SetActive(true);
        }

        public void SendItemPopUp(Item item, int amount)
        {
            itemAmount.enabled = false;
            itemIcon.sprite = item.itemIcon;
            itemName.text = item.itemName;

            if (amount > 0) 
            {
                itemAmount.enabled = true;
                itemAmount.text = "x" + amount.ToString();
            }

            itemPopUpGameObject.SetActive(true);
            PlayerUIManager.instance.popUpWindowIsOpen = true;
        }

        public void SendYouDiedPopUp()
        {
            //Active post processing effects
            youDiedPopUpGameObject.SetActive(true);
            youDiedPopUpBackGroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(youDiedPopUpBackGroundText, 8, 19));
            StartCoroutine(FadeInPopUpOverTime(youDiedPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(youDiedPopUpCanvasGroup,  2,  5));
            // Fade in the pop up
        }

        public void SendBossDefeatedPopUp(string bossDefeatedMessage)
        {
            bossDefeatedPopUpText.text = bossDefeatedMessage;
            bossDefeatedPopUpBackGroundText.text = bossDefeatedMessage;
            bossDefeatedPopUpGameObject.SetActive(true);
            bossDefeatedPopUpBackGroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(bossDefeatedPopUpBackGroundText, 8, 19));
            StartCoroutine(FadeInPopUpOverTime(bossDefeatedPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(bossDefeatedPopUpCanvasGroup, 2, 5));
        }

        public void SendBaceonRestorePopUp(string BaceonRestoreMessage)
        {
            baceonPopUpText.text = BaceonRestoreMessage;
            baceonPopUpBackGroundText.text = BaceonRestoreMessage;
            baceonPopUpGameObject.SetActive(true);
            baceonPopUpBackGroundText.characterSpacing = 0;
            StartCoroutine(StretchPopUpTextOverTime(baceonPopUpBackGroundText, 8, 19));
            StartCoroutine(FadeInPopUpOverTime(baceonPopUpCanvasGroup, 5));
            StartCoroutine(WaitThenFadeOutPopUpOverTime(baceonPopUpCanvasGroup, 2, 5));
        }

        private IEnumerator StretchPopUpTextOverTime(TextMeshProUGUI text, float duration, float stretchAmount) 
        {
            if (duration > 0f) 
            {
                text.characterSpacing = 0;
                float timer = 0;
                yield return null;

                while (timer < duration) 
                {
                    timer = timer + Time.deltaTime;
                    text.characterSpacing = Mathf.Lerp(text.characterSpacing, stretchAmount, duration * (Time.deltaTime / 20));
                    yield return null;
                }
            }
        }

        private IEnumerator FadeInPopUpOverTime(CanvasGroup canvas, float duration) 
        {
            if (duration > 0) 
            {
                canvas.alpha = 0;
                float timer = 0;
                yield return null;

                while (timer < duration) 
                {
                    timer = timer + Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 1, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 1;

            yield return null;
        }

        private IEnumerator WaitThenFadeOutPopUpOverTime(CanvasGroup canvas, float duration, float delay) 
        {
            if (duration > 0)
            {
                while (delay > 0) 
                {
                    delay = delay - Time.deltaTime;
                    yield return null;
                }

                canvas.alpha = 1;
                float timer = 0;
                yield return null;

                while (timer < duration)
                {
                    timer = timer + Time.deltaTime;
                    canvas.alpha = Mathf.Lerp(canvas.alpha, 0, duration * Time.deltaTime);
                    yield return null;
                }
            }

            canvas.alpha = 0;

            yield return null;
        }
    }
}
