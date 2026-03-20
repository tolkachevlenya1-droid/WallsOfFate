using Game.Core;
using System;
using UnityEngine;
using System.Threading.Tasks;

namespace Game
{
    public class DoorInfluenceArea : InfluenceArea
    {
        [Header("Door Settings")]
        [SerializeField] private DoorParameters doorParameters;

        public AsyncEvent<(TriggerEvent, DoorParameters)> OnDoorInteracted { get; } = new();

        public override async Task InvokeEventAsync(Collider obj)
        {
            bool interacted = ConsumeInteractPress();

            string json = JsonUtility.ToJson(doorParameters);

            TriggerEvent eventData = new TriggerEvent(
                AreaType,
                obj.gameObject,
                triggerObject ?? gameObject,
                interacted,
                json
            );

            await base.InvokeEventAsync(obj);

            await OnDoorInteracted.InvokeAsync((eventData, doorParameters));
        }
    }
}
