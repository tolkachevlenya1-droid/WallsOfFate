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
        [SerializeField] private GameObject StatsPool;
        [SerializeField] private GameObject Strength;
        [SerializeField] private GameObject StrengthMainIconShadow;
        [SerializeField] private GameObject Int;
        [SerializeField] private GameObject IntMainIconShadow;
        [SerializeField] private GameObject Dex;
        [SerializeField] private GameObject DexMainIconShadow;
        [SerializeField] private GameObject Percept;
        [SerializeField] private GameObject PerceptMainIconShadow;
        [SerializeField] private GameObject Mystic;
        [SerializeField] private GameObject MysticMainIconShadow;

        private Player.Stats _playerStas;

        [Inject]
        private void Construct(Player.Stats playerStats) {
            _playerStas = ProjectContext.Instance.Container.Resolve<Player.Stats>();
        }

        #region ButtonMethods

        public void ChangeStrength(Image buttImage) {
            if (buttImage.enabled == false) IncreaceStrength();
            else DecreaceStrength();
        }

        public void ChangeInt(Image buttImage) {
            if (buttImage.enabled == false) IncreaceInt();
            else DecreaceInt();
        }
        public void ChangeDex(Image buttImage) {
            if (buttImage.enabled == false) IncreaceDex();
            else DecreaceDex();
        }
        public void ChangePercept(Image buttImage) {
            if (buttImage.enabled == false) IncreacePercept();
            else DecreacePercept();
        }
        public void ChangeMyst(Image buttImage) {
            if (buttImage.enabled == false) IncreaceMystic();
            else DecreaceMystic();
        }

        public void IcChangeStrength() {
            if (_playerStas.Strength <= 5 && _playerStas.FreePoints > 0) IncreaceStrength();
            else DecreaceStrength();
        }

        public void IcChangeInt(Image buttImage) {
            if (_playerStas.Strength <= 5 && _playerStas.FreePoints > 0) IncreaceInt();
            else DecreaceInt();
        }
        public void IcChangeDex(Image buttImage) {
            if (_playerStas.Strength <= 5 && _playerStas.FreePoints > 0) IncreaceDex();
            else DecreaceDex();
        }
        public void IcChangePercept() {
            if (_playerStas.Strength <= 5 && _playerStas.FreePoints > 0) IncreacePercept();
            else DecreacePercept();
        }
        public void IcChangeMyst() {
            if (_playerStas.Strength <= 5 && _playerStas.FreePoints > 0) IncreaceMystic();
            else DecreaceMystic();
        }

        #endregion

        #region Update

        private void SetAllComponentsFalse(GameObject obj, GameObject objShadow) {
            for (int i = 0; i < obj.transform.childCount; i++) {
                Strength.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = false;
            }
            objShadow.SetActive(false);
        }

        private void UpdateStrengthUI() {
            if (Strength != null) {
                SetAllComponentsFalse(Strength, StrengthMainIconShadow);
                int amount = _playerStas.Strength;
                for (int i = 0; i < Strength.transform.childCount && amount > 0; i++, amount--) {
                    Strength.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
                }
                if (amount > 0) {
                    StrengthMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateIntUI() {
            if (Int != null) {
                SetAllComponentsFalse(Int, IntMainIconShadow);
                int amount = _playerStas.Int;
                for (int i = 0; i < Int.transform.childCount && amount > 0; i++, amount--) {
                    Int.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
                }
                if (amount > 0) {
                    IntMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateDexUI() {
            if (Dex != null) {
                SetAllComponentsFalse(Dex, DexMainIconShadow);
                int amount = _playerStas.Dex;
                for (int i = 0; i < Dex.transform.childCount && amount > 0; i++, amount--) {
                    Dex.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
                }
                if (amount > 0) {
                    DexMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdatePerceptUI() {
            if (Percept != null) {
                SetAllComponentsFalse(Percept, PerceptMainIconShadow);
                int amount = _playerStas.Percept;
                for (int i = 0; i < Percept.transform.childCount && amount > 0; i++, amount--) {
                    Percept.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
                }
                if (amount > 0) {
                    PerceptMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateMysticUI() {
            if (Mystic != null) {
                SetAllComponentsFalse(Mystic, MysticMainIconShadow);
                int amount = _playerStas.Mystic;
                for (int i = 0; i < Mystic.transform.childCount && amount > 0; i++, amount--) {
                    Mystic.transform.GetChild(i).gameObject.GetComponent<Image>().enabled = true;
                }
                if (amount > 0) {
                    MysticMainIconShadow.gameObject.SetActive(true);
                }
            }

        }

        private void UpdateStatsPoolUI() {
            if (StatsPool != null) {
                StatsPool.GetComponent<TMP_Text>().text = _playerStas.FreePoints.ToString();
            }
        }

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