using UnityEngine;
using Quest;
using System.Collections.Generic;
using System.Linq;

internal class DialogeTrigger : MonoBehaviour, ITriggerable {
    [Header("Dialogue Settings")]
    [SerializeField] private string _defaultDialogue;
    [SerializeField] private string _npcName;
    public GameObject PowerCheckPrefab;

    public bool IsDone { get; private set; }

    public void Triggered()
    {
        var activeGroups = QuestCollection.GetActiveQuestGroups();
        QuestGroup groupToUpdate = null;
        QuestTask taskToComplete = null;
        DialogueGraph dialogueGraph;
        DialogueManager _dialogueManager = DialogueManager.Instance;

        foreach (var group in activeGroups) {
            taskToComplete = group.Tasks
                .Where(t => !t.IsDone && t.ForNPS == _npcName && t.CanComplete())
                .OrderBy(t => t.Id)
                .FirstOrDefault();

            if (taskToComplete != null) {
                groupToUpdate = group;
                break;
            }
        }

        if (groupToUpdate != null) {
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
            ? currentDay.Quests.Where(q => q.CheckOpen(_npcName)).ToList()
            : new  List<QuestGroup>();

        if (availableGroups.Count > 0)
        {
            var group = availableGroups.First();
            group.StartQuest();

            dialogueGraph = GetDialogueGraph(group.OpenDialog);
            _dialogueManager.StartDialogue(dialogueGraph);
            return;
        }

        dialogueGraph = GetDialogueGraph(_defaultDialogue);
        _dialogueManager.StartDialogue(dialogueGraph);
    }

    private DialogueGraph GetDialogueGraph(string name) {
        return this.GetComponents<DialogueGraph>().Where(t => t.GetName() == name).FirstOrDefault();
    }

    private bool CanTriggerTask(QuestTask task, out string dialogue)
    {
        dialogue = task.RequeredDialog;
        return dialogue != null && task.ForNPS == _npcName && task.CanComplete();
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