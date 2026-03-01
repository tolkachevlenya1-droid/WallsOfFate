using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

namespace Game
{
    public class NPCPrefabFactory
    {
        private readonly List<GameObject> _prefabs;
        private readonly DiContainer _container; // Добавляем контейнер для создания
        private readonly Dictionary<string, GameObject> _instances = new();

        public NPCPrefabFactory(List<GameObject> prefabs, DiContainer container)
        {
            _prefabs = prefabs;
            _container = container;
        }

        public GameObject GetPrefab(string name)
        {
            return _prefabs.FirstOrDefault(p => p.name == name);
        }

        public GameObject Create(string name, Vector3 position, Quaternion rotation, Transform parent = null)
        {
            GameObject prefab = GetPrefab(name);
            if (prefab == null)
            {
                Debug.LogError($"Префаб {name} не найден!");
                return null;
            }

            GameObject instance = _container.InstantiatePrefab(prefab, position, rotation, parent);
            instance.name = $"{name}_{System.Guid.NewGuid().ToString().Substring(0, 4)}";

            _instances[name] = instance;

            //Debug.Log($"Создан NPC {instance.name} на позиции {position}");
            return instance;
        }

        public GameObject GetInstance(string name)
        {
            if (_instances.TryGetValue(name, out GameObject instance))
            {
                return instance;
            }

            Debug.LogError($"NPC {name} еще не создан или не существует!");
            return null;
        }

        // Проверка, существует ли NPC
        public bool HasInstance(string name)
        {
            return _instances.ContainsKey(name);
        }
    }
}