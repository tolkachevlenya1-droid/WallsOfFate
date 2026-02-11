using Game.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;

namespace Game
{
    public class CreateCharacterController : MonoBehaviour
    {
        [SerializeField] private TMP_Text StatsPool;
        [SerializeField] private TMP_Text Strength;
        [SerializeField] private TMP_Text Int;
        [SerializeField] private TMP_Text Dex;
        [SerializeField] private TMP_Text Percept;
        [SerializeField] private TMP_Text Mystic;

        [SerializeField] private GameObject ConfirmatiionButton;

        private PlayerManager playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager)
        {
            this.playerManager = playerManager;
        }

        private Dictionary<StatType, TMP_Text> statTextFields;

        public void Start()
        {
            statTextFields = new Dictionary<StatType, TMP_Text>
            {
                { StatType.Strength, Strength },
                { StatType.Int, Int },
                { StatType.Dex, Dex },
                { StatType.Percept, Percept },
                { StatType.Mystic, Mystic }
            };
            UpdateAllStatsUI();
        }

        private void UpdateStatUI(StatType type, int newValue)
        {
            if (statTextFields.TryGetValue(type, out TMP_Text textField))
            {
                textField.text = $"{type}: {newValue}";
            }
        }

        private void UpdateAllStatsUI()
        {
            var statTypes = Enum.GetValues(typeof(StatType)).Cast<StatType>();

            foreach (var type in statTypes)
            {
                UpdateStatUI(type, playerManager.PlayerData.GetStat(type));
            }

            UpdateStatsPoolUI();
        }

        private void ActivateConfirmationButton()
        {
            if (playerManager.PlayerData.FreePoints == 0) ConfirmatiionButton.SetActive(true);
            else ConfirmatiionButton.SetActive(false);
        }

        private void UpdateStatsPoolUI()
        {
            if (StatsPool != null)
            {
                StatsPool.text = $"Amount: { playerManager.PlayerData.FreePoints }";
            }
        }

        public void IncreaseStrength()
        {
            playerManager.IncreaseStat(StatType.Strength);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaseInt()
        {
            playerManager.IncreaseStat(StatType.Int);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaseDex()
        {
            playerManager.IncreaseStat(StatType.Dex);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreasePercept()
        {
            playerManager.IncreaseStat(StatType.Percept);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaseMystic()
        {
            playerManager.IncreaseStat(StatType.Mystic);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }

        public void DecreaseStrength()
        {
            playerManager.DecreaseStat(StatType.Strength);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaseInt()
        {
            playerManager.DecreaseStat(StatType.Int);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaseDex()
        {
            playerManager.DecreaseStat(StatType.Dex);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreasePercept()
        {
            playerManager.DecreaseStat(StatType.Percept);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaseMystic()
        {
            playerManager.DecreaseStat(StatType.Mystic);

            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
    }
}