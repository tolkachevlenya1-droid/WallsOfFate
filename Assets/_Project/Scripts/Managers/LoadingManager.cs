using Game.Quest;
using System;
using System.Linq;
using UnityEngine;
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

        public bool IsLoading { get; private set; }
        public void LoadScene(string sceneName)
        {
            /*targetSceneName = sceneName;
            ShowLoadingUI();

            IsLoading = true;
            LoadingStarted?.Invoke();

            AudioManager.Instance.ActivateLoadingSnapshot();
            AudioManager.Instance.PlayLoadingMusic();

            StartCoroutine(LoadSceneAsync(sceneName, showStartDay: false));*/
        }
    }
}
