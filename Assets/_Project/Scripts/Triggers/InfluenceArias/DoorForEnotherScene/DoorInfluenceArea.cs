using UnityEngine;
using System;

namespace Game
{
    public class DoorInfluenceArea : InfluenceArea
    {
        [Header("Door Settings")]
        [SerializeField] private DoorParameters doorParameters;

        public event Action<TriggerEvent, DoorParameters> OnDoorInteracted;
      
        public override void InvokeEvent(Collider obj)
        {
            bool interacted = InputManager.GetInstance().GetInteractPressed();

            string json = JsonUtility.ToJson(doorParameters);

            TriggerEvent eventData = new TriggerEvent(
                AreaType,
                obj.gameObject,
                triggerObject ?? gameObject,
                interacted, 
                json
            );

            OnDoorInteracted?.Invoke(eventData, doorParameters);
        }
    }
}