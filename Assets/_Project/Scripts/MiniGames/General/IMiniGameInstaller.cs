using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game.MiniGame
{
    public interface IMiniGameInstaller
    {
        public void InitializeWithData(MiniGameData gameData);

        public void OnMiniGameEnded(bool playerWin);
    }
}
