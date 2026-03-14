using Ink.Parsed;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Data
{

    public class QuestManager
    {
        private Dictionary<int, QuestData> questsData;
        private readonly Dictionary<int, QuestStatusData> questsStatusData = new();

        public QuestManager()
        {
            LoadResourcesData();
            InitializeQuestsStatus();
        }

        private void LoadResourcesData()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Data/Quests");
            if (textAsset == null)
            {
                Debug.LogError("Default resources file not found!");
                return;
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                var defaultData = JsonConvert.DeserializeObject<List<QuestData>>(textAsset.text, settings);
                questsData = defaultData.ToDictionary(q => q.Id);
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON error: {ex.Message}");
            }
        }

        private void InitializeQuestsStatus()
        {
            foreach (var quest in questsData.Values)
            {
                questsStatusData.Add(quest.Id, new QuestStatusData
                {
                    QuestId = quest.Id,
                    Status = QuestStatus.NotStarted,
                    TasksStatusData = quest.Tasks.ToDictionary(t => t.Id, t => new TaskStatusData
                    {
                        TaskId = t.Id,
                        Status = QuestStatus.NotStarted
                    })
                });
            }
        }

        public void LoadSavedQuestsStatus()
        {
            if (Repository.TryGetData("QuestsStatus", out List<QuestStatusData> savedStatus))
            {
                foreach (var status in savedStatus)
                {
                    questsStatusData[status.QuestId].Status = status.Status;
                    foreach (var taskStatus in status.TasksStatusData)
                    {
                        questsStatusData[status.QuestId].TasksStatusData[taskStatus.Key].Status = taskStatus.Value.Status;
                    }
                }

                Debug.Log("Loaded quests status data");
            }
        }

        public void SaveQuestsStatus()
        {
            Repository.SetData("QuestsStatus", questsStatusData.Values.ToList());
            Debug.Log("Saved quests status data");
        }
    }
}
