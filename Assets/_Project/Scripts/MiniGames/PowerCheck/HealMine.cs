using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MiniGame.PowerCheck
{
    public class HealMine : Mine
    {
        public HealMine(uint number, float ñooldown, GameObject mine) : base(number, ñooldown, mine) { }

        public void Heal(MiniGamePlayer player)
        {
            player.TakeHeal();
        }
    }
}

