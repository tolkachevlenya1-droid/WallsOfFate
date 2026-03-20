using UnityEngine;

[CreateAssetMenu(menuName = "Audience/NPC")]
public class NPCDefinition : ScriptableObject
{
    public string npcName;           // для DialogeTrigger
    public GameObject prefab;        // модель с NavMeshAgent + DialogeTrigger
}