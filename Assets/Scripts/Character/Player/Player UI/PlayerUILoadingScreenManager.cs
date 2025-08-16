using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace SweetClown
{
    public class PlayerUILoadingScreenManager : MonoBehaviour
    {
        [SerializeField] GameObject loadingScreen;
        [SerializeField] CanvasGroup canvasGroup;
        private Coroutine fadeLoadingScreenCoroutine;

        private void Start()
        {
            SceneManager.activeSceneChanged += OnSceneChanged;
        }

        private void OnSceneChanged(Scene arg0, Scene arg1)
        {
            DeactiveateLoadingScreen();
        }

        public void ActiveateLoadingScreen() 
        {
            //If the loading Screen Object is already active, return
            if (loadingScreen.activeSelf)
                return;

            canvasGroup.alpha = 1.0f;
            loadingScreen.SetActive(true);
        }

        public void DeactiveateLoadingScreen(float delay = 1) 
        {
            if (!loadingScreen.activeSelf)
                return;

            //If we are already fading away the loading screen return
            if (fadeLoadingScreenCoroutine != null)
                return;

            //The duration is how long the fade will take, the delay is the wait in seconds before the fade begins
            fadeLoadingScreenCoroutine = StartCoroutine(FadeLoadingScreen(1, delay));

        }

        private IEnumerator FadeLoadingScreen(float duration, float delay) 
        {
            while (WorldAIManager.instance.isPerformingLoadingOperation) 
            {
                yield return null;
            }

            loadingScreen.SetActive(true);

            if (duration > 0) 
            {
                while (delay > 0) 
                {
                    delay -= Time.deltaTime;
                    yield return null;
                }

                canvasGroup.alpha = 1;
                float elapsedTime = 0;

                while (elapsedTime < duration) 
                {
                    elapsedTime += Time.deltaTime;
                    canvasGroup.alpha = Mathf.Lerp(1, 0, elapsedTime / duration);
                    yield return null;
                }
            }

            canvasGroup.alpha = 0;
            loadingScreen.SetActive(false);
            fadeLoadingScreenCoroutine = null;
            yield return null;
        }
    }
}
