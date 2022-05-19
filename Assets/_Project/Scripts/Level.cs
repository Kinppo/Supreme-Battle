using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Troop
{
    public Agent agent;
    public int number;
    public Base agentBase;
    public float basePower;
}

[Serializable]
public class Round
{
    public List<Troop> enemyTroops = new List<Troop>();
    public List<Troop> playerTroops = new List<Troop>();
}

public class Level : MonoBehaviour
{
    public Round rounds;
    public bool hasTutorial;
}