using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    public class StartDayScreenController : MonoBehaviour
    {
        private Coroutine _fadeCoroutine;

        public float startDayDuration = 2f;   // сколько сек показывать начало дня
        public float inputDelay = 0.05f;      // пауза перед тем, как выводим кнопку Continue

        public Sprite finalSprite;            // то, чем заменить анимацию перед Continue

        private string targetSceneName;

        private LoadingManager loadingManager;
        [Inject]
        private void Construct(LoadingManager loadingManager)
        {
            this.loadingManager = loadingManager;
        }

        /*public void ShowEndOfDayPanel()
        {
            Time.timeScale = 0f;               // ставим игру на паузу
            panelEndOfDay.SetActive(true);
        }*/

        /*public void OnConfirmEndOfDay()
        {
            PlayerSpawnData.ClearData();

            Quest.QuestCollection.IncreaseCurrentDay();   // день +1

            panelEndOfDay.SetActive(false);
            BeginLoadWithStartOfDay("StartDay");
        }*/

        /*public void OnCancelEndOfDay()
        {
            panelEndOfDay.SetActive(false);
        }*/

        /*public void BeginLoadWithStartOfDay(string sceneName)
        {
            targetSceneName = sceneName;
            ShowLoadingUI();

            IsLoading = true;
            //LoadingStarted?.Invoke();

            AudioManager.Instance.ActivateLoadingSnapshot();
            AudioManager.Instance.PlayLoadingMusic();

            StartCoroutine(LoadSceneAsync(sceneName, showStartDay: true));
        }*/

        /*private void ShowLoadingUI()
        {
            Time.timeScale = 1f;
            loadingScreen.SetActive(true);
            panelEndOfDay.SetActive(false);
        }*/

        /*private void StartTextFade()
        {
            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);
        }*/

        private IEnumerator WaitForUserInput(AsyncOperation op)
        {
            while (!Input.anyKeyDown) yield return null;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            /*if (showStartDay && panelStartOfDay != null)
            {
                panelStartOfDay.SetActive(true);
                yield return new WaitForSeconds(startDayDuration);
                op.allowSceneActivation = true;
                yield return new WaitUntil(() => op.isDone);
                panelStartOfDay.SetActive(false);
            }
            else
            {
                op.allowSceneActivation = true;
                yield return new WaitUntil(() => op.isDone);
            }*/

            // loadingScreen.SetActive(false);

            // IsLoading = false;

            // LoadingFinished?.Invoke();
        }
    }

}
