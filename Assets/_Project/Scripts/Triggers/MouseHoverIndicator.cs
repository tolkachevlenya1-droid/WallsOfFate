using UnityEngine;

public class MouseHoverIndicator : MonoBehaviour
{
    private GameObject interactionIndicator; 

    private void Start()
    {
        interactionIndicator = transform.Find("InteractionIndicator")?.gameObject;

        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(false);
        }
    }

    private bool FindSIblin()
    {
        bool isSiblin = false;
        // Проверяем, есть ли соседний элемент с именем "InteractionIndicator"
        if (transform.parent != null)
        {
            foreach (Transform sibling in transform.parent)
            {
                if (sibling != transform && sibling.name == "InteractionIndicator")
                {
                    isSiblin = sibling.gameObject.activeSelf;                   
                }
            }
        }
        return isSiblin;
    }

    private void Update()
    {
        if (FindSIblin())
        {
            interactionIndicator.SetActive(false);
        }

    }

    private void OnMouseEnter()
    {
        
        if (FindSIblin()) return;

        if (interactionIndicator != null)
        {
            interactionIndicator.SetActive(true);
        }
    }

    private void OnMouseExit()
    {
        if (interactionIndicator != null || FindSIblin())
        {
            interactionIndicator.SetActive(false);
        }
    }
}