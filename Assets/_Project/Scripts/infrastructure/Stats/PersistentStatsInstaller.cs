using UnityEngine;
using Zenject;

namespace Game {
    internal class PersistentStatsInstaller : MonoInstaller, IInstaller {
        [SerializeField] private int initialFreePoints = 5;

        public override void InstallBindings() {
            //var playerStats = new Player.Stats();

            /*playerStats.SetInitialPoints(initialFreePoints);

            Container.Bind<Player.Stats>()
                .FromInstance(playerStats)
                .AsSingle()
                .NonLazy();*/
        }
    }
}
