using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using Zenject;
using Game.Data;

public class CompositeTrigger : MonoBehaviour, ITriggerable
{
    [Header("Quest Settings")]
    [SerializeField] private string _selfName; 

    public event Action OnActivated;
    public bool IsDone { get; private set; }

    private QuestManager questManager;

    [Inject]
    private void Construct(QuestManager questManager)
    {
        this.questManager = questManager;
    }

    public void Triggered()
    {
        /*var availableQuests = QuestCollection.GetAllDays()
            .SelectMany(d => d.Quests)
            .Where(q => q.CheckOpen(_selfName))
            .ToList();

        if (availableGroups.Count > 0)
        {
            var group = availableGroups.First();
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

        foreach (var group in activeGroups) {
            taskToComplete = group.Tasks
                .Where(t => !t.IsDone && t.ForNPS == _selfName && t.CanComplete())
                .OrderBy(t => t.Id)
                .FirstOrDefault();

            if (taskToComplete != null) {
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