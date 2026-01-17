using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Newtonsoft.Json;
using System;

namespace Game.UI
{
    public class SettingsMenuController : MonoBehaviour
    {
        [SerializeField] private Slider sliderMusic;
        [SerializeField] private Slider sliderSFX;
        [SerializeField] private Slider sliderUI;
        [SerializeField] private TMPro.TMP_Dropdown languageDropdown;

        private string currentLanguage;
        private Dictionary<string, List<Dictionary<string, string>>> languageConfig;
        private bool isLoading = false;

        public event Action<string> OnLanguageChanged;

        public string CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                if (currentLanguage != value)
                {
                    currentLanguage = value;
                    OnLanguageChanged?.Invoke(currentLanguage);
                    InitializeLanguageDropdown();
                }
            }
        }

        private void Start()
        {
            sliderMusic.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
            sliderSFX.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
            sliderUI.value = PlayerPrefs.GetFloat("UIVolume", 1f);

            LoadLanguageConfig();
            InitializeLanguageDropdown();

            ApplySettings();
        }

        void Update()
        {
            if (gameObject.activeSelf && Input.GetKeyDown(KeyCode.Escape))
            {
                HideSettingsMenu();
            }
        }

        private void LoadLanguageConfig()
        {
            TextAsset configFile = Resources.Load<TextAsset>("Localization/languages");
            if (configFile != null)
            {
                languageConfig = JsonConvert.DeserializeObject<Dictionary<string, List<Dictionary<string, string>>>>(configFile.text);
            }
            else
            {
                Debug.LogError("Language config file not found!");
            }

            currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "en");
        }

        private void InitializeLanguageDropdown()
        {
            if (languageDropdown == null || languageConfig == null || !languageConfig.ContainsKey(currentLanguage)) return;

            isLoading = true;
            languageDropdown.ClearOptions();

            List<TMPro.TMP_Dropdown.OptionData> options = new();

            List<Dictionary<string, string>> currentLanguageList = languageConfig[currentLanguage];

            foreach (var dict in currentLanguageList)
            {
                foreach (var languageName in dict.Keys)
                {
                    options.Add(new TMPro.TMP_Dropdown.OptionData(languageName));
                }
            }

            languageDropdown.AddOptions(options);

            int selectedIndex = FindCurrentLanguageIndex(currentLanguageList);
            languageDropdown.value = selectedIndex;

            isLoading = false;

            languageDropdown.onValueChanged.RemoveAllListeners();
            languageDropdown.onValueChanged.AddListener(OnLanguageSelected);
        }

        private int FindCurrentLanguageIndex(List<Dictionary<string, string>> languageList)
        {
            for (int i = 0; i < languageList.Count; i++)
            {
                foreach (var kvp in languageList[i])
                {
                    if (kvp.Value == currentLanguage)
                    {
                        return i;
                    }
                }
            }
            return 0;
        }

        private void OnLanguageSelected(int index)
        {
            if (isLoading) return;

            if (languageConfig != null && languageConfig.ContainsKey(currentLanguage))
            {
                List<Dictionary<string, string>> currentLanguageList = languageConfig[currentLanguage];

                if (index < currentLanguageList.Count)
                {
                    var selectedDict = currentLanguageList[index];

                    foreach (var kvp in selectedDict)
                    {
                        CurrentLanguage = kvp.Value;
                        break;
                    }                    

                    PlayerPrefs.SetString("CurrentLanguage", CurrentLanguage);
                    PlayerPrefs.Save();

                    Debug.Log($"Language changed to: {CurrentLanguage}");
                }
            }
        }

        public void ApplySettings()
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.SetVolume("Volume_Music", sliderMusic.value);
                AudioManager.Instance.SetVolume("Volume_SFX", sliderSFX.value);
                AudioManager.Instance.SetVolume("Volume_UI", sliderUI.value);
            }

            PlayerPrefs.SetFloat("MusicVolume", sliderMusic.value);
            PlayerPrefs.SetFloat("SFXVolume", sliderSFX.value);
            PlayerPrefs.SetFloat("UIVolume", sliderUI.value);
            PlayerPrefs.Save();
        }

        private void OnDestroy()
        {
            OnLanguageChanged = null;
        }

        public void HideSettingsMenu()
        {
            gameObject.SetActive(false);
        }
    }
}