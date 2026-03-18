using Game;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Scripts.Triggers
{
    [RequireComponent(typeof(BoxCollider))]
    internal class StartDayDialogueTriggerZone : MonoBehaviour
    {
        public InfluenceType AreaType;
        public InfluenceInteractionType InteractionType;
        public GameObject triggerObject;
        public event Action<TriggerEvent> OnEventTriggered;
        [TextArea]
        public string Parameters;

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
            TriggerEvent iventData = new TriggerEvent(AreaType, null, obj.gameObject, interacted, Parameters);
            OnEventTriggered?.Invoke(iventData);
        }
    }
}
