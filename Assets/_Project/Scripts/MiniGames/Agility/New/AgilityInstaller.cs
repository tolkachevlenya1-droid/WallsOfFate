using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace Game.MiniGame.Agility
{
    public class AgilityInstaller : MonoInstaller, IMiniGameInstaller
    {
        [Header("Player Settings")]
        public Transform PlayerStartPoint;
        public GameObject PlayerPrefab;
        public Transform Parent;

        [Header("Enemy Settings")]
        public Transform EnemyStartPoint;

        [Header("Scene References")]
        public GameProcess GameProcessObject;

        private MiniGameData _gameData;

        public override void InstallBindings()
        {
            Container.Bind<AgentsFactory>()
               .AsSingle();
        }

        public void InitializeWithData(MiniGameData gameData)
        {
            this._gameData = gameData;

            CreateAgents();

            if (_gameData != null)
            {
                Container.BindInstance(_gameData).AsSingle();
            }

            InitializeGameLogic();
        }

        private void CreateAgents()
        {
            var factory = Container.Resolve<AgentsFactory>();

            factory.CreateAgent(PlayerPrefab, PlayerStartPoint.position, PlayerStartPoint.rotation, true, Parent);

            GameObject enemyPrefab = ResolveEnemyPrefab();
            factory.CreateAgent(enemyPrefab, EnemyStartPoint.position, EnemyStartPoint.rotation, false, Parent);
        }

        private GameObject ResolveEnemyPrefab()
        {
            if (_gameData != null && _gameData.customParameters.ContainsKey("EnemyPrefab"))
            {
                return (GameObject)_gameData.customParameters["EnemyPrefab"];
            }
            Debug.LogError("EnemyPrefab не найден в параметрах!");
            return null;
        }

        private void InitializeGameLogic()
        {
            Container.Inject(GameProcessObject);

            GameProcessObject.Initialize();
        }

        public void OnMiniGameEnded(bool playerWin)
        {
            if (MinigameManager.Instance != null)
            {
                MinigameManager.Instance.EndMinigame(playerWin);
            }
        }
    }
}