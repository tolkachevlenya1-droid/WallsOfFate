using Assets._Project.Scripts.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    public class ToggleObjectOnButtonPress : MonoBehaviour
    {
        [System.Serializable]
        public class ToggleButton
        {
            public KeyCode keyCode;  // Клавиша, на которую реагируем
            public bool setActive;   // В какое состояние переводим TargetObject
        }

        [SerializeField]
        private List<ToggleButton> toggleButtons = new List<ToggleButton>();

        [field: SerializeField]
        public GameObject TargetObject { get; private set; }

        private GameObject _objectToDisable;
        private bool _isPaused = false;

        private LoadingManager _loadingManager;

        [Inject]
        private void Construct(PlayerMoveController playerMoveController, LoadingManager loadingManager)
        {
            _objectToDisable = playerMoveController.gameObject;
            _loadingManager = loadingManager;
        }

        void Update()
        {
            // ★ Если загрузочный экран активен — выходим и не обрабатываем Esc/другие кнопки
            if (_loadingManager.IsLoading)
                return;

            foreach (var button in toggleButtons)
            {
                if (Input.GetKeyDown(button.keyCode))
                {
                    Activate(button.setActive);
                }
            }
        }

        public void Activate(bool station)
        {
            // Здесь можно ещё добавить защиту от двойного срабатывания:
            // if (LoadingScreenManager.Instance != null && LoadingScreenManager.Instance.IsLoading) return;

            if (station)
                TargetObject.SetActive(!TargetObject.activeSelf);
            else
                TargetObject.SetActive(false);
        }
    }
}
