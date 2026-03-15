using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Game.UI
{
    public class ExitConfirmationPanelController : MonoBehaviour
    {        
        public Button confirmButton;
        public Button declineButton;

        private void Start()
        {
            // назначаем обработчики кнопок
            confirmButton.onClick.AddListener(QuitGame);
            declineButton.onClick.AddListener(HideExitPanel);
        }

        private void Update()
        {
            // закрываем панель по Esc, если она уже открыта
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                HideExitPanel();
            }
        }

        private void OnEnable()
        {
            // При открытии панели выделяем кнопку «Отмена» по умолчанию
            EventSystem.current.SetSelectedGameObject(null);
            declineButton.Select();
        }

        public void HideExitPanel()
        {
            gameObject.SetActive(false);

            // сброс фокуса, чтобы при закрытии панели ничего не осталось выделенным
            EventSystem.current.SetSelectedGameObject(null);
        }
        public void QuitGame()
        {
            Application.Quit();
        }
    }

}