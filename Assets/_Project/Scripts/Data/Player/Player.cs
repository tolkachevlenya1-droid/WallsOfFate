using System;
using System.Collections.Generic;

namespace Game.Data
{
    public enum ResourceType
    {
        Gold,
        Food,
        PeopleSatisfaction,
        CastleStrength
    }

    public enum StatType
    {
        Strength,
        Int,
        Dex,
        Percept,
        Mystic
    }

    public class Player
    {

        private int _freePoints;
        public int FreePoints
        {
            get => _freePoints;
            set
            {
                if (_freePoints != value) _freePoints = Math.Max(value, 0);
            }
        }

        public void AddFreePoints(int delta) { FreePoints += delta; }

        private readonly Dictionary<StatType, int> stats = new() {
            { StatType.Strength, 0 },
            { StatType.Int, 0 },
            { StatType.Dex, 0 },
            { StatType.Percept, 0 },
            { StatType.Mystic, 0 }
        };

        public event Action<StatType, int> StatChanged;

        public int GetStat(StatType type) => stats[type];

        public void AddStat(StatType type, int delta)
        {
            stats[type] = Math.Max(stats[type] + delta, 0);
            StatChanged?.Invoke(type, stats[type]);
        }

        public void SetStat(StatType type, int value)
        {
            stats[type] = Math.Max(value, 0);
            StatChanged?.Invoke(type, stats[type]);
        }

        private readonly Dictionary<ResourceType, int> resources = new() {
            { ResourceType.Gold, 0 },
            { ResourceType.Food, 0 },
            { ResourceType.PeopleSatisfaction, 0 },
            { ResourceType.CastleStrength, 0 }
        };

        public int GetResource(ResourceType type) => resources[type];

        public event Action<ResourceType, int> ResourceChanged;

        public void AddResource(ResourceType type, int delta)
        {
            resources[type] = Math.Max(resources[type] + delta, 0);
            ResourceChanged?.Invoke(type, resources[type]);
        }

        public void SetResource(ResourceType type, int value)
        {
            resources[type] = Math.Max(value, 0);
            ResourceChanged?.Invoke(type, resources[type]);
        }
    }
}
