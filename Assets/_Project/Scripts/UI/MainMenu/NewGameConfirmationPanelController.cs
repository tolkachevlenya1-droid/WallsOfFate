using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Game
{
    class NewGameConfirmationPanelController : MonoBehaviour
    {
        public void OnYesButtonClick()
        {
            
        }

        public void OnNoButtonClick()
        {
            gameObject.SetActive(false);
        }
    }
}
