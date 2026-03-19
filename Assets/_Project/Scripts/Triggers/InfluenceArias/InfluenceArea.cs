using Ink.Parsed;
using System;
using UnityEngine;
using UnityEngine.Events;
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
    public class InfluenceArea : MonoBehaviour
    {
        public InfluenceType AreaType;
        public InfluenceInteractionType InteractionType;
        public GameObject triggerObject;
        public UnityEvent<TriggerEvent> OnEventTriggered;
        [TextArea(3, 40)]
        public string Parameters;
        public ITriggerHandler Handler;
        //[SerializeField] private OutlineTrigger outlineTrigger;

        private BoxCollider boxCollider;

        //private void Reset()
        //{
        //    boxCollider = GetComponent<BoxCollider>();
        //    boxCollider.isTrigger = true;
        //    boxCollider.size = new Vector3(5f, 5f, 5f);
        //    boxCollider.center = Vector3.zero;
        //}

        private void Start()
        {
            Handler = FindObjectOfType<BoxGrabberHandler>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {

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
                if (InteractionType == InfluenceInteractionType.Exit)
                {
                    InvokeEvent(other);
                }
            }
        }

        public virtual void InvokeEvent(Collider obj)
        {
            bool interacted = InputManager.GetInstance().GetInteractPressed();
            TriggerEvent eventData = new TriggerEvent(AreaType, obj.gameObject, triggerObject ?? this.gameObject, interacted, Parameters);
            //OnEventTriggered?.Invoke(eventData);
            if (interacted)
            {
                Debug.Log("player interacted");
            }
            Handler.Handle(eventData);
        }
    }
}
