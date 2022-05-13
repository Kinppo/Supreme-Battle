using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using TMPro;

public enum Operation
{
    Multiplication,
    Subtract,
    Divide,
    Add
}

public class Operator : MonoBehaviour
{
    private readonly List<string> symbols = new List<string>() {"x", "-", "÷", "+"};
    public int number;
    public Operation operation;
    public TextMeshPro text;
    public new Renderer renderer;
    private bool isTriggered;

    void Start()
    {
        text.text = symbols[(int) operation] + number;

        switch (operation)
        {
            case Operation.Multiplication:
                renderer.material.SetColor("_BaseColor", GameManager.Instance.operatorColors[0]);
                break;
            case Operation.Subtract:
                renderer.material.SetColor("_BaseColor", GameManager.Instance.operatorColors[1]);
                break;
            case Operation.Divide:
                renderer.material.SetColor("_BaseColor", GameManager.Instance.operatorColors[2]);
                break;
            case Operation.Add:
                renderer.material.SetColor("_BaseColor", GameManager.Instance.operatorColors[3]);
                break;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (isTriggered || collision.gameObject.layer is not (6 or 7)) return;
        isTriggered = true;

        switch (operation)
        {
            case Operation.Multiplication:
                Multiply(collision.gameObject.transform.position, collision);
                break;
            case Operation.Subtract:
                Subtract(collision.gameObject.layer);
                break;
            case Operation.Divide:
                Divide(collision.gameObject.layer);
                break;
            case Operation.Add:
                Add(collision.gameObject.transform.position, collision);
                break;
        }

        transform.DOScale(0, 0.7f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }

    private void Multiply(Vector3 pos, Collider collision)
    {
        if (collision.gameObject.layer == 6)
        {
            var length = (number - 1) * Spawner.Instance.players.Count;

            for (var i = 0; i < length; i++)
            {
                var obj = Instantiate(collision.gameObject, pos, Quaternion.identity).GetComponent<Player>();
                obj.gameObject.transform.SetParent(Spawner.Instance.playersParent);
                Spawner.Instance.players.Add(obj);

                // obj.UpdateDestination(Spawner.Instance.enemies.Count > 0
                //     ? Spawner.Instance.enemies[0].gameObject.transform.position
                //     : GameManager.Instance.selectedLevel.rounds.enemyTroops[GameManager.Instance.activeRound].agentBase
                //         .position);

                obj.UpdateDestination(GameManager.Instance.selectedLevel.rounds
                    .enemyTroops[GameManager.Instance.activeRound].agentBase
                    .position);

                obj.agentState = AgentState.Idle;
                obj.ChangeAnimation(AgentState.Run);
            }
        }
        else if (collision.gameObject.layer == 7)
        {
            var length = (number - 1) * Spawner.Instance.enemies.Count;

            for (var i = 0; i < length; i++)
            {
                var obj = Instantiate(collision.gameObject, pos, Quaternion.identity).GetComponent<Enemy>();
                obj.gameObject.transform.SetParent(Spawner.Instance.enemiesParent);
                Spawner.Instance.enemies.Add(obj);

                obj.UpdateDestination(Spawner.Instance.players.Count > 0
                    ? Spawner.Instance.players[0].gameObject.transform.position
                    : GameManager.Instance.selectedLevel.rounds.playerTroops[0].agentBase
                        .position);

                obj.agentState = AgentState.Idle;
                obj.ChangeAnimation(AgentState.Run);
            }
        }
    }

    private void Subtract(int layer)
    {
        if (layer == 6)
        {
            var players = Spawner.Instance.players;

            for (int i = 0; i <= number - 1; i++)
            {
                if (players.Count > 0)
                {
                    Destroy(Spawner.Instance.players[players.Count - 1].gameObject);
                    Instantiate(Spawner.Instance.playerBlood,
                        Spawner.Instance.players[players.Count - 1].gameObject.transform.position,
                        Quaternion.Euler(90, 0, 0));
                    Spawner.Instance.players.RemoveAt(players.Count - 1);
                }
            }
        }
        else if (layer == 7)
        {
            var enemies = Spawner.Instance.enemies;

            for (int i = 0; i <= number - 1; i++)
            {
                if (enemies.Count > 0)
                {
                    Destroy(Spawner.Instance.enemies[enemies.Count - 1].gameObject);
                    Instantiate(Spawner.Instance.enemyBlood,
                        Spawner.Instance.enemies[enemies.Count - 1].gameObject.transform.position,
                        Quaternion.Euler(90, 0, 0));
                    Spawner.Instance.enemies.RemoveAt(enemies.Count - 1);
                }
            }
        }
    }

    private void Add(Vector3 pos, Collider collision)
    {
        if (collision.gameObject.layer == 6)
        {
            for (var i = 0; i < number; i++)
            {
                var obj = Instantiate(collision.gameObject, pos, Quaternion.identity).GetComponent<Player>();
                obj.gameObject.transform.SetParent(Spawner.Instance.playersParent);
                Spawner.Instance.players.Add(obj);

                obj.UpdateDestination(Spawner.Instance.enemies.Count > 0
                    ? Spawner.Instance.enemies[0].gameObject.transform.position
                    : GameManager.Instance.selectedLevel.rounds.enemyTroops[GameManager.Instance.activeRound].agentBase
                        .position);

                obj.agentState = AgentState.Idle;
                obj.ChangeAnimation(AgentState.Run);
            }
        }
        else if (collision.gameObject.layer == 7)
        {
            for (var i = 0; i < number; i++)
            {
                var obj = Instantiate(collision.gameObject, pos, Quaternion.identity).GetComponent<Enemy>();
                obj.gameObject.transform.SetParent(Spawner.Instance.enemiesParent);
                Spawner.Instance.enemies.Add(obj);

                obj.UpdateDestination(Spawner.Instance.players.Count > 0
                    ? Spawner.Instance.players[0].gameObject.transform.position
                    : GameManager.Instance.selectedLevel.rounds.playerTroops[0].agentBase
                        .position);

                obj.agentState = AgentState.Idle;
                obj.ChangeAnimation(AgentState.Run);
            }
        }
    }

    private void Divide(int layer)
    {
        if (layer == 6)
        {
            var players = Spawner.Instance.players;
            int quotient = players.Count / number;

            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (i >= quotient)
                {
                    Destroy(Spawner.Instance.players[i].gameObject);
                    Spawner.Instance.players.RemoveAt(i);
                }
            }
        }
        else if (layer == 7)
        {
            var enemies = Spawner.Instance.enemies;
            int quotient = enemies.Count / number;

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (i >= quotient)
                {
                    Destroy(Spawner.Instance.enemies[i].gameObject);
                    Spawner.Instance.enemies.RemoveAt(i);
                }
            }
        }
    }
}