using Assets._Project.Scripts.Triggers;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    internal class StartDayDialogueHandler : MonoBehaviour, ITriggerHandler
    {
        [SerializeField] private StartDayDialogueTriggerZone influenceAria;

        private void OnEnable()
        {
            influenceAria.OnEventTriggered += Handle;
        }


        private void OnDisable()
        {
            influenceAria.OnEventTriggered -= Handle;
        }

        public void Handle(TriggerEvent iventData)
        {
            DialogueGraph dialogueGraph;
            DialogueManager _dialogueManager = DialogueManager.Instance;

            GameObject npc = iventData.TriggerObj.transform.gameObject;
            dialogueGraph = GetDialogueGraph(npc);
            _dialogueManager.StartDialogue(dialogueGraph);
        }

        private DialogueGraph GetDialogueGraph(GameObject obj)
        {
            return obj.GetComponent<DialogueGraph>(); //.Where(t => t.GetName() == obj).FirstOrDefault();
        }
    }
}

