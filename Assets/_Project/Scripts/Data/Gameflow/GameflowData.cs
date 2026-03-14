using System;
using System.Collections.Generic;

namespace Game.Data
{
    public enum DayPart
    {   
        Part1,
        Part2,
        Part3,
        Part4,
    }

    [Serializable]
    public class DayData
    {
        public int Id;
        public DayPart CurrentPart;
        public int? CurrentQuestId;
    }
}
