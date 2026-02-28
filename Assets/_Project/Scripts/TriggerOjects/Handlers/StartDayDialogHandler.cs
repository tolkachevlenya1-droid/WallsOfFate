using Game.Quest;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    internal class StartDayDialogHandler : MonoBehaviour, ITriggerHandler
    {
        [Header("Dialogue Settings")]
        [SerializeField] private string _npcName;

        public void Handle(TriggerIvent iventData)
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

