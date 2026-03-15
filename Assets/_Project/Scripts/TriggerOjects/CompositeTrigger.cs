using UnityEngine;
using System;
using System.Collections.Generic;
using Game.Quest;
using System.Linq;

public class CompositeTrigger : MonoBehaviour, ITriggerable
{
    [Header("Quest Settings")]
    [SerializeField] private string _selfName; 

    public event Action OnActivated;
    public bool IsDone { get; private set; }

    public void Triggered()
    {
        var availableGroups = QuestCollection.GetAllDays()
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

        }

    }

    private bool CanTriggerTask(QuestTask task)
    {
        return task.ForNPS == _selfName && task.CanComplete();
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
            Tasks = group.Tasks
        };
    }
}