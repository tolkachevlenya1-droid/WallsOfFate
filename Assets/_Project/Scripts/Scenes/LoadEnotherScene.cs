using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

namespace Game
{
    public class LoadAnotherScene : MonoBehaviour
    {
        [SerializeField] private List<DoorForAnotherScene> doors;

        private void Start()
        {
            foreach (var door in doors)
            {
                if (door != null)
                {
                    door.OnActivated += LoadScene;
                }
            }
        }

        public void LoadScene(string sceneName)
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
            SceneManager.LoadScene(sceneName);
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnDestroy()
        {
            foreach (var door in doors)
            {
                if (door != null)
                {
                    door.OnActivated -= LoadScene;
                }
            }
        }
    }

}
