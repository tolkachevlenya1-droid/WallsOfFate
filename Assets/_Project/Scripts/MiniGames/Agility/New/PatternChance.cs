using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game.MiniGame.Agility
{
    [Serializable]
    public class PatternChance
    {
        public PatternController PatternPrefab; 
        [Range(0f, 100f)] public float Weight = 50f; 
    }
}
