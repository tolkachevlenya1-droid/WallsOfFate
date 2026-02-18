using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public class InteractManager : MonoBehaviour
    {
        // Список всех встреченных интерактивных объектов (реализующих ITriggerable)
        private HashSet<ITriggerable> encounteredTriggers = new HashSet<ITriggerable>();
        // Набор для отслеживания уже активированных триггеров
        private HashSet<ITriggerable> triggeredSet = new HashSet<ITriggerable>();
        // Текущий активный интерактивный объект
        private ITriggerable currentTriggerable;
        // Объект-индикатор взаимодействия (например, подсветка)
        private GameObject interactionIndicator;
        // Флаг, чтобы избежать повторного взаимодействия до выхода из зоны
        private bool hasInteracted = false;
        // Коллайдер текущего интерактивного объекта
        private Collider currentTriggerCollider;

        // Ссылка на компонент анимаций игрока, который содержит методы PlayPickupFloor, PlayPickupBody и PlayOpenChest
        private PlayerAnimator playerAnimator;

        [Tooltip("Минимальная пауза между повторными нажатиями, сек.")]
        [SerializeField] private float interactCooldown = 0.4f;
        private float _nextTimeCanInteract = 0f;
        private bool _interactBuffered;
        private void Awake()
        {

            playerAnimator = GetComponent<PlayerAnimator>();
            if (playerAnimator == null)
            {
                //Debug.LogError("InteractManager: Не найден компонент PlayerAnimator!");
            }

        }

        private void OnTriggerEnter(Collider collider)
        {
            // Получаем ВСЕ компоненты ITriggerable на объекте
            ITriggerable[] triggerables = collider.gameObject.GetComponents<ITriggerable>();

            if (triggerables.Length == 0) return;

            bool isNewCollider = currentTriggerCollider != collider;

            foreach (var triggerable in triggerables)
            {
                // Добавляем все триггеры в список
                if (!encounteredTriggers.Contains(triggerable))
                {
                    encounteredTriggers.Add(triggerable);
                }

                // Обновляем текущий активный триггер только если:
                // - это новый коллайдер
                // - или еще не было взаимодействия
                if (isNewCollider || !hasInteracted)
                {
                    currentTriggerable = triggerable;
                    currentTriggerCollider = collider;
                    hasInteracted = false;
                }
            }

            // Поиск индикатора взаимодействия (один на весь объект)
            var indicators = collider.gameObject.GetComponentsInChildren<Transform>(true);
            foreach (var indicator in indicators)
            {
                if (indicator.CompareTag("InteractionIndicator"))
                {
                    interactionIndicator = indicator.gameObject;
                    interactionIndicator.SetActive(true);
                    break;
                }
            }
        }

        private void OnTriggerStay(Collider collider)
        {
            // Обновляем currentTriggerable каждый FixedUpdate,
            // если по какой-то причине он вдруг стал null
            if (currentTriggerable == null)
            {
                OnTriggerEnter(collider);   // переиспользуем уже готовую логику
                _nextTimeCanInteract = Time.time;   // не блокируем первое нажатие
            }

            // --- необязательно, но удобно: маленький буфер нажатия ---
            if (!_interactBuffered && InputManager.GetInstance().GetInteractPressed())
                _interactBuffered = true;
        }

        private void OnTriggerExit(Collider collider)
        {
            // При выходе из зоны интерактивного объекта сбрасываем данные
            ClearData(collider);
        }

        private void ClearData(Collider collider = null, bool onDestroy = false)
        {
            if (interactionIndicator != null)
            {
                interactionIndicator.SetActive(false);
            }

            encounteredTriggers.Clear();
            triggeredSet.Clear();
            currentTriggerable = null;
            interactionIndicator = null;
            hasInteracted = false;
            if (collider == currentTriggerCollider || onDestroy)
            {
                currentTriggerCollider = null;
            }
        }


        private void Update()
        {
            if (currentTriggerable == null) return;
            if (Time.time < _nextTimeCanInteract) return;

            // либо нажали в этом кадре, либо нажатие «забуферилось» в OnTriggerStay
            if (_interactBuffered || InputManager.GetInstance().GetInteractPressed())
            {
                _interactBuffered = false;          // сброс буфера
                InteractWith(currentTriggerable);
                _nextTimeCanInteract = Time.time + interactCooldown;
            }
        }

        // Пытаемся активировать конкретный триггер, если он еще не был активирован (либо если это Box)
        private void TryTrigger(ITriggerable trigger)
        {
            // 1) Можно ли триггер вызывать многократно?
            //    В пример добавлены «Dialogue» и «Box»
            bool repeatable =
                trigger is Box;
            //|| (trigger as MonoBehaviour)?.CompareTag("Dialogue") == true;

            // 2) Проверяем активирован ли раньше
            if (!triggeredSet.Contains(trigger) || repeatable)
            {
                if (trigger is InteractableItem item)
                    item.Interact();
                else
                    trigger.Triggered();

                // В «чёрный список» заносим только одноразовые
                if (!repeatable)
                    triggeredSet.Add(trigger);
            }
        }

        public void InteractWith(ITriggerable trigger)
        {
            if (trigger == null) return;

            // получаем MonoBehaviour, на котором висят все ITriggerable-компоненты
            var mb = trigger as MonoBehaviour;
            if (!mb) return;                       // на всякий случай

            foreach (var t in mb.GetComponents<ITriggerable>())
                TryTrigger(t);                     // включает Interact() или Triggered()

            // анимация игрока по тегу — как было
            GameObject go = mb.gameObject;
            if (go.CompareTag("PickupFloor")) playerAnimator.PlayPickupFloor();
            else if (go.CompareTag("PickupBody")) playerAnimator.PlayPickupBody();
            else if (go.CompareTag("Chest")) playerAnimator.PlayOpenChest();
            else if (go.CompareTag("Box"))
            {
                var grabber = GetComponent<PlayerBoxGrabber>();
                if (grabber != null)
                    grabber.ToggleGrab(go.transform);   // единая логика
            }

            hasInteracted = true;
            if (interactionIndicator) interactionIndicator.SetActive(false);
        }




        // Метод для однократного запуска всех встреченных триггеров
        public void TriggerAllEncounteredOnce()
        {
            foreach (var trigger in encounteredTriggers)
            {
                if (trigger != null)
                {
                    TryTrigger(trigger);
                }
            }
        }

        // Метод для получения списка всех встреченных интерактивных объектов
        public List<ITriggerable> GetEncounteredTriggers()
        {
            return new List<ITriggerable>(encounteredTriggers);
        }

        // Метод для проверки, был ли триггер уже активирован
        public bool HasTriggerBeenActivated(ITriggerable trigger)
        {
            return triggeredSet.Contains(trigger);
        }

        private void OnDisable()
        {
            ClearData(null, true);
        }
    }
}

