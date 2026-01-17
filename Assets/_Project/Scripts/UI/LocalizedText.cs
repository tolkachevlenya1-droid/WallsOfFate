using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace Game.UI
{
    public class LocalizedText : MonoBehaviour
    {
        public string key;

        private TMPro.TMP_Text textField;

        private LocalizationManager lm;

        [Inject]
        public void Init(LocalizationManager lm)
        {
            this.lm = lm;
        }

        public void Start()
        {
            Localize();
        }

        public void Localize() 
        {
            textField = GetComponent<TMPro.TMP_Text>();
            if (textField == null)
            {
                Debug.LogError("LocalizedText component requires a TMPro.TMP_Text component on the same GameObject.");
                return;
            }
            try
            {
                string localizedString = lm.GetLocalizedText(key);
                textField.text = localizedString;
            }
            catch (Exception e)
            {
                Debug.LogError($"Error localizing text for key '{key}': {e.Message}");
            }
        }
    }
}
