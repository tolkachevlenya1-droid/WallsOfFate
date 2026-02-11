using UnityEngine;
using UnityEngine.UI;
using Zenject.SpaceFighter;

namespace Game
{
    public class HealthBarManager : MonoBehaviour
    {
        private Slider _healthBar;
        private MiniGamePlayer _player;

        /// <summary>
        /// Назначает полоску здоровья (вызывается из MiniGameInstaller).
        /// </summary>
        /// <param name="healthBar">Slider для отображения здоровья.</param>
        public void SetHealthBar(Slider healthBar)
        {
            _healthBar = healthBar;
        }

        void Start()
        {
            // Активируем полоску здоровья, если объект активен и _healthBar назначен
            if (this.gameObject.activeSelf && _healthBar != null)
            {
                _healthBar.gameObject.SetActive(true);
            }

            // Получаем компонент MiniGamePlayer
            _player = GetComponent<MiniGamePlayer>();
            if (_player == null)
            {
                Debug.LogError("Компонент MiniGamePlayer не найден!", this);
                return;
            }

            // Проверяем, назначена ли полоска здоровья
            if (_healthBar == null)
            {
                Debug.LogError("Полоска здоровья не назначена!", this);
                return;
            }

            // Обновляем здоровье и портрет при старте
            UpdateHealthBar();
            UpdatePortrait();
        }

        void Update()
        {
            // Обновляем здоровье, если player и _healthBar не null
            if (_player != null && _healthBar != null)
            {
                UpdateHealthBar();
            }
        }

        /// <summary>
        /// Обновляет значение полоски здоровья и текст.
        /// </summary>
        private void UpdateHealthBar()
        {
            // Вычисляем отношение текущего здоровья к максимальному
            float currentHealth = _player.Health;
            float maxHealth = _player.MaxHealth;
            _healthBar.value = currentHealth / maxHealth;

            // Обновляем текст здоровья (например, "50 / 100")
            Text healthBarText = _healthBar.GetComponentInChildren<Text>();
            if (healthBarText != null)
            {
                healthBarText.text = $"{Mathf.Ceil(currentHealth)} / {Mathf.Ceil(maxHealth)}";
            }
        }

        /// <summary>
        /// Проверяет и обновляет спрайт портрета в дочернем Image с именем "image".
        /// </summary>
        private void UpdatePortrait()
        {
            // Проверяем, что player и _healthBar не null
            if (_player == null || _healthBar == null)
            {
                return;
            }

            // Ищем дочерний объект с именем "image" в иерархии _healthBar.transform
            Transform imageTransform = _healthBar.transform.Find("Image");
            if (imageTransform == null)
            {
                Debug.LogWarning("Объект с именем 'image' не найден под полоской здоровья!", _healthBar);
                return;
            }

            // Получаем компонент Image у найденного объекта
            Image portraitImage = imageTransform.GetComponent<Image>();
            if (portraitImage == null)
            {
                Debug.LogWarning("Компонент Image не найден на объекте 'image' под полоской здоровья!", imageTransform);
                return;
            }

            // Проверяем, указан ли путь к портрету
            if (string.IsNullOrEmpty(_player.Portrait))
            {
                Debug.LogWarning("player.Portrait пуст или null!", this);
                return;
            }

            // Загружаем спрайт из Resources/PowerCheckPortraits
            Sprite portraitSprite = Resources.Load<Sprite>("MiniGames/PowerCheck/PowerCheckPortraits/" + _player.Portrait);
            if (portraitSprite == null)
            {
                Debug.LogWarning($"Не удалось загрузить спрайт по пути 'PowerCheckPortraits/{_player.Portrait}'!", this);
                return;
            }

            // Обновляем спрайт, если текущий отличается
            if (portraitImage.sprite != portraitSprite)
            {
                portraitImage.sprite = portraitSprite;
            }
        }
    }
}
