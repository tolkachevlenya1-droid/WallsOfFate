using Player;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIToggle : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; 

    public void ToggleObject() {
        if (targetObject != null) {
            bool newState = !targetObject.activeSelf;
            targetObject.SetActive(newState);
            if(newState) {
                transform.GetComponent<StatsControllerUI>().UpdateAllStatsUI();
                Time.timeScale = 0;
            }
            else Time.timeScale = 1;
        }
    }
}
