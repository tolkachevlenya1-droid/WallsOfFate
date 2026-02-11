using Game;
using Game.Data;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

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

        [SerializeField] private Sprite statOnIcon;
        [SerializeField] private Sprite statOfIcon;

        private PlayerManager playerManager;

        [Inject]
        private void Construct(PlayerManager playerManager) {
            this.playerManager = playerManager;
        }

        #region ButtonMethods

        public void ChangeStrength(Image buttImage) {
            if (buttImage.enabled == false) IncreaseStat(StatType.Strength);
            else DecreaseStat(StatType.Strength);
        }

        public void ChangeInt(Image buttImage) {
            if (buttImage.enabled == false) IncreaseStat(StatType.Int);
            else DecreaseStat(StatType.Int);
        }

        public void ChangeDex(Image buttImage) {
            if (buttImage.enabled == false) IncreaseStat(StatType.Dex);
            else DecreaseStat(StatType.Dex);
        }

        public void ChangePercept(Image buttImage) {
            if (buttImage.enabled == false) IncreaseStat(StatType.Percept);
            else DecreaseStat(StatType.Percept);
        }

        public void ChangeMyst(Image buttImage) {
            if (buttImage.enabled == false) IncreaseStat(StatType.Mystic);
            else DecreaseStat(StatType.Mystic);
        }

        #endregion

        #region Update

        private void SetAllComponentsFalse(GameObject obj, GameObject objShadow) {
            for (int i = 0; i < obj.transform.childCount; i++) {
                Strength.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOfIcon;
            }
            objShadow.SetActive(false);
        }

        private void UpdateStrengthUI() {
            if (Strength != null) {
                SetAllComponentsFalse(Strength, StrengthMainIconShadow);
                int amount = playerManager.PlayerData.GetStat(StatType.Strength);
                for (int i = 0; i < Strength.transform.childCount && amount > 0; i++, amount--) {
                    Strength.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
                if (amount > 0) {
                    StrengthMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateIntUI() {
            if (Int != null) {
                SetAllComponentsFalse(Int, IntMainIconShadow);
                int amount = playerManager.PlayerData.GetStat(StatType.Int);
                for (int i = 0; i < Int.transform.childCount && amount > 0; i++, amount--) {
                    Int.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
                if (amount > 0) {
                    IntMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateDexUI() {
            if (Dex != null) {
                SetAllComponentsFalse(Dex, DexMainIconShadow);
                int amount = playerManager.PlayerData.GetStat(StatType.Dex);
                for (int i = 0; i < Dex.transform.childCount && amount > 0; i++, amount--) {
                    Dex.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
                if (amount > 0) {
                    DexMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdatePerceptUI() {
            if (Percept != null) {
                SetAllComponentsFalse(Percept, PerceptMainIconShadow);
                int amount = playerManager.PlayerData.GetStat(StatType.Percept);
                for (int i = 0; i < Percept.transform.childCount && amount > 0; i++, amount--) {
                    Percept.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
                if (amount > 0) {
                    PerceptMainIconShadow.gameObject.SetActive(true);
                }
            }
        }

        private void UpdateMysticUI() {
            if (Mystic != null) {
                SetAllComponentsFalse(Mystic, MysticMainIconShadow);
                int amount = playerManager.PlayerData.GetStat(StatType.Mystic);
                for (int i = 0; i < Mystic.transform.childCount && amount > 0; i++, amount--) {
                    Mystic.transform.GetChild(i).gameObject.GetComponent<Image>().sprite = statOnIcon;
                }
                if (amount > 0) {
                    MysticMainIconShadow.gameObject.SetActive(true);
                }
            }

        }

        private void UpdateStatsPoolUI() {
            if (StatsPool != null) {
                StatsPool.GetComponent<TMP_Text>().text = playerManager.PlayerData.FreePoints.ToString();
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

        private void IncreaseStat(StatType stat)
        {
            playerManager.IncreaseStat(stat);

            UpdateAllStatsUI();
        }

        private void DecreaseStat(StatType stat)
        {
            playerManager.DecreaseStat(stat);

            UpdateAllStatsUI();
        }

        #endregion
    }
}