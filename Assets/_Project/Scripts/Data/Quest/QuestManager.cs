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
        private Dictionary<int, Quest> questsData;
        private readonly Dictionary<int, QuestStatus> questsStatusData = new();

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

                var defaultData = JsonConvert.DeserializeObject<List<Quest>>(textAsset.text, settings);
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
                questsStatusData.Add(quest.Id, new QuestStatus
                {
                    QuestId = quest.Id,
                    State = QuestState.NotStarted,
                    TasksStatusData = quest.Tasks.ToDictionary(t => t.Id, t => new TaskStatus
                    {
                        TaskId = t.Id,
                        State = QuestState.NotStarted
                    })
                });
            }
        }

        public void LoadSavedQuestsStatus()
        {
            if (Repository.TryGetData("QuestsStatus", out List<QuestStatus> savedStatus))
            {
                foreach (var status in savedStatus)
                {
                    questsStatusData[status.QuestId].State = status.State;
                    foreach (var taskStatus in status.TasksStatusData)
                    {
                        questsStatusData[status.QuestId].TasksStatusData[taskStatus.Key].State = taskStatus.Value.State;
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

        public QuestState GetQuestState(int questId)
        {
            questsStatusData.TryGetValue(questId, out QuestStatus status);

            return status != null ? status.State : QuestState.NotStarted;
        }

        public void StartQuest(int questId)
        {
            if (questsStatusData.TryGetValue(questId, out QuestStatus status) && status.State == QuestState.NotStarted)
            {
                status.State = QuestState.InProgress;
            }
        }
    }
}
