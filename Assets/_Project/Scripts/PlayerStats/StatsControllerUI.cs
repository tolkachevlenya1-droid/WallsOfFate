using TMPro;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Player
{
    public class StatsControllerUI : MonoBehaviour
    {
        [SerializeField] private GameObject Strength;
        [SerializeField] private GameObject Int;
        [SerializeField] private GameObject Dex;
        [SerializeField] private GameObject Percept;
        [SerializeField] private GameObject Mystic;

        [SerializeField] private Sprite statOnIcon;
        [SerializeField] private Sprite statOfIcon;

        [Header("Параметры для пула статов")]
        [SerializeField] private GameObject StatsPool;
        [SerializeField] private GameObject StatsPoolIconPrefab;
        [SerializeField] private int NumberCirclesInLine;
        [SerializeField] private int LineSpacing;
        [SerializeField] private int IntercharSpacing;
        
        private List<GameObject> StatsPrefabsList;

        private Player.Stats _playerStas;

        [Inject]
        private void Construct(Player.Stats playerStats) {
            _playerStas = ProjectContext.Instance.Container.Resolve<Player.Stats>();
        }

        private void Start() {
            StatsPrefabsList = new List<GameObject>();
        }

        #region ButtonMethods

        public void ChangeStrength(Image buttImage) {
            if (buttImage.sprite == statOfIcon) IncreaceStrength();
            else DecreaceStrength();
        }

        public void ChangeInt(Image buttImage) {
            if (buttImage.sprite == statOfIcon) IncreaceInt();
            else DecreaceInt();
        }
        public void ChangeDex(Image buttImage) {
            if (buttImage.sprite == statOfIcon) IncreaceDex();
            else DecreaceDex();
        }
        public void ChangePercept(Image buttImage) {
            if (buttImage.sprite == statOfIcon) IncreacePercept();
            else DecreacePercept();
        }
        public void ChangeMyst(Image buttImage) {
            if (buttImage.sprite == statOfIcon) IncreaceMystic();
            else DecreaceMystic();
        }

        #endregion

        #region Update

        private void SetAllComponentsFalse(GameObject obj) {
            for (int i = 0; i < obj.transform.childCount; i++) {
                obj.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOfIcon;
            }
        }

        private void UpdateStrengthUI() {
            if (Strength != null) {
                SetAllComponentsFalse(Strength);
                int amount = _playerStas.Strength;
                for (int i = 0; i < Strength.transform.childCount && amount > 0; i++, amount--) {
                    Strength.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
            }
        }

        private void UpdateIntUI() {
            if (Int != null) {
                SetAllComponentsFalse(Int);
                int amount = _playerStas.Int;
                for (int i = 0; i < Int.transform.childCount && amount > 0; i++, amount--) {
                    Int.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
            }
        }

        private void UpdateDexUI() {
            if (Dex != null) {
                SetAllComponentsFalse(Dex);
                int amount = _playerStas.Dex;
                for (int i = 0; i < Dex.transform.childCount && amount > 0; i++, amount--) {
                    Dex.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
            }
        }

        private void UpdatePerceptUI() {
            if (Percept != null) {
                SetAllComponentsFalse(Percept);
                int amount = _playerStas.Percept;
                for (int i = 0; i < Percept.transform.childCount && amount > 0; i++, amount--) {
                    Percept.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
            }
        }

        private void UpdateMysticUI() {
            if (Mystic != null) {
                SetAllComponentsFalse(Mystic);
                int amount = _playerStas.Mystic;
                for (int i = 0; i < Mystic.transform.childCount && amount > 0; i++, amount--) {
                    Mystic.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
            }

        }

        #region StatsPool

        private void UpdateStatsPoolUI() {
            ClearPoints();
            DrawPoints();
        }

        private void DrawPoints() {
            int numOfLines = (int)Mathf.Ceil((float)_playerStas.FreePoints / (float)NumberCirclesInLine);
            for (int i = 0; i < numOfLines; i++) {
                for (int j = 0; j < NumberCirclesInLine && j < _playerStas.FreePoints; j++) {
                    Vector3 position = StatsPool.transform.position;
                    if (StatsPrefabsList.Count != 0) {
                        position.x += IntercharSpacing * j;
                        position.y -= LineSpacing * i;
                    }
                    GameObject obj = Instantiate(StatsPoolIconPrefab, position, Quaternion.identity, StatsPool.transform);
                    StatsPrefabsList.Add(obj);
                }
            }
        }

        private void ClearPoints() {
            StatsPrefabsList.Clear();

            List<Transform> StatsPoolChild = new List<Transform>();
            for (int i = 0; i < StatsPool.transform.childCount; i++) {
                Destroy(StatsPool.transform.GetChild(i).gameObject);
            }
        }
        #endregion

        public void UpdateAllStatsUI() {
            UpdateStrengthUI();
            UpdateIntUI();
            UpdateDexUI();
            UpdatePerceptUI();
            UpdateMysticUI();
            UpdateStatsPoolUI();
        }
        #endregion

        #region Utility


        private void IncreaceStrength() {
            if (_playerStas.FreePoints > 0 && _playerStas.Strength <= 4) {
                _playerStas.AddFreePoints(-1);
                _playerStas.AddStrength(1);
            }
            UpdateAllStatsUI();
        }

        private void IncreaceInt() {
            if (_playerStas.FreePoints > 0 && _playerStas.Int <= 4) {
                _playerStas.AddFreePoints(-1);
                _playerStas.AddInt(1);
            }
            UpdateAllStatsUI();
        }

        private void IncreaceDex() {
            if (_playerStas.FreePoints > 0 && _playerStas.Dex <= 4) {
                _playerStas.AddFreePoints(-1);
                _playerStas.AddDex(1);
            }
            UpdateAllStatsUI();
        }

        private void IncreacePercept() {
            if (_playerStas.FreePoints > 0 && _playerStas.Percept <= 4) {
                _playerStas.AddFreePoints(-1);
                _playerStas.AddPerceept(1);
            }
            UpdateAllStatsUI();
        }

        private void IncreaceMystic() {
            if (_playerStas.FreePoints > 0 && _playerStas.Mystic <= 4) {
                _playerStas.AddFreePoints(-1);
                _playerStas.AddMystic(1);
            }
            UpdateAllStatsUI();
        }
        private void DecreaceStrength() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Strength != 0) _playerStas.AddFreePoints(1);
                _playerStas.AddStrength(-1);
            }
            UpdateAllStatsUI();
        }
        private void DecreaceInt() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Int != 0) _playerStas.AddFreePoints(1);
                _playerStas.AddInt(-1);
            }
            UpdateAllStatsUI();
        }
        private void DecreaceDex() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Dex != 0) _playerStas.AddFreePoints(1);
                _playerStas.AddDex(-1);
            }
            UpdateAllStatsUI();
        }
        private void DecreacePercept() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Percept != 0) _playerStas.AddFreePoints(1);
                _playerStas.AddPerceept(-1);
            }
            UpdateAllStatsUI();
        }
        private void DecreaceMystic() {
            if (_playerStas.FreePoints >= 0) {
                if (_playerStas.Mystic != 0) _playerStas.AddFreePoints(1);
                _playerStas.AddMystic(-1);
            }
            UpdateAllStatsUI();
        }
        #endregion
    }
}