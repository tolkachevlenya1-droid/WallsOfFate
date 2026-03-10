using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Game
{
    [Serializable]
    public class Task
    {
        public readonly int Id;

        public QuestStatus Status = QuestStatus.NotStarted;

        public Dictionary<Data.ResourceType, int> ResourcesReward = new();

        public List<int> Dependencies = new();
    }
}