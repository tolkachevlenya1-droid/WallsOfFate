using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    internal class CompositeTriggerHandler : MonoBehaviour, ITriggerHandler
    {
        [Header("Quest Settings")]
        //[SerializeField] private string defaultName;

        public bool IsDone { get; private set; }
        [SerializeField] private List<InfluenceArea> influenceArias;

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

            /*var availableQuests = QuestCollection.GetAllDays()
                .SelectMany(d => d.Quests)
                .Where(q => q.CheckOpen(eventData.Parameters))
                .ToList();

            if (availableQuests.Count > 0)
            {
                var group = availableQuests.First();
                group.StartQuest();
                IsDone = true;
                return;
            }

            var activeGroups = QuestCollection.GetActiveQuestGroups();
            //var groupToUpdate = activeGroups.FirstOrDefault(g =>
            //    g.GetCurrentTask() != null &&
            //    CanTriggerTask(g.Tasks.FirstOrDefault(t => t.CanComplete())));

            QuestGroup groupToUpdate = null;
            QuestTask taskToComplete = null;

            foreach (var group in activeGroups)
            {
                taskToComplete = group.Tasks
                    .Where(t => !t.IsDone && t.ForNPS == eventData.Parameters && t.CanComplete())
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
                //var task = groupToUpdate.GetCurrentTask();
                taskToComplete.CompleteTask();

                groupToUpdate.TryCompleteGroup();

                var next = groupToUpdate.Tasks
                    .Where(t => !t.IsDone)
                    .OrderBy(t => t.Id)
                    .FirstOrDefault();
                groupToUpdate.CurrentTaskId = next != null ? next.Id : -1;
                IsDone = true;

            }*/
        }
    }
}
