using Game.Core;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using Zenject;

namespace Game
{
    public class InteractibleItemInfluenceArea : InfluenceArea
    {
        [Header("Item Parameters")]
        [SerializeField] private InteractableItemParameters itemParameters;

        [Header("Item State")]
        [SerializeField] private bool hasBeenUsed = false;

        public AsyncEvent<(TriggerEvent, InteractableItemParameters)> OnItemInteracted { get; } = new();
        public bool HasBeenUsed => hasBeenUsed;

        private void Start()
        {
            LoadItemState();
            if (triggerObject != null) triggerObject = this.gameObject;

        }

        public void MarkAsUsed()
        {
            if (hasBeenUsed) return;

            hasBeenUsed = true;
            SaveItemState();
        }

        public void ResetForRespawn()
        {
            hasBeenUsed = false;

            if (TryGetComponent<Collider>(out var col))
                col.enabled = true;

            foreach (var o in GetComponentsInChildren<cakeslice.Outline>())
                o.enabled = true;

            string scene = SceneManager.GetActiveScene().name;
            InteractableItemCollection.SetItemState(scene, gameObject.name, false);
        }

        private void LoadItemState()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            InteractableItemCollection.TryGetItemState(sceneName, gameObject.name, out hasBeenUsed);
        }

        private void SaveItemState()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            InteractableItemCollection.SetItemState(sceneName, gameObject.name, hasBeenUsed);
        }

        public override async Task InvokeEventAsync(Collider obj)
        {
            if (!hasBeenUsed)
            {

                bool interacted = InputManager.GetInstance().GetInteractPressed();
                TriggerEvent eventData = new TriggerEvent(
                    AreaType,
                    obj.gameObject,
                    triggerObject,
                    interacted,
                    string.Empty 
                );
                if (interacted)
                {
                    //Debug.Log("interact pressed" + interacted);
                    //Debug.Log("eventData" + eventData.IsEnteracted);
                    MarkAsUsed();
                }

                await OnItemInteracted.InvokeAsync((eventData, itemParameters));
            }
        }
    }
}