using UnityEngine;

namespace Game
{
    public class MiniGameDebugStart : MonoBehaviour, IMiniGameInstaller
    {
        public DexMiniGameController controller;

        public void InitializeWithData(MiniGameData gameData)
        {
            if (controller != null)
                controller.OnEndGame += OnMiniGameEnded;
            controller.StartMiniGame();
        }

        public void OnMiniGameEnded(bool playerWin)
        {
            if (MinigameManager.Instance != null) MinigameManager.Instance.EndMinigame(playerWin);
        }

        void OnDestroy()
        {
            if (controller != null)
            {
                controller.OnEndGame -= OnMiniGameEnded;
            }
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space) && controller != null)
            {
                controller.StartMiniGame();
            }
        }
    }
}

