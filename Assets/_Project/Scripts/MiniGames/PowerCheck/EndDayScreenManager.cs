using UnityEngine;
using System.Collections;
using System;

namespace Game
{
    public class EndDayScreenManager : MonoBehaviour
    {
        [SerializeField] private GameObject WinScreen;
        [SerializeField] private GameObject LooseScreen;
        [SerializeField] private GameObject GameProcess;
        [SerializeField] private float displayTime = 3f; // Глобальная переменная времени

        public Action<bool> OnEndGame;
        bool playerWonGlobal;

        void Start()
        {

            GameProcess.GetComponent<GameProcess>().OnEndGame += DisplayEndScreen;
            WinScreen.SetActive(false);
            LooseScreen.SetActive(false);
        }

        private void DisplayEndScreen(bool playerWon)
        {
            Time.timeScale = 0f;
            GameObject endScreen = playerWon ? WinScreen : LooseScreen;
            playerWonGlobal = playerWon;

            endScreen.SetActive(true);

            StartCoroutine(HideScreenAfterDelay(endScreen, displayTime));
        }

        private IEnumerator HideScreenAfterDelay(GameObject screen, float delay)
        {
            yield return new WaitForSecondsRealtime(delay);

            screen.SetActive(false);
            Time.timeScale = 1f;
            OnEndGame?.Invoke(playerWonGlobal);
        }

        void OnDestroy()
        {
            if (GameProcess != null)
                GameProcess.GetComponent<GameProcess>().OnEndGame -= DisplayEndScreen;
        }
    }
}
