using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EntryPoint;

public class EntryPoint : MonoBehaviour {
    #region Singleton
    private static EntryPoint _instance;
    public static EntryPoint Instance {
        get {
            if (_instance == null) {
                _instance = FindObjectOfType<EntryPoint>();
                if (_instance == null) {
                    GameObject singleton = new GameObject("EntryPoint");
                    _instance = singleton.AddComponent<EntryPoint>();
                    DontDestroyOnLoad(singleton);
                }
            }
            return _instance;
        }
    }

    private void Awake() {
        if (_instance != null && _instance != this) {
            Destroy(gameObject);
            return;
        }

        DialogueManager.Instance.OnMiniGameStartRequested += LaunchMinigame;

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    #region Minigame Data Structures
    [System.Serializable]
    public class MiniGameData {
        public DialogueGraph.MiniGameType minigameType;
        public int difficultyLevel;
        public Dictionary<string, object> customParameters;

        public MiniGameData() {
            difficultyLevel = 0;
            minigameType = DialogueGraph.MiniGameType.None;
            customParameters = new Dictionary<string, object>();
        }

        public MiniGameData(DialogueGraph.MiniGameType type, Dictionary<string, object> gameVariables) {
            minigameType = type;
            customParameters = gameVariables;
            difficultyLevel = 0;
        }

        public string ToJson() {
            return JsonUtility.ToJson(this);
        }

        public static MiniGameData FromJson(string json) {
            return JsonUtility.FromJson<MiniGameData>(json);
        }
    }
    #endregion

    #region Minigame Management

    public bool IsMinigameActive => MinigameManager.Instance != null && MinigameManager.Instance.transform.gameObject.activeSelf;

    public void LaunchMinigame(MiniGameData launchData) {
        if (IsMinigameActive) {
            Debug.LogWarning("Мини-игра уже запущена!");
            return;
        }

        // Создаем GameProcess
        GameObject managerObj = new GameObject("MinigameManager");
        MinigameManager minigameManager = managerObj.AddComponent<MinigameManager>();
        DontDestroyOnLoad(managerObj);

        Debug.Log("Создан новый MinigameManager");

        // Передаем данные в формате JSON
        string jsonData = launchData.ToJson();

        // Подписываемся на события GameProcess

        // Уведомляем подписчиков
        minigameManager.StartMinigame(launchData);

        Debug.Log($"Мини-игра запущена: {launchData.minigameType}");
    }

    #endregion
}