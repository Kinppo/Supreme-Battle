using System;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Animator animator;
    public float power;
    public TextMeshPro powerText;
    public AgentType agentType;
    public List<GameObject> detectedAgents = new List<GameObject>();
    
    private void Start()
    {
        powerText.text = power.ToString();
    }

    private void Update()
    {
        DetectAgents();
    }

    private void DetectAgents()
    {
        var colls = new Collider[50];
        Physics.OverlapSphereNonAlloc(transform.position, 1.75f, colls,
            agentType == AgentType.Enemy ? GameController.Instance.playerMask : GameController.Instance.enemyMask);

        foreach (var c in colls)
        {
            if (c == null || detectedAgents.Contains(c.gameObject)) continue;
            
            detectedAgents.Add(c.gameObject);
            
            if (c.gameObject.layer == 7 && agentType == AgentType.Player)
            {
                animator.SetTrigger("Hit");
                StartCoroutine(c.gameObject.GetComponent<Agent>().DestroyAgent(0f, 0, true));
                GameManager.Instance.Lose();
            }
            else if (c.gameObject.layer == 6 && agentType == AgentType.Enemy)
            {
                animator.SetTrigger("Hit");
                power--;
                UIManager.Instance.UpdateReward();
                powerText.text = power.ToString();
                StartCoroutine(c.gameObject.GetComponent<Agent>().DestroyAgent(0f, 0, true));

                if (power <= 0)
                {
                    transform.DOScale(0, 0.75f).SetEase(Ease.InOutBack);
                    GameManager.Instance.Win();
                }
            }
        }
    }

    // private void OnCollisionEnter(Collision collision)
    // {
    //     if (GameManager.gameState != GameState.Play) return;
    //
    //     if (collision.gameObject.layer == 7 && agentType == AgentType.Player)
    //     {
    //         animator.SetTrigger("Hit");
    //         StartCoroutine(collision.gameObject.GetComponent<Agent>().DestroyAgent(0f, 0, true));
    //         GameManager.Instance.Lose();
    //     }
    //     else if (collision.gameObject.layer == 6 && agentType == AgentType.Enemy)
    //     {
    //         animator.SetTrigger("Hit");
    //         power--;
    //         UIManager.Instance.UpdateReward();
    //         powerText.text = power.ToString();
    //         StartCoroutine(collision.gameObject.GetComponent<Agent>().DestroyAgent(0f, 0, true));
    //
    //         if (power <= 0)
    //         {
    //             transform.DOScale(0, 0.75f).SetEase(Ease.InOutBack);
    //             GameManager.Instance.Win();
    //         }
    //     }
    // }
    
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 1.75f);
    }
}