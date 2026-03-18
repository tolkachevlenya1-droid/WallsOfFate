using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    [RequireComponent(typeof(InteractibleItemInfluenceArea))]
    public class InteractibleHighlightController : MonoBehaviour
    {
        [Header("Outline Settings")]
        [SerializeField] private URPOutline[] outlines;
        [SerializeField] private bool highlightOnTrigger = true;

        private InteractibleItemInfluenceArea itemArea;
        private bool isPlayerInTrigger;

        private void Awake()
        {
            itemArea = GetComponent<InteractibleItemInfluenceArea>();

            // Автоматически находим все URPOutline компоненты
            if (outlines == null || outlines.Length == 0)
            {
                if (itemArea.triggerObject != null)
                {
                    outlines = itemArea.triggerObject.GetComponentsInChildren<URPOutline>(true);
                }

                if (outlines.Length == 0)
                {
                    outlines = GetComponentsInChildren<URPOutline>(true);
                }
            }

            // Если всё еще нет - добавляем на все рендереры
            if (outlines.Length == 0)
            {
                AddOutlinesToRenderers();
            }
        }

        private void AddOutlinesToRenderers()
        {
            GameObject target = itemArea.triggerObject ?? gameObject;
            var renderers = target.GetComponentsInChildren<Renderer>();

            List<URPOutline> outlineList = new List<URPOutline>();
            foreach (var renderer in renderers)
            {
                var outline = renderer.gameObject.AddComponent<URPOutline>();
                outlineList.Add(outline);
            }

            outlines = outlineList.ToArray();
        }

        private void OnEnable()
        {
            itemArea.OnItemInteracted += OnItemInteracted;
        }

        private void OnDisable()
        {
            itemArea.OnItemInteracted -= OnItemInteracted;
        }

        private void OnItemInteracted(TriggerEvent eventData, InteractableItemParameters parameters)
        {
            SetHighlight(false);
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = true;
                if (highlightOnTrigger)
                    UpdateHighlight();
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                isPlayerInTrigger = false;
                if (highlightOnTrigger)
                    UpdateHighlight();
            }
        }

        private void UpdateHighlight()
        {
            if (itemArea.HasBeenUsed) return;
            SetHighlight(isPlayerInTrigger);
        }

        private void SetHighlight(bool enabled)
        {
            if (outlines == null) return;

            foreach (var outline in outlines)
            {
                if (outline != null)
                {
                    outline.SetHighlighted(enabled);
                }
            }
        }

        // Для теста
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SetHighlight(true);
            }
            if (Input.GetKeyUp(KeyCode.Space))
            {
                SetHighlight(false);
            }
        }
    }
}