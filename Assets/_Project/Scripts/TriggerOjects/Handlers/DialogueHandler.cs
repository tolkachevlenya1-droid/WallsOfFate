using Game.Quest;
using NUnit.Framework.Interfaces;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    internal class DialogueHandler : MonoBehaviour, ITriggerHandler
    {
        [Header("Dialogue Settings")]
        [SerializeField] private string _defaultDialogue;

        public void Handle(TriggerIvent iventData)
        {
            DialogueManager _dialogueManager = DialogueManager.Instance;
            if (iventData.IsEnteracted && !_dialogueManager.IsInDialogue)
            {
                var activeGroups = QuestCollection.GetActiveQuestGroups();
                QuestGroup groupToUpdate = null;
                QuestTask taskToComplete = null;
                DialogueGraph dialogueGraph;

                foreach (var group in activeGroups)
                {
                    taskToComplete = group.Tasks
                        .Where(t => !t.IsDone && t.ForNPS == iventData.TriggerObj.name && t.CanComplete())
                        .OrderBy(t => t.Id)
                        .FirstOrDefault();

                    if (taskToComplete != null)
                    {
                        groupToUpdate = group;
                        break;
                    }
                }

                if (groupToUpdate != null)
                {
                    groupToUpdate.GetCurrentTask().CompleteTask();
                    groupToUpdate = UpdateGroupState(groupToUpdate);
                    var originalGroup = QuestCollection.GetAllQuestGroups()
                        .FirstOrDefault(g => g.Id == groupToUpdate.Id);
                    originalGroup?.CopyFrom(groupToUpdate);

                    dialogueGraph = GetDialogueGraph(taskToComplete.RequeredDialog);
                    _dialogueManager.StartDialogue(dialogueGraph);
                    return;
                }

                // Проверка на старт новых квестов
                var currentDay = QuestCollection.GetCurrentDayData();
                var availableGroups = currentDay != null
                    ? currentDay.Quests.Where(q => q.CheckOpen(iventData.TriggerObj.name)).ToList()
                    : new List<QuestGroup>();

                if (availableGroups.Count > 0)
                {
                    var group = availableGroups.First();
                    group.StartQuest();

                    dialogueGraph = GetDialogueGraph(group.OpenDialog);
                    _dialogueManager.StartDialogue(dialogueGraph);
                    return;
                }

                dialogueGraph = GetDialogueGraph(iventData.TriggerObj);
                _dialogueManager.StartDialogue(dialogueGraph);
            }
        }

        private DialogueGraph GetDialogueGraph(string name)
        {
            return this.GetComponents<DialogueGraph>().Where(t => t.GetName() == name).FirstOrDefault();
        }

        private DialogueGraph GetDialogueGraph(GameObject obj)
        {
            return obj.GetComponent<DialogueGraph>(); //.Where(t => t.GetName() == obj).FirstOrDefault();
        }

        private QuestGroup UpdateGroupState(QuestGroup group)
        {
            bool allTasksDone = group.Tasks.All(t => t.IsDone);

            return new QuestGroup
            {
                Id = group.Id,
                Complite = allTasksDone,
                InProgress = !allTasksDone,
                OpenNPS = group.OpenNPS,
                OpenDialog = group.OpenDialog,
                CurrentTaskId = allTasksDone
                    ? -1
                    : group.Tasks
                        .Where(t => !t.IsDone)
                        .OrderBy(t => t.Id)
                        .FirstOrDefault()?.Id ?? -1,
                Tasks = group.Tasks,
                Prime = group.Prime
            };
        }

    }
}

