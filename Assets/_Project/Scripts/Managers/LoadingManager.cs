using Game.Quest;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{
    public class LoadingManager
    {
        public bool StartupIntroShown { get; set; }

        public event Action LoadingStarted;
        public event Action LoadingFinished;
        public event Action<float> LoadingProgressUpdated;
        public event Action WaitingForInputStarted;

        private readonly string loadingSceneName = "LoadingScene";
        public bool IsLoading { get; private set; }
        public void LoadSceneAsync(string sceneName)
        {
            SceneManager.LoadScene(loadingSceneName);            

            Scene loadingScene = SceneManager.GetActiveScene();

            IsLoading = true;
            LoadingStarted?.Invoke();

            var op = SceneManager.LoadSceneAsync(sceneName);
            op.allowSceneActivation = false;

            var controller = loadingScene.GetRootGameObjects()
                .SelectMany(go => go.GetComponentsInChildren<LoadingScreenController>())
                .First();

            controller.ActivateLoading(op);
            controller.WaitingForInputEnded += () =>
            {
                var loadedScene = SceneManager.GetSceneByName(sceneName);
                SceneManager.SetActiveScene(loadedScene);

                AudioManager.Instance.ReloadVolumeSettings();
                AudioManager.Instance.ChangeMusicForScene(sceneName);

                IsLoading = false;
                LoadingFinished?.Invoke();
            };
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.LoadScene(sceneName);

            AudioManager.Instance.ReloadVolumeSettings();
            AudioManager.Instance.ChangeMusicForScene(sceneName);
        }
    }
}
