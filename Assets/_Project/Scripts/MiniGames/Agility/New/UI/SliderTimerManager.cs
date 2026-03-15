using UnityEngine;
using UnityEngine.UI;
using System;

namespace Game.MiniGame.Agility
{
    public class SliderTimerManager : MonoBehaviour
    {
        [SerializeField] private Slider timerSlider;
        private float gameDuration;

        public void InitializeTimer(float duration)
        {
            gameDuration = duration;

            if (timerSlider != null)
            {
                timerSlider.maxValue = duration;
                timerSlider.value = duration;
            }
        }

        public void UpdateTimer(float currentTime)
        {
            if (timerSlider != null)
                timerSlider.value = currentTime;
        }

        public void ResetTimer()
        {
            if (timerSlider != null)
                timerSlider.value = gameDuration;
        }
    }
}