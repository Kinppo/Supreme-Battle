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
    private List<GameObject> triggered = new List<GameObject>();

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
        if (collision.gameObject.layer is not (6 or 7) || number <= 0 ||
            triggered.Contains(collision.gameObject)) return;
        triggered.Add(collision.gameObject);

        switch (operation)
        {
            case Operation.Multiplication:
                Multiply(collision.gameObject);
                break;
            case Operation.Subtract:
                Subtract(collision);
                break;
            case Operation.Divide:
                Divide(collision.gameObject.layer);
                break;
            case Operation.Add:
                Add(collision.gameObject);
                break;
        }
    }

    private void Multiply(GameObject agent)
    {
        var pos = agent.transform.position;

        switch (agent.layer)
        {
            case 6:
            {
                for (var i = 0; i < number - 1; i++)
                {
                    var obj = Instantiate(agent, pos, agent.transform.rotation);
                    triggered.Add(obj);

                    var player = obj.GetComponent<Agent>();
                    obj.gameObject.transform.SetParent(GameController.Instance.playersParent);
                    GameController.Instance.players.Add(player);

                    player.UpdateDestination(GameManager.Instance.selectedLevel.rounds
                        .enemyTroops[GameManager.Instance.activeRound].agentBase
                        .transform.position);

                    player.agentState = AgentState.Idle;
                    player.ChangeAnimation(AgentState.Run);
                }

                break;
            }
            case 7:
            {
                for (var i = 0; i < number - 1; i++)
                {
                    var obj = Instantiate(agent, pos, agent.transform.rotation);
                    triggered.Add(obj);

                    var enemy = obj.GetComponent<Agent>();
                    obj.gameObject.transform.SetParent(GameController.Instance.enemiesParent);
                    GameController.Instance.enemies.Add(enemy);

                    enemy.UpdateDestination(GameManager.Instance.selectedLevel.rounds
                        .playerTroops[0].agentBase
                        .transform.position);

                    enemy.agentState = AgentState.Idle;
                    enemy.ChangeAnimation(AgentState.Run);
                }
                break;
            }
        }
    }

    private void Subtract(Collider collision)
    {
        number--;
        text.text = symbols[(int) operation] + number;
        StartCoroutine(collision.GetComponent<Agent>().DestroyAgent(0.25f, 0.2f));

        if (number == 0) DestroyOperator();
    }

    private void Add(GameObject agent)
    {
        var pos = agent.transform.position;

        switch (agent.layer)
        {
            case 6:
            {
                var obj = Instantiate(agent, pos, agent.transform.rotation);
                triggered.Add(obj);

                var player = obj.GetComponent<Agent>();
                obj.gameObject.transform.SetParent(GameController.Instance.playersParent);
                GameController.Instance.players.Add(player);

                player.UpdateDestination(GameManager.Instance.selectedLevel.rounds
                    .enemyTroops[GameManager.Instance.activeRound].agentBase
                    .transform.position);

                player.agentState = AgentState.Idle;
                player.ChangeAnimation(AgentState.Run);
                number--;
                text.text = symbols[(int) operation] + number;
                break;
            }
            case 7:
            {
                var obj = Instantiate(agent, pos, agent.transform.rotation);
                triggered.Add(obj);

                var enemy = obj.GetComponent<Agent>();
                obj.gameObject.transform.SetParent(GameController.Instance.enemiesParent);
                GameController.Instance.enemies.Add(enemy);

                enemy.UpdateDestination(GameManager.Instance.selectedLevel.rounds
                    .playerTroops[0].agentBase
                    .transform.position);

                enemy.agentState = AgentState.Idle;
                enemy.ChangeAnimation(AgentState.Run);
                number--;
                text.text = symbols[(int) operation] + number;
                break;
            }
        }

        if (number == 0) DestroyOperator();
    }

    private void Divide(int layer)
    {
        if (layer == 6)
        {
            var players = GameController.Instance.players;
            int quotient = players.Count / number;

            for (int i = players.Count - 1; i >= 0; i--)
            {
                if (i >= quotient)
                {
                    StartCoroutine(GameController.Instance.players[i].DestroyAgent(0));
                }
            }
        }
        else if (layer == 7)
        {
            var enemies = GameController.Instance.enemies;
            int quotient = enemies.Count / number;

            for (int i = enemies.Count - 1; i >= 0; i--)
            {
                if (i >= quotient)
                {
                    StartCoroutine(GameController.Instance.enemies[i].DestroyAgent(0));
                }
            }
        }
    }

    private void DestroyOperator()
    {
        transform.DOScale(0, 0.7f).SetEase(Ease.InBack).OnComplete(() => Destroy(gameObject));
    }
}