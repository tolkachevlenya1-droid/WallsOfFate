using Player;
using UnityEngine;

public class UIToggle : MonoBehaviour
{
    [SerializeField] private GameObject targetObject; 

    public void ToggleObject() {
        if (targetObject != null) {
            bool newState = !targetObject.activeSelf;
            targetObject.SetActive(newState);
            if(newState) {
                transform.GetComponent<StatsControllerUI>().UpdateAllStatsUI();
            }
        }
    }

    public void EnableObject() {
        if (targetObject != null) targetObject.SetActive(true);
    }

    public void DisableObject() {
        if (targetObject != null) targetObject.SetActive(false);
    }
}
