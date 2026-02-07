using Game.Data;
using Newtonsoft.Json;
using UnityEngine;
using Zenject;

namespace Player {
    public class StatsSaveLoader {

        private Player.Stats _playerStas;

        [Inject]
        private void Construct(Player.Stats playerStats) {
            _playerStas = playerStats;
        }
        public bool LoadData() {
            if (Repository.TryGetData("GameResources", out ResourceData data)) {
                _playerStas.Strength = data.Strength;
                _playerStas.Dex = data.Dex;
                _playerStas.Percept = data.Percept;
                _playerStas.Mystic = data.Mystic;
                //Debug.Log("Loaded resources data");
                return true;
            }
            return false;
        }

        public void LoadDefaultData() {
            TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/PlayerStats/DefaultPlayerStats");
            if (textAsset == null) {
                //Debug.LogError("Default resources file not found!");
                return;
            }

            try {
                var settings = new JsonSerializerSettings {
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                    NullValueHandling = NullValueHandling.Ignore,
                    MissingMemberHandling = MissingMemberHandling.Error
                };

                var defaultData = JsonConvert.DeserializeObject<ResourceData>(textAsset.text, settings);
                _playerStas.Strength = defaultData.Strength;
                _playerStas.Dex = defaultData.Dex;
                _playerStas.Percept = defaultData.Percept;
                _playerStas.Mystic = defaultData.Mystic;
            }
            catch (JsonException ex) {
                Debug.LogError($"JSON error: {ex.Message}");
            }
        }

        public void SaveData() {
            var data = new ResourceData {
                Strength = _playerStas.Strength,
                Dex = _playerStas.Dex,
                Percept = _playerStas.Percept,
                Mystic = _playerStas.Mystic
            };
            Repository.SetData("GameResources", data);
            //Debug.Log("Saved resources data");
        }
    }

    [System.Serializable]
    public class ResourceData {
        public int Strength;
        public int Dex;
        public int Percept;
        public int Mystic;
    }
}