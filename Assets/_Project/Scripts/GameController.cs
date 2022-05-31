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
    private int playerX, playerZ, enemyX, enemyZ;
    private Vector3 lastPlayerPos, lastEnemyPos;
    private float space = 0.55f;

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

    public void ActivateAgents()
    {
        foreach (var agent in enemies)
        {
            agent.agent.enabled = true;
            agent.ChangeAnimation(AgentState.Run);
        }

        foreach (var agent in players)
        {
            agent.agent.enabled = true;
            agent.ChangeAnimation(AgentState.Run);
        }
    }

    public void SetAgentsDestinationToBase()
    {
        foreach (var agent in players)
            agent.UpdateDestination(agent.destinationBase);
    }

    public void SpawnPlayers(Troop troop)
    {
        for (int i = 0; i < GameManager.Instance.soldiersNumber; i++)
        {
            lastPlayerPos = GetPlayerCoordinates();
            var pos = troop.agentBase.transform.position + new Vector3(0, 0, 1.65f) + lastPlayerPos;

            var obj = Instantiate(troop.agent, pos, Quaternion.identity);
            obj.name = "Players " + i;

            obj.gameObject.transform.SetParent(playersParent.transform);
            players.Add(obj);

            obj.destinationBase = GameManager.Instance.selectedLevel.rounds
                .enemyTroops[GameManager.Instance.activeRound].agentBase
                .transform.position + new Vector3(0, 0, 1.5f);
            obj.UpdateDestination(obj.destinationBase);
        }
    }

    public void SpawnEnemies(Troop troop)
    {
        for (int i = 0; i < troop.number; i++)
        {
            lastEnemyPos = GetEnemyCoordinates();
            var pos = troop.agentBase.transform.position + new Vector3(0, 0, -1.65f) + lastEnemyPos;

            var obj = Instantiate(troop.agent, pos, Quaternion.Euler(0, 180, 0));
            obj.name = "Enemy " + i;

            obj.gameObject.transform.SetParent(enemiesParent);
            enemies.Add(obj);

            obj.destinationBase = GameManager.Instance.selectedLevel.rounds.playerTroops[0].agentBase
                .transform.position + new Vector3(0, 0, -1.5f);
            obj.UpdateDestination(obj.destinationBase);
        }
    }

    private Vector3 GetPlayerCoordinates()
    {
        var pos = new Vector3(space * playerX, 0, space * playerZ);

        if (playerZ == 1)
            playerX = playerX > 0 ? playerX * -1 : (playerX * -1) + 1;

        playerZ = playerZ == 0 ? 1 : 0;
        return pos;
    }

    private Vector3 GetEnemyCoordinates()
    {
        var pos = new Vector3(space * enemyX, 0, space * enemyZ);

        if (enemyZ == 1)
            enemyX = enemyX > 0 ? enemyX * -1 : (enemyX * -1) + 1;

        enemyZ = enemyZ == 0 ? 1 : 0;
        return pos;
    }

    public void AddPlayer()
    {
        lastPlayerPos = GetPlayerCoordinates();
        var pos = GameManager.Instance.selectedLevel.rounds.playerTroops[0].agentBase.transform.position +
                  new Vector3(0, 0, 1.65f) + lastPlayerPos;

        var obj = Instantiate(GameManager.Instance.selectedLevel.rounds.playerTroops[0].agent, pos,
            Quaternion.identity);

        obj.gameObject.transform.SetParent(playersParent.transform);
        players.Add(obj);

        obj.destinationBase = GameManager.Instance.selectedLevel.rounds
            .enemyTroops[GameManager.Instance.activeRound].agentBase
            .transform.position + new Vector3(0, 0, 1.5f);
        obj.UpdateDestination(obj.destinationBase);
        obj.blastEffect.gameObject.SetActive(true);
        obj.blastEffect.Play();
    }

    public void PlayBlastEffect()
    {
        foreach (var p in players)
        {
            p.blastEffect.gameObject.SetActive(true);
            p.blastEffect.Play();
        }
    }
}