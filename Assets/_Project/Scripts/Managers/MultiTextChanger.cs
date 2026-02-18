using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace Game
{
    public class MultiTextChanger : MonoBehaviour
    {
        [SerializeField] private List<TMP_Text> _textMeshProLinks;
        [SerializeField] private List<GameObject> _iconsLinks;

        [Header("Quest Settings")]
        [SerializeField] private string _defaultTextAllQuests = "Все квесты выполнены! Вы можете закончить день!";
        [SerializeField] private string _defaultTextStillQuests = "Основной квест выполнен! Вы можете пообщаться с другими жителями!";

        private void Update()
        {
            UpdateQuestText();
            SyncIcons();
        }

        private void UpdateQuestText()
        {
            try
            {
                var allGroups = Quest.QuestCollection.GetAllQuestGroups();

                int idx = 0;
                foreach (var group in allGroups
                                      .Where(q => q.InProgress && !q.Complite)
                                      .OrderByDescending(q => q.Prime))
                {
                    if (idx >= _textMeshProLinks.Count) break;
                    _textMeshProLinks[idx++].text = group.GetCurrentTask().TaskInfo;
                }


                int idxn = idx;
                for (; idxn < _textMeshProLinks.Count; idxn++)
                    _textMeshProLinks[idxn].text = "";

                if (idx > 0) return;

                // 0) Ни одного квеста ещё не стартовано
                if (allGroups.Count > 0 && allGroups.Any(q => !q.InProgress && !q.Complite))
                {
                    ShowSingleMessage(_defaultTextStillQuests);
                    return;
                }

                //1) Все квесты этого дня завершены
                if (allGroups.Count > 0 && allGroups.All(q => !q.InProgress && q.Complite))
                {
                    ShowSingleMessage(_defaultTextAllQuests);
                    return;
                }
            }
            catch (Exception e)
            {
                //Debug.LogError($"Error {e.Message}");
            }
        }

        private void ShowSingleMessage(string msg)
        {
            var allGroups = Quest.QuestCollection.GetAllQuestGroups();

            var avalibleQuests = allGroups.Where(q => q.InProgress && !q.Complite).OrderByDescending(q => q.Prime);
            var avalibleQuestsList = avalibleQuests.ToArray();
            for (int i = 0; i < _textMeshProLinks.Count; i++)
                _textMeshProLinks[i].text = (i == 0 ? msg : avalibleQuestsList[i].GetCurrentTask().TaskInfo);
        }

        private void SyncIcons()
        {
            // Сколько сейчас активных квестов?
            int activeQuests = Quest.QuestCollection.GetActiveQuestGroups().Count;
            // Проверяем, отображается ли общее сообщение (_defaultTextStillQuests или _defaultTextAllQuests)
            bool isShowingMessage = activeQuests == 0;

            // Определяем, для каких слотов показываем иконки
            for (int i = 0; i < _iconsLinks.Count; i++)
            {
                bool shouldShowIcon = false;
                if (i < _textMeshProLinks.Count)
                {
                    // Показываем иконку, если:
                    // 1) Текст в панели не пустой (для активных квестов или сообщения)
                    // 2) Индекс в пределах активных квестов или это сообщение в первом слоте
                    shouldShowIcon = !string.IsNullOrEmpty(_textMeshProLinks[i].text) &&
                                     (isShowingMessage ? i == 0 : i < activeQuests);
                }
                _iconsLinks[i].SetActive(shouldShowIcon);
            }
        }
    }

}

