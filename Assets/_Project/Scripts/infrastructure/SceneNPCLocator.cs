using System;
using System.Collections.Generic;
using System.Linq;
using Game.Quest;
using UnityEngine;
using Zenject;

namespace Game
{
    internal class SceneNPCLocator : MonoInstaller
    {
        public List<GameObject> NPC; // Список NPC префабов
        public Transform Parent; // Родительский объект для NPC

        public List<Transform> StartPoints; // Список точек старта для каждого NPC
        public List<bool> EnableCheck; 

        public override void InstallBindings()
        {
            InstantiateNPSPrefab();
        }

        private void InstantiateNPSPrefab()
        {
            if (NPC.Count != StartPoints.Count)
            {
                //Debug.LogError("Количество NPC и точек старта должно быть одинаковым!");
                return;
            }

            for (int i = 0; i < NPC.Count; i++)
            {
                GameObject prefab = NPC[i];
                Transform startPoint = StartPoints[i];

                // Проверяем наличие нужных компонентов на префабе или его дочерних объектах
                bool shouldInstantiate = CheckQuestConditions(prefab);

                if (shouldInstantiate || !EnableCheck[i])
                {
                    // Получаем позицию и поворот из Transform точки старта
                    Vector3 spawnPosition = startPoint.position;
                    Quaternion spawnRotation = startPoint.rotation;

                    // Инстанцируем NPC
                    GameObject instance = Instantiate(prefab, spawnPosition, spawnRotation, Parent);
                }
                else
                {
                    //Debug.Log($"NPC {prefab.name} не создан, так как не выполнены условия квеста.");
                }
            }
        }

        private bool CheckQuestConditions(GameObject prefab)
        {
            // Проверяем наличие компонентов на префабе или его дочерних объектах
            CompositeTrigger compositeTrigger = prefab.GetComponentInChildren<CompositeTrigger>();
            DialogueTrigger dialogeTrigger = prefab.GetComponentInChildren<DialogueTrigger>();
            Pickup pickup = prefab.GetComponentInChildren<Pickup>();

            // Если ни один из компонентов не найден, создаем NPC без дополнительных проверок
            if (compositeTrigger == null && dialogeTrigger == null && pickup == null)
            {
                return true;
            }

            var currentDayData = QuestCollection.GetCurrentDayData();
            if (currentDayData == null)
            {
                return false; // Нет данных о текущем дне, не создаем NPC
            }

            // Проверка для CompositeTrigger
            if (compositeTrigger != null)
            {
                string selfName = compositeTrigger.GetType().GetField("_selfName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(compositeTrigger) as string;
                if (string.IsNullOrEmpty(selfName))
                {
                    //Debug.LogWarning($"CompositeTrigger на {prefab.name} имеет пустое _selfName.");
                    return false;
                }

                // Проверяем, есть ли квесты с OpenNPS или ForNPS, равным _selfName
                bool hasMatchingQuest = currentDayData.Quests.Any(q =>
                    (q.OpenNPS == selfName && !q.InProgress && !q.Complite) ||
                    q.Tasks.Any(t => t.ForNPS == selfName && !t.IsDone));

                if (!hasMatchingQuest)
                {
                    return false;
                }
            }

            // Проверка для DialogeTrigger
            if (dialogeTrigger != null)
            {
                string npcName = dialogeTrigger.GetType().GetField("_npcName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)?.GetValue(dialogeTrigger) as string;
                if (string.IsNullOrEmpty(npcName))
                {
                    //Debug.LogWarning($"DialogeTrigger на {prefab.name} имеет пустое _npcName.");
                    return false;
                }

                // Проверяем, есть ли квесты с OpenNPS или ForNPS, равным _npcName
                bool hasMatchingQuest = currentDayData.Quests.Any(q =>
                    (q.OpenNPS == npcName) ||
                    q.Tasks.Any(t => t.ForNPS == npcName));

                if (!hasMatchingQuest)
                {
                    return false;
                }
            }

            // Проверка для Pickup
            if (pickup != null)
            {
                string pickupType = pickup.Type;
                if (string.IsNullOrEmpty(pickupType))
                {
                    //Debug.LogWarning($"Pickup на {prefab.name} имеет пустое Type.");
                    return false;
                }

                // Проверяем, есть ли квест-группа с Evidence != null
                bool hasEvidenceQuest = currentDayData.Quests.Any(q => q.Evidence != null);
                if (!hasEvidenceQuest)
                {
                    return false;
                }

                // Проверяем совпадение Type pickup с EvidenceType
                bool typeMatches = currentDayData.Quests.Any(q =>
                    q.Evidence != null && q.Evidence.EvidenceType == pickupType);

                if (!typeMatches)
                {
                    return false;
                }
            }

            return true; // Все проверки пройдены, можно создавать NPC
        }
    }
}
