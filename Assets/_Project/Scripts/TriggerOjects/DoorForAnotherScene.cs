using System;
using UnityEngine;
using Zenject;

namespace Game
{
    internal class DoorForAnotherScene : MonoBehaviour, ITriggerable
    {
        [SerializeField] string SceneName;
        [SerializeField] private Vector3 SpawnPosition; // Точка спавна в новой сцене
        [SerializeField] private Vector3 SpawnEulerAngles = Vector3.zero; // Углы Эйлера для поворота

        private LoadingManager loadingManager;
        [Inject]
        public void Construct(LoadingManager loadingManager)
        {
            this.loadingManager = loadingManager;
        }

        // Скрытое поле для Quaternion, вычисляемое на основе SpawnEulerAngles
        private Quaternion SpawnRotation => Quaternion.Euler(SpawnEulerAngles);
        public int dayNumber = -1;
        public event Action<string> OnActivated;
        public void Triggered()
        {
            if (ShouldTrigger()) {
                //Debug.Log("Переход на новую локу!");

                // Сохраняем данные о точке спавна
                PlayerSpawnData.SpawnPosition = SpawnPosition;
                PlayerSpawnData.SpawnRotation = SpawnRotation;

                // Вызываем событие для загрузки сцены
                loadingManager.LoadScene(SceneName);
            }
        }

        public bool ShouldTrigger() {
            
            if (dayNumber == -1) return true;
            else if (dayNumber != -1 && dayNumber == Quest.QuestCollection.CurrentDayNumber) return true;
            else return false;
        }
    }
}