using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Agent
{
    public static Enemy Instance { get; protected set; }

    private void Awake()
    {
        Instance = this;
    }
}
