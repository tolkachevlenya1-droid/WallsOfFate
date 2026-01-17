using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game
{
    public class LocalizedAnimation : MonoBehaviour
    {
        [Header("Localization for load text")]
        [SerializeField] private List<string> keys;
        [SerializeField] private string localizationFileName = "localization";

        [Header("UI Components")]
        public TMP_Text loadingText;
        public Image loadingImage;
        public Sprite finalSprite;

        [Header("Animation Settings")]
        public float fadeFrequency = 4f;

        private UISpriteAnimator spriteAnimator;
        private Coroutine fadeCoroutine;
        private string currentLanguage;
        private string localisedLoading;
        private string localisedContionue;

        private void Awake()
        {
            currentLanguage = PlayerPrefs.GetString("CurrentLanguage", "en");

            spriteAnimator = loadingImage.GetComponent<UISpriteAnimator>();

            if (LoadingScreenManager.Instance != null)
            {
                LoadingScreenManager.Instance.LoadingStarted += OnLoadingStarted;
                LoadingScreenManager.Instance.LoadingFinished += OnLoadingFinished;
                LoadingScreenManager.Instance.WaitingForInputStarted += OnWaitingForInputStarted;
            }
        }

        private void OnDestroy()
        {
            if (LoadingScreenManager.Instance != null)
            {
                LoadingScreenManager.Instance.LoadingStarted -= OnLoadingStarted;
                LoadingScreenManager.Instance.LoadingFinished -= OnLoadingFinished;
                LoadingScreenManager.Instance.WaitingForInputStarted -= OnWaitingForInputStarted;
            }
        }

        private void OnLoadingStarted()
        {
            ShowLoadingAnimation();
        }

        private void OnLoadingFinished()
        {
            StopAllAnimations();
            ResetTextAppearance();
        }

        private void OnWaitingForInputStarted()
        {
            ShowWaitingForInputState();
        }

        private void ShowLoadingAnimation()
        {
            loadingText.text = localisedLoading;

            if (spriteAnimator != null)
            {
                spriteAnimator.enabled = true;
            }

            ResetTextAppearance();

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
        }

        private void ShowWaitingForInputState()
        {
            if (spriteAnimator != null)
            {
                spriteAnimator.enabled = false;
            }

            if (finalSprite != null)
            {
                loadingImage.sprite = finalSprite;
            }

            loadingText.text = localisedContionue;

            StartTextFade();
        }

        private void StopAllAnimations()
        {
            if (spriteAnimator != null)
            {
                spriteAnimator.enabled = false;
            }

            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
                fadeCoroutine = null;
            }
        }

        private void ResetTextAppearance()
        {
            var color = loadingText.color;
            loadingText.color = new Color(color.r, color.g, color.b, 1f);
        }

        private void StartTextFade()
        {
            if (fadeCoroutine != null)
                StopCoroutine(fadeCoroutine);

            fadeCoroutine = StartCoroutine(FadeText());
        }

        private IEnumerator FadeText()
        {
            Color baseColor = loadingText.color;

            while (true)
            {
                float alpha = (Mathf.Sin(Time.time * fadeFrequency) + 1f) / 2f;
                loadingText.color = new Color(baseColor.r, baseColor.g, baseColor.b, alpha);
                yield return null;
            }
        }
    }
}