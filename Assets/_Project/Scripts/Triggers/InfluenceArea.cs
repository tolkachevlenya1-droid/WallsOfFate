using Ink.Parsed;
using System;
using UnityEngine;
using Zenject;

namespace Game
{
    public enum InfluenceType
    {
        None = 0,
        Dialog = 1,
        Object = 2
    }

    public enum InfluenceInteractionType
    {
        Stay = 0,
        Enter = 1,
        Exit = 2,
    }

    public struct TriggerEvent
    {
        public InfluenceType AreaType;
        public GameObject PlayerObj;
        public GameObject TriggerObj;
        public bool IsEnteracted;
        public string Parameters;

        public TriggerEvent(InfluenceType areaType, GameObject playerObj, GameObject triggerObj, bool isEnteracted, string parameters)
        {
            AreaType = areaType;
            PlayerObj = playerObj;
            IsEnteracted = isEnteracted;
            TriggerObj = triggerObj;
            Parameters = parameters;
        }
    }

    [RequireComponent(typeof(BoxCollider))]
    internal class InfluenceArea : MonoBehaviour
    {
        public InfluenceType AreaType;
        public InfluenceInteractionType InteractionType;
        public GameObject triggerObject;
        public event Action<TriggerEvent> OnEventTriggered;
        [TextArea]
        public string Parameters;
        [SerializeField] private cakeslice.Outline outline;

        private BoxCollider boxCollider;

        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(5f, 5f, 5f);
            boxCollider.center = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UpdateOutlineState(true);

                if (InteractionType == InfluenceInteractionType.Enter)
                {
                    InvokeEvent(other);
                }
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                if (InteractionType == InfluenceInteractionType.Stay)
                {
                    InvokeEvent(other);
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                UpdateOutlineState(false);

                if (InteractionType == InfluenceInteractionType.Exit)
                {
                    InvokeEvent(other);
                }
            }
        }

        private void UpdateOutlineState(bool playerInZone)
        {
            if (outline == null) return;

            bool canHighlight = true;

            if (triggerObject != null)
            {
                var interactable = triggerObject.GetComponent<InteractableItem>();
                if (interactable != null)
                    canHighlight = !interactable.HasBeenUsed;
            }

            SetOutlineEnabled(canHighlight && playerInZone);
        }

        private void SetOutlineEnabled(bool enabled)
        {
            outline.enabled = enabled;
        }

        private void InvokeEvent(Collider obj)
        {
            bool interacted = InputManager.GetInstance().GetInteractPressed();
            TriggerEvent iventData = new TriggerEvent(AreaType, obj.gameObject, triggerObject, interacted, Parameters);
            OnEventTriggered?.Invoke(iventData);
        }
    }
}
