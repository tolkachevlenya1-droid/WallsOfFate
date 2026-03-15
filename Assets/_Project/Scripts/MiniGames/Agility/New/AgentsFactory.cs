using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using Zenject;

namespace Game.MiniGame.Agility
{
    public class AgentsFactory
    {
        private readonly DiContainer container;

        public GameObject Player {  get; private set; }
        public GameObject Enemy { get; private set; }


        public AgentsFactory(DiContainer container)
        {
            this.container = container;
        }

        public GameObject GetPlayer()
        {
            return this.Player;
        }
        public GameObject GetEnemy()
        {
            return this.Enemy;
        }

        public GameObject CreateAgent(GameObject agent, Vector3 position, Quaternion rotation, bool isPlayer = true, Transform parent = null)
        {
            if (agent == null)
            {
                Debug.LogError($"Префаб {agent} не найден!");
                return null;
            }

            GameObject instance = container.InstantiatePrefab(agent, position, rotation, parent);
            
            if (isPlayer)
            {
                this.Player = instance;
            }
            else this.Enemy = instance;

            return instance;
        }
    }
}
