using TMPro;
using UnityEngine;
using Zenject;

namespace Player {
    public class StatsController : MonoBehaviour {
        [SerializeField] private TMP_Text StatsPool;
        [SerializeField] private TMP_Text Strength;
        [SerializeField] private TMP_Text Int;
        [SerializeField] private TMP_Text Dex;
        [SerializeField] private TMP_Text Percept;
        [SerializeField] private TMP_Text Mystic;

        [SerializeField] private GameObject ConfirmatiionButton;

        private Player.Stats _playerStas;

        [Inject]
        private void Construct(Player.Stats playerStats) {
            _playerStas = playerStats;
        }

        private void UpdateStrengthUI(int newValue) {
            if (Strength != null)
                Strength.text = $"Strength: {newValue}";
        }
        private void UpdateIntUI(int newValue) {
            if (Int != null)
                Int.text = $"Int: {newValue}";
        }
        private void UpdateDexUI(int newValue) {
            if (Dex != null)
                Dex.text = $"Dex: {newValue}";
        }
        private void UpdatePerceptUI(int newValue) {
            if (Percept != null)
                Percept.text = $"Percept: {newValue}";
        }
        private void UpdateMysticUI(int newValue) {
            if (Mystic != null)
                Mystic.text = $"Mystic: {newValue}";
        }

        private void UpdateAllStatsUI() {
            UpdateStrengthUI(_playerStas.Strength);
            UpdateIntUI(_playerStas.Int);
            UpdateDexUI(_playerStas.Dex);
            UpdatePerceptUI(_playerStas.Percept);
            UpdateMysticUI(_playerStas.Mystic);
            UpdateStatsPoolUI();
        }

        private void ActivateConfirmationButton() {
            if (_playerStas.FreePoints == 0) ConfirmatiionButton.SetActive(true);
            else ConfirmatiionButton.SetActive(false);
        }

        private void UpdateStatsPoolUI() {
            if (StatsPool != null) {
                StatsPool.text = $"Amount: {_playerStas.FreePoints}";
            }
        }

        public void IncreaceStrength() {
            if (_playerStas.FreePoints > 0 && _playerStas.Strength <= 4) {
                _playerStas.FreePoints--;
                _playerStas.AddStrength(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaceInt() { 
            if (_playerStas.FreePoints > 0 && _playerStas.Int <= 4) {
                _playerStas.FreePoints--;
                _playerStas.AddInt(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaceDex() { 
            if (_playerStas.FreePoints > 0 && _playerStas.Dex <= 4) {
                _playerStas.FreePoints--;
                _playerStas.AddDex(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreacePerceept() { 
            if (_playerStas.FreePoints > 0 && _playerStas.Percept <= 4) {
                _playerStas.FreePoints--;
                _playerStas.AddPerceept(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void IncreaceMystic() { 
            if (_playerStas.FreePoints > 0 && _playerStas.Mystic <= 4)  {
                _playerStas.FreePoints--;
                _playerStas.AddMystic(1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }     
        
        public void DecreaceStrength() { 
            if (_playerStas.FreePoints >= 0) {
                if(_playerStas.Strength != 0) _playerStas.FreePoints++;
                _playerStas.AddStrength(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaceInt() {
            if(_playerStas.FreePoints >= 0) {
                if (_playerStas.Int != 0) _playerStas.FreePoints++;
                _playerStas.AddInt(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaceDex() {
            if(_playerStas.FreePoints >= 0) {
                if (_playerStas.Dex != 0) _playerStas.FreePoints++;
                _playerStas.AddDex(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreacePerceept() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Percept != 0) _playerStas.FreePoints++;
                _playerStas.AddPerceept(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
        public void DecreaceMystic() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Mystic != 0) _playerStas.FreePoints++;
                _playerStas.AddMystic(-1);
            }
            UpdateAllStatsUI();
            ActivateConfirmationButton();
        }
    }
}