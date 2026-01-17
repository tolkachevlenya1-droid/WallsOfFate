using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game
{

    public class ExitConfirmation : MonoBehaviour
    {
        [SerializeField] private GameObject exitPanel;
        [SerializeField] private Button confirmButton;  // кнопка подтверждения выхода


        private void Start()
        {
            exitPanel.SetActive(false);
        }

        private void Update()
        {
            // закрываем панель по Esc, если она уже открыта
            if (exitPanel.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                HideExitPanel();
            }
        }

        public void ShowExitPanel()
        {
            exitPanel.SetActive(true);

            // Убираем текущее выделение, затем выбираем кнопку «Подтвердить»
            EventSystem.current.SetSelectedGameObject(null);
            confirmButton.Select();
        }

        public void HideExitPanel()
        {
            exitPanel.SetActive(false);

            // сброс фокуса, чтобы при закрытии панели ничего не осталось выделенным
            EventSystem.current.SetSelectedGameObject(null);
        }

        public void QuitToMenuGame()
        {
            LoadingScreenManager.Instance.LoadScene("MainMenu");
        }

        public void QuitGame()
        {
            Application.Quit();
        }
    }

}