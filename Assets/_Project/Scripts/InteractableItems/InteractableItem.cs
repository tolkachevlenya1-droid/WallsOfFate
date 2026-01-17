using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game
{
    public enum ResourceType { Gold, Food, PeopleSatisfaction, CastleStrength }

    [RequireComponent(typeof(Collider))]
    public class InteractableItem : MonoBehaviour, ITriggerable
    {
        [SerializeField] private bool _dependFromQuests = false;

        [Header("Resource Settings")]
        public ResourceType resourceType;
        public int amount = 1;

        [TextArea]
        public string message = "+1 resource";
        public string Key = "key";
        public string localizationFileName = "intaractive-items";

        [Header("Floating Text")]
        public GameObject floatingTextPrefab;
        public Vector3 spawnOffset = new Vector3(0f, 2.5f, 0f);

        [Header("Post-Use Behavior")]
        public bool destroyAfterUse = true;

        [Header("Move-to settings")]
        [Tooltip("На каком расстоянии от предмета игрок останавливается")]
        [SerializeField] private float approachDistance = 1.2f;

        private Transform _player;
        private bool _hasBeenUsed = false;
        public bool HasBeenUsed => _hasBeenUsed;

        void Awake()
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) _player = go.transform;
            else Debug.LogError("Player not found — please tag the player object as 'Player'.");

            CheckUsability();
        }

        private void Update()
        {
            CheckUsability();
        }

        private void CheckUsability()
        {
            CompositeTrigger compositeTrigger = this.gameObject.GetComponent<CompositeTrigger>();

            string sceneName = SceneManager.GetActiveScene().name;
            if (InteractableItemCollection.TryGetItemState(sceneName, gameObject.name, out bool hasBeenUsed))
            {
                _hasBeenUsed = hasBeenUsed;
                if (_hasBeenUsed)
                {
                    if (destroyAfterUse)
                        gameObject.SetActive(false);
                    else
                    {
                        var col = GetComponent<Collider>();
                        if (col) col.enabled = false;
                    }
                    foreach (var o in GetComponentsInChildren<cakeslice.Outline>())
                        o.enabled = false;
                }
            }
        }
        void OnMouseUpAsButton()
        {
            if (_hasBeenUsed) return;

            var playerGO = GameObject.FindGameObjectWithTag("Player");
            var mover = playerGO?.GetComponent<PlayerMoveController>();
            if (mover == null) return;

            float approach = 1.2f;
            mover.MoveToAndCallback(
                /* target  */ this.transform,
                /* run     */ true,
                /* arrive  */ () => Interact(),
                /* stop    */ approach
            );
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

        public void Interact()
        {
            CompositeTrigger compositeTrigger = this.gameObject.GetComponent<CompositeTrigger>();
            if (_hasBeenUsed) return;
            else
            {
                if (_dependFromQuests)
                {
                    if (!compositeTrigger.IsDone) return;
                }
            }
            _hasBeenUsed = true;

            string sceneName = SceneManager.GetActiveScene().name;
            InteractableItemCollection.SetItemState(sceneName, gameObject.name, _hasBeenUsed);

            switch (resourceType)
            {
                case ResourceType.Gold:
                    Player.Resources.ChangeGold(amount);
                    break;
                case ResourceType.Food:
                    Player.Resources.ChangeFood(amount);
                    break;
                case ResourceType.PeopleSatisfaction:
                    Player.Resources.ChangePeopleSatisfaction(amount);
                    break;
                case ResourceType.CastleStrength:
                    Player.Resources.ChangeCastleStrength(amount);
                    break;
            }

            if (floatingTextPrefab != null && _player != null)
            {
                Vector3 worldPos = _player.position + spawnOffset;
                var ftGO = Instantiate(floatingTextPrefab, worldPos, Quaternion.identity);
                if (ftGO.TryGetComponent<FloatingText>(out var ft))
                    ft.SetText(message);
            }

            if (destroyAfterUse)
                gameObject.SetActive(false);
            else
            {
                var col = GetComponent<Collider>();
                if (col) col.enabled = false;
            }

            InteractableItemCollection.SetItemState(SceneManager.GetActiveScene().name, this.gameObject.name, _hasBeenUsed);

            foreach (var o in GetComponentsInChildren<cakeslice.Outline>())
                o.enabled = false;
        }

        public void Triggered() => Interact();

    }

}

