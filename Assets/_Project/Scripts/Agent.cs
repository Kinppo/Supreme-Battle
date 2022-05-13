using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AgentState
{
    Idle,
    Run,
    Fight,
    Win
}

public class Agent : MonoBehaviour
{
    public static Agent Instance { get; protected set; }
    private static readonly int Blend1 = Animator.StringToHash("Blend 1");
    private static readonly int Blend2 = Animator.StringToHash("Blend 2");
    public AgentState agentState;
    public NavMeshAgent agent;
    public Animator animator;
    public CapsuleCollider coll;
    public float health;
    public float damage;
    private bool isAnimating;
    private float para1, para2;

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (GameManager.gameState is not GameState.Play) return;

        if (isAnimating)
            SmoothAnimation();
    }

    public void UpdateDestination(Vector3 pos)
    {
        agent.SetDestination(pos);
    }

    private void SmoothAnimation()
    {
        var blend1 = Mathf.Lerp(animator.GetFloat(Blend1), para1, 10 * Time.deltaTime);
        var blend2 = Mathf.Lerp(animator.GetFloat(Blend2), para2, 10 * Time.deltaTime);

        animator.SetFloat(Blend1, blend1);
        animator.SetFloat(Blend2, blend2);

        if ((blend1 - para1 == 0) && (blend2 - para2 == 0))
            isAnimating = false;
    }

    public void ChangeAnimation(AgentState state = AgentState.Idle)
    {
        if (agentState == state) return;

        switch (state)
        {
            case AgentState.Idle:
                para1 = 1;
                para2 = 1;
                break;
            case AgentState.Run:
                para1 = 1;
                para2 = -1;
                break;
            case AgentState.Fight:
                para1 = -1;
                para2 = 1;
                break;
            case AgentState.Win:
                para1 = -1;
                para2 = -1;
                break;
        }

        isAnimating = true;
        agentState = state;
    }
}