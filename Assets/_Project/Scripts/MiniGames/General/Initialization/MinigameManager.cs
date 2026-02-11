using Game.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static Game.EntryPoint;

public enum MiniGameType {
    None = 0,
    PowerCheck = 1
}

namespace Game
{

    public class MinigameManager : MonoBehaviour
    {
        public static MinigameManager Instance { get; private set; }

        [Header("Minigame Scenes")]
        [SerializeField] private string powerCheckScene = "PowerCheck";
        [SerializeField] private string castleDefenseScene = "CastleDefenseMinigame";
        // Добавьте другие сцены по необходимости

        private MiniGameData _currentGameData;
        private Dictionary<string, object> _lastResult;
        private string _previousScene;

        public MiniGameData CurrentGameData => _currentGameData;

        private LoadingManager loadingManager;
        private PlayerManager playerManager;

        [Inject]
        private void Construct(LoadingManager loadingManager, PlayerManager playerManager)
        {
            this.loadingManager = loadingManager;
            this.playerManager = playerManager;
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        public void StartMinigame(MiniGameData gameData)
        {
            _currentGameData = gameData;
            _currentGameData = gameData;
            _previousScene = SceneManager.GetActiveScene().name;

            string sceneToLoad = GetSceneForMinigameType(gameData.minigameType);

            if (!string.IsNullOrEmpty(sceneToLoad))
            {
                this.loadingManager.LoadSceneAsync(sceneToLoad);
            }
            else
            {
                Debug.LogError($"Не определена сцена для мини-игры типа: {gameData.minigameType}");
            }
        }

        private string GetSceneForMinigameType(MiniGameType type)
        {
            switch (type)
            {
                case MiniGameType.PowerCheck:
                    return powerCheckScene;
                default:
                    Debug.LogWarning($"Тип мини-игры {type} не настроен. Использую сцену по умолчанию.");
                    return powerCheckScene;
            }
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == powerCheckScene || scene.name == castleDefenseScene)
            {
                Invoke(nameof(InitializeMinigame), 0.1f);
            }
        }

        private void InitializeMinigame()
        {
            PowerCheckInstaller installer = FindFirstObjectByType<PowerCheckInstaller>();
            if (installer != null)
            {
                installer.InitializeWithData(_currentGameData);
            }
            else
            {
                Debug.LogError("MiniGameInstaller не найден на сцене мини-игры!");
            }
        }

        public void EndMinigame(bool playerWon)
        {

            string outcomeKey = playerWon ? "Win" : "Lose";

            try
            {
                if (_currentGameData.customParameters.TryGetValue(outcomeKey, out object outcomeObj) &&
                    outcomeObj is Newtonsoft.Json.Linq.JObject outcomeJObject)
                {
                    if (outcomeJObject.TryGetValue("Resources", out Newtonsoft.Json.Linq.JToken resourcesToken) &&
                        resourcesToken is Newtonsoft.Json.Linq.JObject resourcesJObject)
                    {
                        var resourcesDict = resourcesJObject.ToObject<Dictionary<string, string>>();

                        if (resourcesDict != null)
                        {
                            // Обрабатываем ресурсы;
                            ProcessResources(resourcesDict);
                        }
                    }
                }
                else
                {
                    Debug.Log($"No '{outcomeKey}' configuration found in minigame data");
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Error processing minigame result: {e.Message}");
            }

            // Возвращаемся в предыдущую сцену
            this.loadingManager.LoadSceneAsync(_previousScene);
            //SceneManager.LoadScene(_previousScene);
            Destroy(this.gameObject);
        }

        private void ProcessResources(Dictionary<string, string> resourcesDict)
        {
            // Проверяем и обрабатываем золото
            if (!string.IsNullOrEmpty(resourcesDict["Gold"]))
                if (int.TryParse(resourcesDict["Gold"], out int goldChange))
                    playerManager.PlayerData.AddResource(ResourceType.Gold, goldChange);

            if (!string.IsNullOrEmpty(resourcesDict["Food"]))
                if (int.TryParse(resourcesDict["Food"], out int goldChange))
                    playerManager.PlayerData.AddResource(ResourceType.Food, goldChange);

            if (!string.IsNullOrEmpty(resourcesDict["PeopleSatisfaction"]))
                if (int.TryParse(resourcesDict["PeopleSatisfaction"], out int goldChange))
                    playerManager.PlayerData.AddResource(ResourceType.PeopleSatisfaction, goldChange);

            if (!string.IsNullOrEmpty(resourcesDict["CastleStrength"]))
                if (int.TryParse(resourcesDict["CastleStrength"], out int goldChange))
                    playerManager.PlayerData.AddResource(ResourceType.CastleStrength, goldChange);
        }
    }
}
