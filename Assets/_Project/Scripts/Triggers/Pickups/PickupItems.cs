using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Game
{
    internal class PickupItems : MonoBehaviour, ICheckableTrigger
    {
        [Header("UI Settings")]
        [SerializeField] private GameObject _textInd;
        [SerializeField] private string _spritePath; // Путь относительно папки Resources

        private Pickup _pickup;
        private ToggleObjectOnButtonPress _toggleScript;
        public bool IsDone { get; private set; }

        [Inject]
        private void Construct(ToggleObjectOnButtonPress toggleScript)
        {
            _toggleScript = toggleScript;
        }

        private void Awake()
        {
            _pickup = GetComponent<Pickup>();

            // Если путь не задан в инспекторе, используем путь из Pickup
            if (string.IsNullOrEmpty(_spritePath) && _pickup != null)
            {
                _spritePath = _pickup.Picture;
            }
        }

        private IEnumerator ShowIndication()
        {
            if (_textInd != null)
            {
                if (!_textInd.activeSelf) _textInd.SetActive(true);
                yield return new WaitForSeconds(2f);
                if (_textInd.activeSelf) _textInd.SetActive(false);
            }
        }

        public void Triggered()
        {
            IsDone = true;
            StartCoroutine(ShowIndication());
            //Debug.Log("Подобрал предмет: " + _pickup?.Name);

            if (_pickup != null)
            {
                // Создаем копию объекта с учетом разных случаев
                Pickup pickupCopy = CreatePickupCopy();

                // Добавляем копию в коллекцию
                AssembledPickups.AddPickup(pickupCopy);
                LogAllPickups();

                if (_pickup.RenderedOnScreen)
                {
                    HandleScreenRendering();
                }
            }
        }

        private Pickup CreatePickupCopy()
        {
            // Если это компонент GameObject
            if (_pickup is Component component)
            {
                // Создаем копию всего GameObject
                GameObject copyObject = Instantiate(component.gameObject);

                // Делаем копию неуничтожаемой при загрузке новых сцен
                DontDestroyOnLoad(copyObject);

                // Деактивируем копию
                copyObject.SetActive(false);

                // Возвращаем компонент Pickup с копии
                return copyObject.GetComponent<Pickup>();
            }
            else
            {
                // Если это ScriptableObject или другой не-компонент
                Pickup pickupCopy = Instantiate(_pickup);
                return pickupCopy;
            }
        }

        private void LogAllPickups()
        {
            StringBuilder sb = new StringBuilder("Все предметы в инвентаре: ");
            foreach (Pickup pickup in AssembledPickups.GetAllPickups())
            {
                sb.Append(pickup.Name).Append(", ");
            }
            //Debug.Log(sb.ToString());
        }

        private void HandleScreenRendering()
        {
            if (_toggleScript == null || _toggleScript.TargetObject == null) return;

            // Активируем объект если нужно
            if (!_toggleScript.TargetObject.activeSelf)
            {
                _toggleScript.Activate(true);
            }

            // Загружаем и устанавливаем спрайт
            if (!string.IsNullOrEmpty(_spritePath))
            {
                Sprite loadedSprite = Resources.Load<Sprite>(_spritePath);

                if (loadedSprite != null)
                {
                    Image image = _toggleScript.TargetObject.GetComponent<Image>();
                    if (image != null)
                    {
                        image.sprite = loadedSprite;
                    }
                    else
                    {
                        //Debug.LogError("У целевого объекта отсутствует компонент Image");
                    }
                }
                else
                {
                    //Debug.LogError($"Не удалось загрузить спрайт по пути: Resources/{_spritePath}");
                }
            }
        }

        private void OnDestroy()
        {
            // Очищаем ссылки при уничтожении объекта
            _pickup = null;
            _toggleScript = null;
        }
    }
}