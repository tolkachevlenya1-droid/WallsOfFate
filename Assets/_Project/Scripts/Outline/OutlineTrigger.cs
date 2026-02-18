using UnityEngine;
using cakeslice;

namespace Game
{
    [RequireComponent(typeof(Collider))]
    public class OutlineTrigger : MonoBehaviour
    {
        [Header("Hover Settings")]
        [Tooltip("Максимальная дистанция, на которой мы проверяем наведение курсора")]
        [SerializeField] private float hoverCheckDistance = 100f;
        [Tooltip("Слой(и) для проверки наведения")]
        [SerializeField] private LayerMask hoverLayerMask = ~0;

        private Outline[] outlines;
        private Collider[] colliders;
        private InteractableItem interactable;

        private bool isPlayerInTrigger;
        private bool isMouseOver;

        private void Start()
        {
            // Получаем все Outline-ы и Collider-ы на объекте и его дочерних объектах
            outlines = GetComponentsInChildren<Outline>(true);
            colliders = GetComponentsInChildren<Collider>(true);
            interactable = GetComponent<InteractableItem>();

            // Отключаем подсветку по умолчанию
            foreach (var o in outlines)
                o.enabled = false;
        }

        private void Update()
        {
            // Кастомная проверка наведения на любой из коллайдеров
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            bool hitThis = false;

            foreach (var col in colliders)
            {
                // Фильтруем по слою
                if (((1 << col.gameObject.layer) & hoverLayerMask) == 0) continue;

                // Проверяем попадание луча в конкретный коллайдер
                if (col.Raycast(ray, out _, hoverCheckDistance))
                {
                    hitThis = true;
                    break;
                }
            }

            if (hitThis != isMouseOver)
            {
                isMouseOver = hitThis;
                UpdateOutlineState();
            }
        }

        private void UpdateOutlineState()
        {
            bool canHighlight = interactable == null || !interactable.HasBeenUsed;
            bool shouldBeOn = canHighlight && (isPlayerInTrigger || isMouseOver);

            foreach (var o in outlines)
                o.enabled = shouldBeOn;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = true;
                UpdateOutlineState();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = false;
                UpdateOutlineState();
            }
        }
    }

}

