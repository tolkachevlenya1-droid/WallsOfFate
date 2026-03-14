using System;
using System.Collections.Generic;

namespace Game.Data
{
    [Serializable]
    public class QuestTaskData
    {
        public int Id;
        public string Name;
        public string Description;
        public List<int> Dependencies;
        public Dictionary<ResourceType, int> ResourcesReward;
    }

    [Serializable]
    public class QuestData
    {
        public int Id;
        public string Name;
        public string Description;
        public bool Main;
        public List<QuestTaskData> Tasks;
        public Dictionary<ResourceType, int> ResourcesReward = new();
    }

    public class TaskStatusData
    {
        public int TaskId;
        public QuestStatus Status;
    }

    [Serializable]
    public class QuestStatusData
    {
        public int QuestId;
        public QuestStatus Status;
        public Dictionary<int, TaskStatusData> TasksStatusData;
    }

    public enum QuestStatus
    {
        NotStarted,
        InProgress,
        Completed
    }
}
