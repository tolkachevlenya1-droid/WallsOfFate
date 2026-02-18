using UnityEngine;

public enum Entrance
{
    NorthEast,
    NorthWest,
    SouthEast,
    SouthWest
}

public class EntrancePoints : MonoBehaviour
{
    public Transform northEast;
    public Transform northWest;
    public Transform southEast;
    public Transform southWest;

    public Transform Get(Entrance e) => e switch
    {
        Entrance.NorthEast => northEast,
        Entrance.NorthWest => northWest,
        Entrance.SouthEast => southEast,
        Entrance.SouthWest => southWest,
        _ => northEast
    };
}
