using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class GameController : MonoBehaviour
{
    public static GameController Instance { get; protected set; }
    public Transform camPoint;
    public NavMeshSurface navMesh;
    public LayerMask playerMask;
    public LayerMask enemyMask;
    public LayerMask playerEnemyMask;
    public GameObject playerBlood;
    public GameObject enemyBlood;
    [HideInInspector] public Transform enemiesParent;
    [HideInInspector] public Transform playersParent;
    [HideInInspector] public List<Agent> players = new List<Agent>();
    [HideInInspector] public List<Agent> enemies = new List<Agent>();

    void Awake()
    {
        Instance = this;
        playersParent = new GameObject("Players Parent").transform;
        enemiesParent = new GameObject("Enemies Parent").transform;
    }

    public void UpdateNaveMesh()
    {
        navMesh.UpdateNavMesh(navMesh.navMeshData);
    }

    public void StopAgents()
    {
        foreach (var agent in enemies)
            agent.agent.enabled = false;

        foreach (var agent in players)
            agent.agent.enabled = false;
    }

    public void KillAllEnemies()
    {
        foreach (var enemy in enemies)
            StartCoroutine(enemy.DestroyAgent(0));
    }

    public void SetWinAnimationToAgents()
    {
        foreach (var agent in enemies)
            agent.ChangeAnimation(AgentState.Win);

        foreach (var agent in players)
            agent.ChangeAnimation(AgentState.Win);
    }

    private int test2 = 0;

    public void SpawnPlayers(Troop troop)
    {
        for (int i = 0; i < troop.number; i++)
        {
            var pos = troop.agentBase.transform.position +
                      new Vector3(Random.Range(-1f, 1f), 0, +Random.Range(1f, 2f));
            var obj = Instantiate(troop.agent, pos, Quaternion.identity);
            obj.name = "Players " + i;

            obj.gameObject.transform.SetParent(playersParent.transform);
            players.Add(obj);

            obj.destinationBase = GameManager.Instance.selectedLevel.rounds
                .enemyTroops[GameManager.Instance.activeRound].agentBase
                .transform.position + new Vector3(0, 0, 1.5f);
            obj.UpdateDestination(obj.destinationBase);
            obj.ChangeAnimation(AgentState.Run);
        }
    }

    public void SpawnEnemies(Troop troop)
    {
        for (int i = 0; i < troop.number; i++)
        {
            var pos = troop.agentBase.transform.position +
                      new Vector3(Random.Range(-2f, 2f), 0, +Random.Range(-1f, -1.5f));
            var obj = Instantiate(troop.agent, pos, Quaternion.Euler(0, 180, 0));
            obj.name = "Enemy " + i;

            obj.gameObject.transform.SetParent(enemiesParent);
            enemies.Add(obj);

            obj.destinationBase = GameManager.Instance.selectedLevel.rounds.playerTroops[0].agentBase
                .transform.position + new Vector3(0, 0, -1.5f);
            obj.UpdateDestination(obj.destinationBase);
            obj.ChangeAnimation(AgentState.Run);
        }
    }
}