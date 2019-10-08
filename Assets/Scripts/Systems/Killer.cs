﻿using UnityEngine;

public enum KillType
{
    Cactus,
    Hole
}

public class Killer : MonoBehaviour
{
    [SerializeField] private KillType killType;
    
    private void OnTriggerEnter(Collider other)
    {
        var player = other.GetComponent<Player>();
        
        if (player != null)
            player.KillBy(killType);
    }
}
