using System.Linq;
using UnityEngine;
using static DialogueDatabase;

public class dialogueTrigger : MonoBehaviour
{

    [SerializeField] private GameObject dialogueManager;

    private void OnTriggerStay(Collider other)
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            DialogueGraph dialogueGraph = this.GetComponents<DialogueGraph>().Where(t => t.GetName() == name).FirstOrDefault();
            dialogueManager.GetComponent<DialogueManager>().StartDialogue(dialogueGraph);

        }
    }
}
