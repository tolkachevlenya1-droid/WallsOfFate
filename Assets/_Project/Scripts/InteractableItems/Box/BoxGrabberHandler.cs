using Game.Data;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game
{
    internal class BoxGrabberHandler : MonoBehaviour, ITriggerHandler
    {
        [SerializeField] private List<InfluenceArea> influenceArias;

        private BoxMover currentBoxMover;
        private GameObject currentBox;

        private void OnEnable()
        {
            foreach (var area in influenceArias)
                area.OnEventTriggered += Handle;
        }

        private void OnDisable()
        {
            foreach (var area in influenceArias)
                area.OnEventTriggered -= Handle;
        }

        public void Handle(TriggerEvent eventData)
        {
            if (eventData.AreaType != InfluenceType.Object || !eventData.IsEnteracted)
                return;

            GameObject box = eventData.TriggerObj;

            if (!box.CompareTag("Box"))
                return;

            ToggleGrab(box);
        }

        private void ToggleGrab(GameObject box)
        {
            if (currentBox == box)
            {
                DetachBox();
            }
            else
            {
                if (currentBox != null)
                    DetachBox();

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