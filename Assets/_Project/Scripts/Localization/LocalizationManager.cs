using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game
{
    // Use only via Zenject DI
    public class LocalizationManager
    {
        private class LocalizationItem
        {
            public string key;
            public string value;
        }

        private readonly string localizationRoot = "Localization/";        
        private readonly Dictionary<string, Dictionary<string, string>> localizationDictionary = new();

        private Dictionary<string, List<Dictionary<string, string>>> localizationConfig;

        private string currentLanguage;

        public event Action OnLanguageChanged;

        public LocalizationManager()
        {
            currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "en");

            LoadLocalization();
        }

        public Dictionary<string, List<Dictionary<string, string>>> LocalizationConfig => localizationConfig;

        public string CurrentLanguage
        {
            get => currentLanguage;
            set
            {
                currentLanguage = value;
                PlayerPrefs.SetString("CurrentLanguage", value);
                PlayerPrefs.Save();
                OnLanguageChanged.Invoke();
            }
        }

        public Dictionary<string, string> CurrentLocalization => localizationDictionary[currentLanguage];

        public string GetLocalizedText(string path)
        {
            if (CurrentLocalization.ContainsKey(path)) 
            {
                return CurrentLocalization[path];
            }

            var segments = path.Split('/');
            var localizationPath = string.Join("/", segments.Take(segments.Length - 1));
            var fullPath = localizationRoot + currentLanguage + "/" + localizationPath;

            var items = ParseLocalisationFile(fullPath);
            foreach (var item in items)
            {
                if (!CurrentLocalization.ContainsKey(item.key))
                {
                    CurrentLocalization.Add(localizationPath + "/" + item.key, item.value);
                }
            }

            if (CurrentLocalization.ContainsKey(path))
            {
                return CurrentLocalization[path];
            } 
            else
            {
                throw new Exception($"Localization key not found: {path}");
            }
        }

        private void LoadLocalization()
        {
            var languages = Resources.Load<TextAsset>(localizationRoot + "languages");
            if (languages == null)
            {
                throw new Exception("Languages file not found.");
            }
            localizationConfig = JsonConvert.DeserializeObject< Dictionary<string, List<Dictionary<string, string>>>>(languages.text);
            foreach (var lang in localizationConfig.Keys)
            {
                localizationDictionary.TryAdd(lang, new Dictionary<string, string>());                
            }
        }

        private List<LocalizationItem> ParseLocalisationFile(string path)
        {
            var jsonFile = Resources.Load<TextAsset>(path);

            if (jsonFile == null)
            {
                throw new Exception($"Localization file not found: {path}");
            }

            return JsonConvert.DeserializeObject<List<LocalizationItem>>(jsonFile.text);            
        }
    }
}

