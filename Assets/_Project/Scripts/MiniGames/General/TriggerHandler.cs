using UnityEngine;


namespace Game.MiniGame
{
    public class TriggerHandler : MonoBehaviour
    {
        public System.Action<GameObject, GameObject> OnObjectEnteredTrigger; // Событие для передачи данных

        private void OnTriggerEnter(Collider other)
        {

            if (!gameObject.activeSelf || !other.gameObject.activeSelf)
            {
                return; 
            }
            ////Debug.Log($"Объект {other.name} вошёл в триггер {gameObject.name}");
            OnObjectEnteredTrigger?.Invoke(gameObject, other.gameObject); // Вызов события
        }
    }
}