using Newtonsoft.Json;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    public static class QuestSaveLoader
    {
        public static bool LoadData()
        {
            if (Repository.TryGetData("QuestSchedule", out Quest.QuestSaveData saveData))
            {
                Quest.QuestCollection.Initialize(saveData);
                Debug.Log($"Loaded quest data for day {saveData.CurrentDay}");
                return true;
            }
            return false;
        }

        public static void LoadDefaultData()
        {
            TextAsset textAsset = UnityEngine.Resources.Load<TextAsset>("Data/Quests");
            if (textAsset == null)
            {
                Debug.LogError("Default quests file not found!");
                return;
            }

            try
            {
                var settings = new JsonSerializerSettings
                {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Ignore // Изменили на Ignore для большей устойчивости
                };

                var defaultData = JsonConvert.DeserializeObject<Quest.QuestSaveData>(textAsset.text, settings);

                if (defaultData == null)
                {
                    Debug.LogError("Failed to deserialize default quest data");
                    return;
                }

                Quest.QuestCollection.Initialize(defaultData);
                Debug.Log($"Loaded default quest data for day {defaultData.CurrentDay} with {defaultData.Days.Count} days");
            }
            catch (JsonException ex)
            {
                Debug.LogError($"JSON error: {ex.Message}\n{ex.StackTrace}");
            }
        }

        public static void SaveData()
        {
            var saveData = new Quest.QuestSaveData
            {
                CurrentDay = Quest.QuestCollection.CurrentDayNumber,
                Days = Quest.QuestCollection.GetAllDays()
            };

            Repository.SetData("QuestSchedule", saveData);
            Debug.Log($"Saved quest data for day {saveData.CurrentDay} with {saveData.Days.Count} days");
        }
    }
}