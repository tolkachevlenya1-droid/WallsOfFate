using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class RandomFactDisplay : MonoBehaviour
    {
        [Header("Настройки фактов")]
        [Tooltip("Компонент TextMeshPro для отображения факта.")]
        public TMP_Text factText;

        [Tooltip("Массив фактов, из которого выбирается один случайный.")]
        [TextArea(2, 5)]
        public List<string> facts;

        private static int currentIndex = 0;
        private static bool allFactsShown = false;

        private void OnEnable()
        {
            RefreshFact();
        }

        public void RefreshFact()
        {
            if (factText != null && facts != null && facts.Count > 0)
            {
                string selectedFact;

                if (!allFactsShown)
                {
                    selectedFact = facts[currentIndex];
                    currentIndex++;

                    if (currentIndex >= facts.Count)
                    {
                        allFactsShown = true;
                    }
                }
                else
                {
                    int randomIndex = Random.Range(0, facts.Count);
                    selectedFact = facts[randomIndex];
                }

                factText.text = selectedFact;
            }
        }
    }

}
