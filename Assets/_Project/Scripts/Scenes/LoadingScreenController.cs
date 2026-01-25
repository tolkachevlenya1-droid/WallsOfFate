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
    public class LoadingScreenController : MonoBehaviour
    {
        public bool IsLoading { get; private set; }
        public event Action<float> LoadingProgressUpdated;
        public event Action WaitingForInputStarted;
        public event Action WaitingForInputEnded;

        private Coroutine _fadeCoroutine;

        [Header("UI-панели")]
        public GameObject loadingScreen;      // ваша существующая панель загрузки

        public GameObject panelEndOfDay;      // новая: экран «подтвердить конец дня»
        public GameObject panelStartOfDay;    // новая: экран «начало дня»
        public float startDayDuration = 2f;   // сколько сек показывать начало дня
        public float inputDelay = 0.05f;      // пауза перед тем, как выводим кнопку Continue

        public Sprite finalSprite;            // то, чем заменить анимацию перед Continue

        private string targetSceneName;
        private bool waitingForInput;

        [Header("Intro Screen (New Game)")]
        public GameObject panelNewGameIntro;      // ваш новый экран интро при старте        

        private LoadingManager loadingManager;
        [Inject]
        private void Init(LoadingManager loadingManager)
        {
            this.loadingManager = loadingManager;
        }

        private void Start()
        {
            if (!loadingManager.StartupIntroShown)
            {
                loadingManager.StartupIntroShown = true;
                StartCoroutine(ShowStartupIntro());
            }
        }

        public void ActivateLoading(AsyncOperation operation)
        {
            AudioManager.Instance.ActivateLoadingSnapshot();
            AudioManager.Instance.PlayLoadingMusic();

            StartCoroutine(LoadingWait(operation));
        }

        private IEnumerator LoadingWait(AsyncOperation operation)
        {
            // Отслеживаем прогресс загрузки
            while (!operation.isDone)
            {
                LoadingProgressUpdated?.Invoke(operation.progress);

                if (operation.progress >= 0.9f)
                {
                    yield return new WaitForSeconds(inputDelay);

                    // Уведомляем о начале ожидания ввода
                    WaitingForInputStarted?.Invoke();
                    waitingForInput = true;

                    yield return StartCoroutine(WaitForUserInput(operation));
                    yield break;
                }
                yield return null;
            }
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
            waitingForInput = false;

            if (_fadeCoroutine != null)
                StopCoroutine(_fadeCoroutine);

            waitingForInput = false;
            
            AudioManager.Instance.ReloadVolumeSettings();
            AudioManager.Instance.ChangeMusicForScene(targetSceneName);

            WaitingForInputEnded?.Invoke();

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

        private IEnumerator ShowStartupIntro()
        {
            Time.timeScale = 0f;
            panelNewGameIntro.SetActive(true);
            IsLoading = true;

            float timer = 0f;
            while (true)
            {
                if (Input.anyKeyDown)
                    break;
                timer += Time.unscaledDeltaTime;
                yield return null;
            }

            IsLoading = false;
            panelNewGameIntro.SetActive(false);
            Time.timeScale = 1f;
        }
    }

}
