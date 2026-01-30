using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game
{
    public class ResourcesUI : MonoBehaviour
    {
        #region Inspector
        [Header("Text References")]
        [SerializeField] private TMP_Text goldText;
        [SerializeField] private TMP_Text foodText;
        [SerializeField] private TMP_Text satisfactionText;
        [SerializeField] private TMP_Text strengthText;

        [Header("Animation Settings")]
        [Tooltip("Time (seconds) to fade from white to green/red")]
        [SerializeField] private float colourFadeDuration = 0.25f;
        [Tooltip("Time (seconds) to fade back from green/red to white")]
        [SerializeField] private float colourReturnDuration = 0.4f;
        [Tooltip("Delay (seconds) between each +1 / -1 increment")]
        [SerializeField] private float valueStepDelay = 0.015f;
        [SerializeField] private Color increaseColour = Color.green;
        [SerializeField] private Color decreaseColour = Color.red;
        [SerializeField] private Color normalColour = Color.white;
        #endregion

        #region Private fields
        private int lastGold;
        private int lastFood;
        private int lastSatisfaction;
        private int lastStrength;

        // Stores the currently‑running coroutine for every TMP_Text so that it can be safely stopped when a new change arrives.
        private readonly Dictionary<TMP_Text, Coroutine> runningCoroutines = new();
        #endregion

        private void Awake() {
            Player.Resources.GoldChanged += OnGoldChanged;
            Player.Resources.FoodChanged += OnFoodChanged;
            Player.Resources.PeopleSatisfactionChanged += OnPeopleSatisfactionChanged;
            Player.Resources.CastleStrengthChanged += OnCastleStrengthChanged;
        }

        private void OnDestroy() {
            Player.Resources.GoldChanged -= OnGoldChanged;
            Player.Resources.FoodChanged -= OnFoodChanged;
            Player.Resources.PeopleSatisfactionChanged -= OnPeopleSatisfactionChanged;
            Player.Resources.CastleStrengthChanged -= OnCastleStrengthChanged;
        }

        #region Unity lifecycle
        private void Start() => UpdateAllResources(forceUpdate: true);
        #endregion

        #region Event handlers
        private void OnGoldChanged(int newGold) {
            UpdateResource(ref lastGold, newGold, goldText, true);
        }

        private void OnFoodChanged(int newFood) {
            UpdateResource(ref lastFood, newFood, foodText, true);
        }

        private void OnPeopleSatisfactionChanged(int newSatisfaction) {
            UpdateResource(ref lastSatisfaction, newSatisfaction, satisfactionText, true);
        }

        private void OnCastleStrengthChanged(int newStrength) {
            UpdateResource(ref lastStrength, newStrength, strengthText, true);
        }
        #endregion

        #region Updating helpers
        private void UpdateAllResources(bool forceUpdate = false)
        {
            UpdateResource(ref lastGold, Player.Resources.Gold, goldText, forceUpdate);
            UpdateResource(ref lastFood, Player.Resources.Food, foodText, forceUpdate);
            UpdateResource(ref lastSatisfaction, Player.Resources.PeopleSatisfaction, satisfactionText, forceUpdate);
            UpdateResource(ref lastStrength, Player.Resources.CastleStrength, strengthText, forceUpdate);
        }

        private void UpdateResource(ref int lastValue,
                                    int currentValue,
                                    TMP_Text textField,
                                    bool forceUpdate)
        {
            if (textField == null) return;

            // First frame or hard refresh – just set value without animation.
            if (forceUpdate)
            {
                textField.text = currentValue.ToString();
                textField.color = normalColour;
                lastValue = currentValue;
                return;
            }

            if (lastValue == currentValue) return;

            // A change detected – cancel previous animation (if any) and start a new one
            if (runningCoroutines.TryGetValue(textField, out var previous) && previous != null)
                StopCoroutine(previous);

            runningCoroutines[textField] = StartCoroutine(AnimateResourceChange(textField, lastValue, currentValue));
            lastValue = currentValue;
        }
        #endregion

        #region Animation coroutine
        private IEnumerator AnimateResourceChange(TMP_Text textField, int fromValue, int toValue)
        {
            int step = toValue > fromValue ? 1 : -1;
            Color targetTint = toValue > fromValue ? increaseColour : decreaseColour;

            // ▸ 1. Fade from white → green/red
            for (float t = 0f; t < colourFadeDuration; t += Time.unscaledDeltaTime)
            {
                textField.color = Color.Lerp(normalColour, targetTint, t / colourFadeDuration);
                yield return null;
            }
            textField.color = targetTint;

            // ▸ 2. Quickly tick value ±1 until we reach the target
            int shown = fromValue;
            while (shown != toValue)
            {
                shown += step;
                textField.text = shown.ToString();
                yield return new WaitForSecondsRealtime(valueStepDelay);
            }

            // ▸ 3. Fade back to white
            for (float t = 0f; t < colourReturnDuration; t += Time.unscaledDeltaTime)
            {
                textField.color = Color.Lerp(targetTint, normalColour, t / colourReturnDuration);
                yield return null;
            }
            textField.color = normalColour;

            // Mark coroutine as finished so future changes can start cleanly
            runningCoroutines[textField] = null;
        }
        #endregion

        #region Editor‑only validation
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (!goldText) //Debug.LogWarning("Gold Text not assigned!", this);
            if (!foodText) //Debug.LogWarning("Food Text not assigned!", this);
            if (!satisfactionText) //Debug.LogWarning("Satisfaction Text not assigned!", this);
            if (!strengthText) //Debug.LogWarning("Strength Text not assigned!", this);

            colourFadeDuration = Mathf.Max(0.01f, colourFadeDuration);
            colourReturnDuration = Mathf.Max(0.01f, colourReturnDuration);
            valueStepDelay = Mathf.Max(0.001f, valueStepDelay);
        }
#endif
        #endregion
    }
}
