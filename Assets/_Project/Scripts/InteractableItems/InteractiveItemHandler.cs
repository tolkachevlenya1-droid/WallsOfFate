using Game.Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace Game
{
    internal class InteractiveItemHandler : MonoBehaviour, ITriggerHandler
    {
        /*
         {
            "resourceType": 1,
            "amount": 5,
            "message": "Получено 5 монет!",
            "key": "gold_chest",
            "localizationFileName": "interactive-items",
            "spawnOffset": {"x": 0, "y": 2.5, "z": 0},
            "destroyAfterUse": true,
            "approachDistance": 1.2,
            "dependFromQuests": true,
            "requiredQuestIds": [1, 3, 5],
            "itemName": "Золотой сундук"
          }
         */

        [System.Serializable]
        public class ItemParameters
        {
            public ResourceType ResourceType = ResourceType.Gold;
            public int Amount = 1;
            public string Message = "+1 resource";
            public string Key = "key";
            public string LocalizationFileName = "interactive-items";
            public Vector3 SpawnOffset = new Vector3(0f, 2.5f, 0f);
            public bool DestroyAfterUse = true;
            public float ApproachDistance = 1.2f;
            public bool DependFromQuests = false;
            public List<int> RequiredQuestIds = new List<int>();
            public string ItemName = "";
        }

        [Header("References")]
        [SerializeField] private GameObject _floatingTextPrefab;
        [SerializeField] private List<InfluenceArea> influenceArias;

        private PlayerManager _playerManager;
        private Dictionary<GameObject, InteractableItemState> _itemStates = new Dictionary<GameObject, InteractableItemState>();

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        private void OnEnable()
        {
            foreach (var area in influenceArias)
            {
                area.OnEventTriggered += Handle;
            }
        }


        private void OnDisable()
        {
            foreach (var area in influenceArias)
            {
                area.OnEventTriggered -= Handle;
            }
        }

        public void Handle(TriggerEvent eventData)
        {
            if (!eventData.IsEnteracted) return;

            if (!_itemStates.TryGetValue(eventData.TriggerObj, out var state))
            {
                state = new InteractableItemState();
                _itemStates[eventData.TriggerObj] = state;
            }

            if (state.HasBeenUsed) return;

            ItemParameters parameters = ParseParameters(eventData.Parameters);

            //if (parameters.DependFromQuests)
            //{
            //    var compositeTrigger = eventData.TriggerObj.GetComponent<CompositeTrigger>();
            //    if (compositeTrigger != null && !compositeTrigger.IsDone)
            //    {
            //        return; 
            //    }
            //}

            InteractWithItem(eventData, parameters, state);
            
        }

        private void InteractWithItem(TriggerEvent eventData, ItemParameters parameters, InteractableItemState state)
        {
            state.HasBeenUsed = true;

            _playerManager.PlayerData.AddResource(parameters.ResourceType, parameters.Amount);

            ShowFloatingText(eventData.PlayerObj, parameters);

            HandlePostUseBehavior(eventData.TriggerObj, parameters);

            SaveItemState(eventData.TriggerObj, state.HasBeenUsed);

            Debug.Log($"Interacted with {eventData.TriggerObj.name}, received {parameters.Amount} {parameters.ResourceType}");
        }

        private ItemParameters ParseParameters(string jsonParameters)
        {
            if (string.IsNullOrEmpty(jsonParameters))
            {
                return new ItemParameters {};
            }

            try
            {
                return JsonUtility.FromJson<ItemParameters>(jsonParameters);
            }
            catch (System.Exception e)
            {
                Debug.LogError($"Error parsing JSON for {name}: {e.Message}\nJSON: {jsonParameters}");
                return new ItemParameters {};
            }
        }

        private void MovePlayerToItem(GameObject playerObj, GameObject itemObj, float approachDistance, System.Action onArrived)
        {
            var mover = playerObj.GetComponent<PlayerMoveController>();
            if (mover != null)
            {
                mover.MoveToAndCallback(
                    itemObj.transform,
                    true,  
                    onArrived,
                    approachDistance
                );
            }
            else
            {
                onArrived?.Invoke();
            }
        }

        private void ShowFloatingText(GameObject playerObj, ItemParameters parameters)
        {
            if (_floatingTextPrefab == null || playerObj == null) return;

            Vector3 worldPos = playerObj.transform.position + parameters.SpawnOffset;
            var ftGO = Instantiate(_floatingTextPrefab, worldPos, Quaternion.identity);

            if (ftGO.TryGetComponent<FloatingText>(out var ft))
            {
                ft.SetText(parameters.Message);
            }
        }

        private void HandlePostUseBehavior(GameObject itemObj, ItemParameters parameters)
        {
            if (parameters.DestroyAfterUse)
            {
                itemObj.SetActive(false);
            }
            else
            {
                var collider = itemObj.GetComponent<Collider>();
                if (collider != null) collider.enabled = false;
            }

            //foreach (var outline in itemObj.GetComponentsInChildren<cakeslice.Outline>())
            //{
            //    outline.enabled = false;
            //}
        }

        private void SaveItemState(GameObject itemObj, bool hasBeenUsed)
        {
            string sceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            InteractableItemCollection.SetItemState(sceneName, itemObj.name, hasBeenUsed);
        }

        private class InteractableItemState
        {
            public bool HasBeenUsed = false;
        }
    }
}
