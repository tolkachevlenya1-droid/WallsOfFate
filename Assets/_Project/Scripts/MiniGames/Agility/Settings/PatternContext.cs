using UnityEngine;

public class PatternContext
{
    public Transform arenaCenter;
    public EntrancePoints entrances;
    public PlayerHealth playerHealth;
    public MovementModifiers playerModifiers;
    public MonoBehaviour runner; // кто запускает корутины

    public float boardY;
    public float hazardHeightOffset; // например 2.5f

    public Vector3 ToHazardPlane(Vector3 v)
    {
        return new Vector3(v.x, boardY + hazardHeightOffset, v.z);
    }
}
