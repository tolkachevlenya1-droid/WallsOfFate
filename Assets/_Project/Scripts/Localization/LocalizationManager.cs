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

        private string currentLanguage;

        public LocalizationManager()
        {
            currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "en");

            LoadLocalization();
        }

        public void SetCurrentLanguage(string language)
        { 
            currentLanguage = language;
            PlayerPrefs.SetString("CurrentLanguage", language);
        }

        public string GetCurrentLanguage()
        {
            return currentLanguage;
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
                    CurrentLocalization.Add(item.key, item.value);
                }
            }

            if (CurrentLocalization.ContainsKey(path))
            {
                return CurrentLocalization[path];
            } else
            {
                throw new Exception($"Localization key not found: {path}");
            }
        }

        public class LocalizedText
        {
            public string key;
            public TMPro.TMP_Text textField;
        }

        public List<LocalizedText> localizedTexts = new();

        private void LoadLocalization()
        {
            var languages = Resources.Load<TextAsset>(localizationRoot + "languages");
            if (languages == null)
            {
                throw new Exception("Languages file not found.");
            }
            var languageList = JsonConvert.DeserializeObject<Dictionary<string, object>>(languages.text);
            foreach (var lang in languageList.Keys)
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

