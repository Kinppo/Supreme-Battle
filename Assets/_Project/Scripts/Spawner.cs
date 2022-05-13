using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public static Spawner Instance { get; protected set; }
    public GameObject playerBlood;
    public GameObject enemyBlood;
    [HideInInspector] public Transform enemiesParent;
    [HideInInspector] public Transform playersParent;
    [HideInInspector] public List<Agent> players = new List<Agent>();
    [HideInInspector] public List<Agent> enemies = new List<Agent>();
    private Vector3 closerEnemy;
    private Vector3 closerPlayer;
    private Round rounds;

    void Start()
    {
        Instance = this;
        rounds = GameManager.Instance.selectedLevel.rounds;
        playersParent = new GameObject("Players Parent").transform;
        enemiesParent = new GameObject("Enemies Parent").transform;

        SpawnPlayers(rounds.playerTroops[0]);
        SpawnEnemies(rounds.enemyTroops[0]);
    }

    public void SpawnPlayers(Troop troop)
    {
        for (int i = 0; i < troop.number; i++)
        {
            var pos = troop.agentBase.position + new Vector3(Random.Range(-1f, 1f), 0, +Random.Range(1.5f, 2.25f));
            var obj = Instantiate(troop.agent, pos, Quaternion.identity);

            obj.gameObject.transform.SetParent(playersParent);
            players.Add(obj);

            
            obj.UpdateDestination(GameManager.Instance.selectedLevel.rounds
                .enemyTroops[GameManager.Instance.activeRound].agentBase
                .position);
            obj.ChangeAnimation(AgentState.Run);
        }
    }

    public void SpawnEnemies(Troop troop)
    {
        for (int i = 0; i < troop.number; i++)
        {
            var pos = troop.agentBase.position +
                      new Vector3(Random.Range(-1.5f, 1.5f), 0, +Random.Range(-1.5f, -2.25f));
            var obj = Instantiate(troop.agent, pos, Quaternion.identity);

            obj.gameObject.transform.SetParent(enemiesParent);
            enemies.Add(obj);

            obj.UpdateDestination(
                GameManager.Instance.selectedLevel.rounds.playerTroops[0].agentBase
                    .position);
            obj.ChangeAnimation(AgentState.Run);
        }
    }
}