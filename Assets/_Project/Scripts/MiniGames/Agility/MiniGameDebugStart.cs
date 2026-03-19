using System.Collections;
using UnityEngine;

namespace Game.MiniGame.PowerCheck
{
    public class MiniGameDebugStart : MonoBehaviour
    {
        public DexMiniGameController controller;
        public bool autoStartWhenStandalone = true;
        public float standaloneDelay = 0.15f;

        private bool _runRequested;
        private Coroutine _autoStartRoutine;

        private void OnEnable()
        {
            if (!Application.isPlaying || !autoStartWhenStandalone)
                return;

            _autoStartRoutine = StartCoroutine(TryStandaloneStart());
        }

        private void OnDisable()
        {
            if (_autoStartRoutine != null)
            {
                StopCoroutine(_autoStartRoutine);
                _autoStartRoutine = null;
            }
        }

        private IEnumerator TryStandaloneStart()
        {
            yield return new WaitForSeconds(standaloneDelay);

            if (ShouldSkipStandaloneStart())
                yield break;

            StartStandaloneRun();
        }

        private void Update()
        {
            if (!Application.isPlaying || ShouldSkipStandaloneControls())
                return;

            if (Input.GetKeyDown(KeyCode.Space))
                StartStandaloneRun();
        }

        private void StartStandaloneRun()
        {
            if (_runRequested)
                return;

            var installer = AgilitySceneUtility.FindInLoadedScene<Game.MiniGame.Agility.AgilityInstaller>();
            if (installer == null)
            {
                Debug.LogError("MiniGameDebugStart: AgilityInstaller не найден.");
                return;
            }

            _runRequested = true;
            installer.InitializeWithData(null);
        }

        private bool ShouldSkipStandaloneStart()
        {
            if (_runRequested)
                return true;

            return Game.MiniGame.MinigameManager.Instance != null &&
                   Game.MiniGame.MinigameManager.Instance.CurrentGameData != null;
        }

        private bool ShouldSkipStandaloneControls()
        {
            return Game.MiniGame.MinigameManager.Instance != null &&
                   Game.MiniGame.MinigameManager.Instance.CurrentGameData != null;
        }
    }
}
