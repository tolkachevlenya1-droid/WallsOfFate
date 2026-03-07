using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.MiniGame.Agility
{
    public class AttackPattern : MonoBehaviour
    {
        [Header("Available Patterns with Weights")]
        public List<PatternChance> PatternChances = new List<PatternChance>();
        public PatternController GetNextPattern()
        {
            if (PatternChances.Count == 0) return null;

            float totalWeight = PatternChances.Sum(p => p.Weight);

            float randomValue = UnityEngine.Random.Range(0, totalWeight);

            float cumulativeWeight = 0;

            foreach (var chance in PatternChances)
            {
                cumulativeWeight += chance.Weight;
                if (randomValue <= cumulativeWeight)
                {
                    return chance.PatternPrefab;
                }
            }

            return PatternChances[0].PatternPrefab;
        }
    }

}