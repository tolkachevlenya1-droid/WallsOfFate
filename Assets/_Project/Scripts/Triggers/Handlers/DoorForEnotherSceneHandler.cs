using Game.Data;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace Game
{

    internal class DoorForEnotherSceneHandler : MonoBehaviour, ITriggerHandler
    {
        /*{
            "sceneName": "Level2",
            "spawnPosition": {"x": 10.5, "y": 0, "z": 20.3},
            "spawnEulerAngles": {"x": 0, "y": 90, "z": 0},
            "dayNumber": 2
            }
         */

        [Serializable]
        public class DoorParameters
        {
            public string SceneName;
            public Vector3 SpawnPosition;
            public Vector3 SpawnEulerAngles;
            public int DayNumber;
        }

        [SerializeField] private string defaultSceneName = "";
        [SerializeField] private Vector3 defaultSpawnPosition = Vector3.zero;
        [SerializeField] private Vector3 defaultSpawnEulerAngles = Vector3.zero;
        [SerializeField] private int defaultDayNumber = -1;

        [SerializeField] private List<InfluenceArea> influenceArias;
        private LoadingManager loadingManager;
        private GameflowManager gameflowManager;

        [Inject]
        public void Construct(LoadingManager loadingManager, GameflowManager gameflowManager)
        {
            this.loadingManager = loadingManager;
            this.gameflowManager = gameflowManager;
        }

        private void OnEnable()
        {
            foreach (var area in influenceArias)
            {
                area.OnEventTriggered += Handle;
            }
        }

        private void OnDisable()
        {
            foreach (var area in influenceArias)
            {
                area.OnEventTriggered -= Handle;
            }
        }

        public void Handle(TriggerEvent eventData)
        {
            DoorParameters parameters = ParseParameters(eventData.Parameters);

            if (ShouldTrigger(parameters.DayNumber))
            {
                PlayerSpawnData.SpawnPosition = parameters.SpawnPosition;
                PlayerSpawnData.SpawnRotation = Quaternion.Euler(parameters.SpawnEulerAngles);

                loadingManager.LoadScene(parameters.SceneName);

                Debug.Log($"Переход на сцену: {parameters.SceneName}");
            }
        }

        private DoorParameters ParseParameters(string jsonParameters)
        {
            if (string.IsNullOrEmpty(jsonParameters))
            {
                return new DoorParameters
                {
                    SceneName = defaultSceneName,
                    SpawnPosition = defaultSpawnPosition,
                    SpawnEulerAngles = defaultSpawnEulerAngles,
                    DayNumber = defaultDayNumber
                };
            }

            try
            {
                return JsonUtility.FromJson<DoorParameters>(jsonParameters);
            }
            catch (Exception e)
            {
                Debug.LogError($"Ошибка парсинга JSON для двери: {e.Message}\nJSON: {jsonParameters}");

                return new DoorParameters
                {
                    SceneName = defaultSceneName,
                    SpawnPosition = defaultSpawnPosition,
                    SpawnEulerAngles = defaultSpawnEulerAngles,
                    DayNumber = defaultDayNumber
                };
            }
        }

        private bool ShouldTrigger(int targetDayNumber)
        {
            if (targetDayNumber == -1) return true;
            return targetDayNumber == this.gameflowManager.CurrentDay.Id;
        }
    }
}
