using Game.Data;
using System;
using System.Collections;
using UnityEngine;
using Zenject;

namespace Game
{
    public class StartDayController : MonoBehaviour
    {

        private GameflowManager gameflowManager;

        [Inject]
        private void Construct(GameflowManager gameflowManager)
        { 
            this.gameflowManager = gameflowManager;
        }
    }

}
