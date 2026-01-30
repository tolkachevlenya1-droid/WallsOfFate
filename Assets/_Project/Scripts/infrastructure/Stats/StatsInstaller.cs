using Player;
using System;
using UnityEngine;
using Zenject;

namespace Game
{
    public class StatsInstaller : MonoInstaller
    {
        [SerializeField] private int initialFreePoints = 5;

        public override void InstallBindings() {
            InstallStats();
        }

        private void InstallStats() {

            if (!Container.HasBinding<Stats>()) {
                var playerStats = new Stats();
                playerStats.SetInitialPoints(initialFreePoints);

                Container.Bind<Stats>()
                    .FromInstance(playerStats)
                    .AsSingle()
                    .NonLazy();
            }
        }
    }
}
