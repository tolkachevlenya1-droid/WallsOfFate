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