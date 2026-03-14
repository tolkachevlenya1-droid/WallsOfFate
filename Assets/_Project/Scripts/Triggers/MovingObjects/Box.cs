using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Box : MonoBehaviour, ITriggerable
{
    public event Action OnActivated;
    public void Triggered()
    {
        ////Debug.Log("Коробку можно двигать!");
        OnActivated?.Invoke();
    }
}
