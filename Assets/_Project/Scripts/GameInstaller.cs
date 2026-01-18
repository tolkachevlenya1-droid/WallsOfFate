using System;
using System.Collections.Generic;
using System.Text;
using Zenject;

namespace Game
{
    public class GameInstaller: MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<LocalizationManager>().AsSingle().NonLazy();
            Container.Bind<LoadingManager>().AsSingle().NonLazy();
        }
    }
}
