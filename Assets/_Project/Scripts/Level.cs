using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Troop
{
    public Agent agent;
    public int number;
    public Transform agentBase;
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
}