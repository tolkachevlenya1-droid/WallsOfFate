using Game.Data;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;
using Zenject;

namespace Game
{
    public class InteractiveItemHandler : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private GameObject _floatingTextPrefab;
        [SerializeField] private List<InteractibleItemInfluenceArea> influenceArias;


        private PlayerManager _playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            _playerManager = playerManager;
        }

        private void Start()
        {
            foreach (var area in influenceArias)
            {
                area.OnItemInteracted += HandleInteraction;
            }
        }


        private void OnDestroy()
        {
            foreach (var area in influenceArias)
            {
                area.OnItemInteracted -= HandleInteraction;
            }            
        }

        public void HandleInteraction(TriggerEvent eventData, InteractableItemParameters itemParameters)
        {
            var area = influenceArias.Find(a => a.Parameters == itemParameters);
            if (area != null)
            {
                var fieldInfo = typeof(InteractibleItemInfluenceArea)
                    .GetField("OnItemInteracted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (fieldInfo != null)
                {
                    var delegateValue = fieldInfo.GetValue(area) as System.Delegate;
                    if (delegateValue != null)
                    {
                        var subscribers = delegateValue.GetInvocationList();
                        Debug.Log($"Найдено подписчиков: {subscribers.Length}");
                        foreach (var sub in subscribers)
                        {
                            Debug.Log($"Подписчик: {sub.Target?.GetType().Name} -> {sub.Method.Name}");
                        }
                    }
                }
            }

            if (eventData.IsEnteracted)
            {
                Debug.Log("eventData.IsEnteracted" + eventData.IsEnteracted);
            }
            if (!eventData.IsEnteracted) return;

            UpdateResources(eventData.PlayerObj, itemParameters);

            ShowFloatingText(eventData.PlayerObj, itemParameters);

            HandlePostUseBehavior(eventData.TriggerObj, itemParameters);

            Debug.Log($"Interacted with {eventData.TriggerObj.name}, received {itemParameters.Amount} {itemParameters.ResourceType}");

        }

        private void UpdateResources(GameObject player, InteractableItemParameters parameters)
        {
            _playerManager.PlayerData.AddResource(parameters.ResourceType, parameters.Amount);
        }


        private void ShowFloatingText(GameObject playerObj, InteractableItemParameters parameters)
        {
            if (_floatingTextPrefab == null || playerObj == null) return;

            Vector3 worldPos = playerObj.transform.position + parameters.SpawnOffset;
            var ftGO = Instantiate(_floatingTextPrefab, worldPos, Quaternion.identity);

            if (ftGO.TryGetComponent<FloatingText>(out var ft))
            {
                ft.SetText(parameters.Message);
            }
        }

        private void HandlePostUseBehavior(GameObject itemObj, InteractableItemParameters parameters)
        {
            if (parameters.DestroyAfterUse)
            {
                itemObj.SetActive(false);
            }
            //else
            //{
            //    var collider = itemObj.GetComponent<Collider>();
            //    if (collider != null) collider.enabled = false;
            //}        
        }
    }
}
