using UnityEngine;
using Zenject;

namespace Game
{
    public class SceneInteractableManagerFactory : PlaceholderFactory<SceneInteractableManager>
    {
        public override SceneInteractableManager Create()
        {
            var go = new GameObject("SceneInteractableManager");
            return go.AddComponent<SceneInteractableManager>();
        }
    }
}