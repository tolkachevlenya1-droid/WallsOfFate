using Game.Data;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [Serializable]
    public struct Vector3DTO
    {
        public float x;
        public float y;
        public float z;

        public Vector3DTO(Vector3 vector)
        {
            x = vector.x;
            y = vector.y;
            z = vector.z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3(x, y, z);
        }
    }

    [Serializable]
    public struct PlayerSaveData
    {
        public Vector3DTO position;
        public Vector3DTO rotation;
        public string sceneName;

        public PlayerSaveData(Vector3 position, Quaternion rotation, string sceneName)
        {
            this.position = new Vector3DTO(position);
            this.rotation = new Vector3DTO(rotation.eulerAngles);
            this.sceneName = sceneName;
        }
    }

    public static class PlayerSaveLoader
    {
        public static bool LoadData()
        {
            if (Repository.TryGetData("Player", out PlayerSaveData savedData))
            {
                PlayerSpawnData.SpawnPosition = savedData.position.ToVector3();
                PlayerSpawnData.SpawnRotation = Quaternion.Euler(savedData.rotation.ToVector3());

                if (!string.IsNullOrEmpty(savedData.sceneName))
                {
                    //LoadingScreenManager.Instance.LoadScene(savedData.sceneName);
                    Time.timeScale = 1;
                }
                return true;
            }
            else
            {
                Debug.LogWarning("No saved data found. Loading default scene and position.");
                return false;
            }
        }

        public static void SaveData(Transform playerTransform)
        {
            PlayerSaveData saveData = new(
                playerTransform.position,
                playerTransform.rotation,
                UnityEngine.SceneManagement.SceneManager.GetActiveScene().name
            );

            Repository.SetData("Player", saveData);
            Debug.Log("Player data saved: " + saveData.sceneName);
        }
    }
}


/*public class StatsSaveLoader : ISaveLoader
{
    public bool LoadData()
    {
        if (Repository.TryGetData("GameResources", out ResourceData data))
        {
            PlayerStats.Strength = data.Strength;
            PlayerStats.Dex = data.Dex;
            PlayerStats.Percept = data.Percept;
            PlayerStats.Mystic = data.Mystic;
            //Debug.Log("Loaded resources data");
            return true;
        }
        return false;
    }

    public void LoadDefaultData()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("SavsInformation/PlayerStats/DefaultPlayerStats");
        if (textAsset == null)
        {
            //Debug.LogError("Default resources file not found!");
            return;
        }

        try
        {
            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                MissingMemberHandling = MissingMemberHandling.Error
            };

            var defaultData = JsonConvert.DeserializeObject<ResourceData>(textAsset.text, settings);
            PlayerStats.Strength = defaultData.Strength;
            PlayerStats.Dex = defaultData.Dex;
            PlayerStats.Percept = defaultData.Percept;
            PlayerStats.Mystic = defaultData.Mystic;
        }
        catch (JsonException ex)
        {
            //Debug.LogError($"JSON error: {ex.Message}");
        }
    }

    public void SaveData()
    {
        var data = new ResourceData
        {
            Strength = PlayerStats.Strength,
            Dex = PlayerStats.Dex,
            Percept = PlayerStats.Percept,
            Mystic = PlayerStats.Mystic
        };
        Repository.SetData("GameResources", data);
        //Debug.Log("Saved resources data");
    }
}

[System.Serializable]
public class ResourceData
{
    public int Strength;
    public int Dex;
    public int Percept;
    public int Mystic;
}*/