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

        public TriggerEvent(InfluenceType areaType, GameObject playerObj, GameObject triggerObj, bool isEnteracted)
        {
            AreaType = areaType;
            PlayerObj = playerObj;
            IsEnteracted = isEnteracted;
            TriggerObj = triggerObj;
        }
    }

    [RequireComponent(typeof(BoxCollider))]
    internal class InfluenceArea : MonoBehaviour
    {
        public InfluenceType AreaType;
        public InfluenceInteractionType InteractionType;
        public string triggerObjectName;
        //GameObject triggerObject;
        NPCPrefabFactory npcFactory;
        //[SerializeReference]
        //public GameObject Handler;
        public event Action<TriggerEvent> OnEventTriggered;

        private BoxCollider boxCollider;

        [Inject]
        private void Construct(NPCPrefabFactory npcFActory)
        {
            //this.triggerObject = npcFActory.GetInstance(triggerObjectName);
            this.npcFactory = npcFActory;
        }

        private void Reset()
        {
            boxCollider = GetComponent<BoxCollider>();
            boxCollider.isTrigger = true;
            boxCollider.size = new Vector3(5f, 5f, 5f);
            boxCollider.center = Vector3.zero;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (InteractionType == InfluenceInteractionType.Enter)
            {
                InvokeEvent(other);
            }
        }

        private void OnTriggerStay(Collider other)
        {
            if (InteractionType == InfluenceInteractionType.Stay)
            {
                InvokeEvent(other);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (InteractionType == InfluenceInteractionType.Exit)
            {
                InvokeEvent(other);
            }
        }

        private void InvokeEvent(Collider obj)
        {
            bool interacted = InputManager.GetInstance().GetInteractPressed();
            GameObject triggerObject = npcFactory.GetInstance(triggerObjectName);
            TriggerEvent iventData = new TriggerEvent(AreaType, obj.gameObject, triggerObject, interacted);
            OnEventTriggered?.Invoke(iventData);
            //if (Handler != null)
            //{

            //    //Handler.transform.GetComponent<ITriggerHandler>().Handle(iventData);
            //}
        }
    }
}
