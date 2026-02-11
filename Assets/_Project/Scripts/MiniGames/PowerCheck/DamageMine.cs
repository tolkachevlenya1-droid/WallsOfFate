using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game
{
    public class DamageMine : Mine
    {
        public DamageMine(uint number, float ñooldown, GameObject mine) : base(number, ñooldown, mine) { }

        public void Damage(MiniGamePlayer player1, MiniGamePlayer player2)
        {
            ////Debug.Log(player2.Name + " áüåò " + player1.Name);
            player1.TakeDamage(player2.Damage);
        }
    }
}

