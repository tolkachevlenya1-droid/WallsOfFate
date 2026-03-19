using Game.Data;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    internal class BoxGrabberHandler : MonoBehaviour, ITriggerHandler
    {
        [SerializeField] private List<InfluenceArea> influenceArias;
        private Dictionary<GameObject, InfluenceArea> areaByObject;

        private BoxMover currentBoxMover;
        private GameObject currentBox;

        private void OnEnable()
        {
            //foreach (var area in influenceArias)
            //    area.OnEventTriggered += Handle;

            areaByObject = new Dictionary<GameObject, InfluenceArea>();

            foreach (var area in influenceArias)
            {
                //area.Handler = this;
                //if (!areaByObject.ContainsKey(area.gameObject))
                //{
                //    areaByObject.Add(area.gameObject, area);
                //    area.OnEventTriggered.AddListener(Handle);
                //}
            }
        }

        private void OnDisable()
        {
            foreach (var area in influenceArias)
                area.OnEventTriggered.RemoveListener(Handle);
        }

        public void Handle(TriggerEvent eventData)
        {
            if (eventData.AreaType != InfluenceType.Object || !eventData.IsEnteracted)
                return;

            GameObject box = eventData.TriggerObj;

            if (!box.CompareTag("Box"))
                return;

            ToggleGrab(eventData);
            //if (areaByObject.ContainsKey(eventData.TriggerObj))
            //{

            //}

        }

        private void ToggleGrab(TriggerEvent eventData)
        {
            GameObject box = eventData.TriggerObj;
            if (currentBox == box)
            {
                DetachBox();
            }
            else
            {
                if (currentBox != null)
                    DetachBox();

                PlayerAnimatinController playerAnimator = eventData.PlayerObj.GetComponent<PlayerAnimatinController>();
                playerAnimator?.InteractWith(eventData);
                AttachBox(box);
            }
        }

        private void AttachBox(GameObject box)
        {
            currentBox = box;
            currentBoxMover = box.GetComponent<BoxMover>();

            if (currentBoxMover == null)
                currentBoxMover = box.AddComponent<BoxMover>();

            currentBoxMover.StartHolding();
            Debug.Log($"Box grabbed: {box.name}");
        }

        private void DetachBox()
        {
            if (currentBoxMover != null)
                currentBoxMover.StopHolding();

            Debug.Log($"Box released: {currentBox.name}");
            currentBox = null;
            currentBoxMover = null;
        }
    }
}