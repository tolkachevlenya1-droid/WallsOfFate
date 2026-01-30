using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Game.EntryPoint;

namespace Game
{
    public class PowerCheckInstaller : MonoBehaviour
    {
        [Header("Player Settings")]
        public Transform StartPoint;
        public GameObject PlayerPrefab;
        public Transform Parent;
        public Transform CameraTransform;

        [Header("HP Bar Settings")]
        public Transform CanvasTransform;
        public Slider PlayerHPBarPrefab;
        public Slider EnemyHPBarPrefab;

        [Header("Enemy Settings")]
        public Transform SpawnPoint;
        public GameObject EnemyPrefab;

        [Header("Game Settings")]
        public MineSpawner MineSpawnerObject;
        public GameProcess GameProcessObject;
        public EndDayScreenManager EndGameScreenObject;

        [Header("References (Auto-filled)")]
        public PlayerMove PlayerInstance;
        public AIController EnemyInstance;

        private MiniGameData _gameData;

        void Start()
        {
            // Ждем, пока MinigameManager инициализирует данные
            //Invoke(nameof(Initialize), 0.2f);
        }

        public void InitializeWithData(MiniGameData gameData)
        {
            _gameData = gameData;
            Debug.Log($"Инициализация мини-игры: {gameData.minigameType}");
            Initialize();
        }

        private void Initialize()
        {
            // Если данных нет, пробуем получить их из MinigameManager
            if (_gameData == null && MinigameManager.Instance != null)
            {
                _gameData = MinigameManager.Instance.CurrentGameData;
            }

            // Применяем настройки сложности
            if (_gameData != null && _gameData.customParameters.ContainsKey("difficulty"))
            {
                ApplyDifficulty((int)_gameData.customParameters["difficulty"]);
            }

            // Инициализируем игру
            InitializeCameraTransform();
            BindMineSpawner();
            BindGameProcess();
            CreatePlayer();
            CreateEnemy();
            SetupForbiddenSpawnPoints();

            if (MineSpawnerObject != null)
            {
                MineSpawnerObject.Initialize();
            }

            if (GameProcessObject != null)
            {
                GameProcessObject.Initialize();
            }
            if (EndGameScreenObject != null)
                EndGameScreenObject.OnEndGame += OnMinigameEnded;
        }

        private void ApplyDifficulty(int difficulty)
        {
            Debug.Log($"Установлена сложность: {difficulty}");
            // Здесь можно настроить параметры игры в зависимости от сложности
        }

        private void BindMineSpawner()
        {
            if (MineSpawnerObject == null)
            {
                MineSpawnerObject = FindObjectOfType<MineSpawner>();
            }
        }

        private void BindGameProcess()
        {
            if (GameProcessObject == null)
            {
                GameProcessObject = FindObjectOfType<GameProcess>();
            }
        }

        private void CreatePlayer()
        {
            if (PlayerPrefab == null || StartPoint == null) return;

            GameObject playerObj = Instantiate(PlayerPrefab, StartPoint.position,
                PlayerPrefab.transform.rotation, Parent);

            PlayerInstance = playerObj.GetComponent<PlayerMove>();

            // Создаем HP бар для игрока
            if (PlayerHPBarPrefab != null && CanvasTransform != null)
            {
                //Slider playerHealthBar = Instantiate(PlayerHPBarPrefab, CanvasTransform);
                HealthBarManager healthBarManager = playerObj.GetComponent<HealthBarManager>();
                if (healthBarManager != null)
                {
                    healthBarManager.SetHealthBar(PlayerHPBarPrefab);
                }
            }
        }

        private void CreateEnemy()
        {
            if (EnemyPrefab == null || SpawnPoint == null) return;

            string pathToEnemyPrefab = (string)_gameData.customParameters["Prefab"];
            if (!string.IsNullOrEmpty(pathToEnemyPrefab))
                EnemyPrefab = Resources.Load<GameObject>(pathToEnemyPrefab);

            GameObject enemyObj = Instantiate(EnemyPrefab, SpawnPoint.position, SpawnPoint.rotation, Parent);

            enemyObj.name = enemyObj.GetComponent<MiniGamePlayer>().GetName();
            EnemyInstance = enemyObj.GetComponent<AIController>();

            // Создаем HP бар для врага
            if (EnemyHPBarPrefab != null && CanvasTransform != null)
            {
                //Slider enemyHealthBar = Instantiate(EnemyHPBarPrefab, CanvasTransform);
                HealthBarManager healthBarManager = enemyObj.GetComponent<HealthBarManager>();
                if (healthBarManager != null)
                {
                    healthBarManager.SetHealthBar(EnemyHPBarPrefab);
                }
            }
        }

        private void InitializeCameraTransform()
        {
            if (CameraTransform == null)
            {
                Camera mainCamera = Camera.main;
                if (mainCamera != null)
                {
                    CameraTransform = mainCamera.transform;
                }
            }
        }

        private void SetupForbiddenSpawnPoints()
        {
            if (MineSpawnerObject != null && PlayerInstance != null && EnemyInstance != null)
            {
                var forbiddenPoints = new List<Transform>
            {
                PlayerInstance.transform,
                EnemyInstance.transform
            };

                MineSpawnerObject.SetForbiddenSpawnPoints(forbiddenPoints);
            }
        }

        private void OnMinigameEnded(bool playerWin)
        {
            Debug.Log($"Мини-игра завершена! Победил ли игрок? {playerWin}");
            if (MinigameManager.Instance != null) MinigameManager.Instance.EndMinigame(playerWin);
        }

        void OnDestroy()
        {
            if (EndGameScreenObject != null)
            {
                EndGameScreenObject.OnEndGame -= OnMinigameEnded;
            }
        }
    }
}
