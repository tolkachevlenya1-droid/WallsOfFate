using System;
using System.Collections.Generic;

namespace Game.Data
{
    [Serializable]
    public class QuestTask
    {
        public int Id;
        public string Name;
        public string Description;
        public List<int> Dependencies;
        public Dictionary<ResourceType, int> ResourcesReward;
    }

    [Serializable]
    public class Quest
    {
        public int Id;
        public string Name;
        public string Description;
        public bool Main;
        public List<QuestTask> Tasks;
        public Dictionary<ResourceType, int> ResourcesReward = new();
    }

    public class TaskStatus
    {
        public int TaskId;
        public QuestState State;
    }

    [Serializable]
    public class QuestStatus
    {
        public int QuestId;
        public QuestState State;
        public Dictionary<int, TaskStatus> TasksStatusData;
    }

    public enum QuestState
    {
        NotStarted,
        InProgress,
        Completed
    }
}
