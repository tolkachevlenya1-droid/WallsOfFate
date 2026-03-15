using UnityEngine;
using Zenject;

namespace Game
{
    public class DisplayInstaller : MonoInstaller
    {
        // Публичная переменная для назначения объекта через инспектор
        public GameObject displayObject;

        public override void InstallBindings()
        {
            if (displayObject == null)
            {
                //Debug.LogError("Объект 'displayObject' не назначен в инспекторе.");
                return;
            }

            // Получаем компонент ToggleObjectOnButtonPress с объекта
            ToggleObjectOnButtonPress toggleScript = displayObject.GetComponent<ToggleObjectOnButtonPress>();

            if (toggleScript == null)
            {
                //Debug.LogError("На объекте 'displayObject' отсутствует компонент ToggleObjectOnButtonPress.");
                return;
            }

            // Регистрируем компонент ToggleObjectOnButtonPress в контейнере Zenject
            Container
                .Bind<ToggleObjectOnButtonPress>()
                .FromInstance(toggleScript)
                .AsCached();
        }
    }
}