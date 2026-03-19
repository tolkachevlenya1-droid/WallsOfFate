using System;
using UnityEngine;
using Zenject;

namespace Game
{
    public class InteractibleItemInfluenceArea : InfluenceArea
    {
        [Header("Item Parameters")]
        [SerializeField] private InteractableItemParameters itemParameters;

        [Header("Item State")]
        [SerializeField] private bool _hasBeenUsed = false;

        public event Action<TriggerEvent, InteractableItemParameters> OnItemInteracted;

        public bool HasBeenUsed => _hasBeenUsed;

        public InteractableItemParameters Parameters => itemParameters;

        private void Start()
        {
            LoadItemState();
        }

        public void MarkAsUsed()
        {
            if (_hasBeenUsed) return;

            _hasBeenUsed = true;
            SaveItemState();
        }

        private void LoadItemState()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            InteractableItemCollection.TryGetItemState(sceneName, gameObject.name, out _hasBeenUsed);
        }

        private void SaveItemState()
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            InteractableItemCollection.SetItemState(sceneName, gameObject.name, _hasBeenUsed);
        }

        public override void InvokeEvent(Collider obj)
        {
            if (!_hasBeenUsed)
            {

                bool interacted = InputManager.GetInstance().GetInteractPressed();
                TriggerEvent eventData = new TriggerEvent(
                    AreaType,
                    obj.gameObject,
                    triggerObject ?? gameObject,
                    interacted,
                    string.Empty 
                );
                if (interacted) { 
                    Debug.Log("interact pressed" + interacted);
                    Debug.Log("eventData" + eventData.IsEnteracted);
                    MarkAsUsed();
                }

                OnItemInteracted?.Invoke(eventData, itemParameters);
            }
        }
    }
}