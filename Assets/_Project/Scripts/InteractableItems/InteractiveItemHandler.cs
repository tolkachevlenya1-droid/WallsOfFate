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

        private void OnEnable()
        {
            foreach (var area in influenceArias)
            {
                area.OnItemInteracted += HandleInteraction;
            }
        }


        private void OnDisable()
        {
            foreach (var area in influenceArias)
            {
                area.OnItemInteracted -= HandleInteraction;
            }
        }

        public void HandleInteraction(TriggerEvent eventData, InteractableItemParameters itemParameters)
        {
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
