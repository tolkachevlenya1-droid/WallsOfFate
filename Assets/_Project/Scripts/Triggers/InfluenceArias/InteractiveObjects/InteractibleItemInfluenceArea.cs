using System;
using UnityEngine;
using UnityEngine.SceneManagement;
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

        public void ResetForRespawn()
        {
            _hasBeenUsed = false;

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
                if (interacted)
                {
                    //Debug.Log("interact pressed" + interacted);
                    //Debug.Log("eventData" + eventData.IsEnteracted);
                    MarkAsUsed();
                }

                OnItemInteracted?.Invoke(eventData, itemParameters);
            }
        }
    }
}