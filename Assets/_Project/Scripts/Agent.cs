using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum AgentType
{
    Player,
    Enemy,
    Dummy
}

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
    public AgentType agentType;
    public NavMeshAgent agent;
    public Animator animator;
    public new Renderer renderer;
    public CapsuleCollider coll;
    public Rigidbody rb;
    public ParticleSystem blastEffect;
    public float health;
    public float damage;
    public float speed;
    public float detectionRange;
    private bool isAnimating;
    private float para1, para2;
    [HideInInspector] public Vector3 destination, destinationBase;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        speed = speed < 4.5f ? GameManager.Instance.speed : 4.5f;
        agent.speed = speed;
        health = GameManager.Instance.health;
        damage = GameManager.Instance.damage;
    }

    private void Update()
    {
        if (isAnimating)
            SmoothAnimation();

        if (GameManager.gameState is not GameState.Play) return;
        if (agentType == AgentType.Dummy) DetectOtherAgents();

        DetectFence();
        DetectEnemy();
        CheckForDeath();
    }

    public void UpdateDestination(Vector3 pos)
    {
        if (!agent.enabled) return;
        agent.SetDestination(pos);
        destination = pos;
    }

    private void CheckForDeath()
    {
        if (health <= 0) StartCoroutine(DestroyAgent(0));
    }

    private void SmoothAnimation()
    {
        var blend1 = Mathf.Lerp(animator.GetFloat(Blend1), para1, 12 * Time.deltaTime);
        var blend2 = Mathf.Lerp(animator.GetFloat(Blend2), para2, 12 * Time.deltaTime);

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

    private void DetectFence()
    {
        if (!agent.enabled) return;

        if (agent.remainingDistance < 1.5f)
        {
            agent.speed = 0;
            ChangeAnimation();
        }
        else
        {
            agent.speed = speed;
            ChangeAnimation(AgentState.Run);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14 && agent.enabled)
        {
            agent.enabled = false;
            rb.constraints = RigidbodyConstraints.None;
            StartCoroutine(DestroyAgent(0.15f, -1.5f));
        }
    }

    public IEnumerator DestroyAgent(float time, float posZ = 0, bool isBaseDestroy = false)
    {
        yield return new WaitForSeconds(time);

        var arr = new List<Agent>();
        arr = agentType == AgentType.Player ? GameController.Instance.players : GameController.Instance.enemies;

        if (this != null)
        {
            Destroy(gameObject);
            Instantiate(
                agentType == AgentType.Player
                    ? GameController.Instance.playerBlood
                    : GameController.Instance.enemyBlood,
                transform.position + new Vector3(0, posZ, 0),
                Quaternion.identity);
            arr.Remove(this);
            if (!isBaseDestroy) AudioManager.Instance.PlaySound(AudioManager.Instance.kill);
        }

        if (GameManager.gameState == GameState.Play && agentType == AgentType.Player && arr.Count <= 0)
            GameManager.Instance.Lose();
        else if (GameManager.gameState == GameState.Play && agentType == AgentType.Enemy && arr.Count <= 0)
            GameController.Instance.SetAgentsDestinationToBase();
    }

    private void DetectEnemy()
    {
        var isFighting = false;
        var colls = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, detectionRange, colls,
            agentType == AgentType.Enemy ? GameController.Instance.playerMask : GameController.Instance.enemyMask);

        if (health <= 0) return;

        foreach (var c in colls)
        {
            if (c == null) continue;
            UpdateDestination(c.transform.position);
            ChangeAnimation(AgentState.Fight);
            health -= damage;
            isFighting = true;
            break;
        }

        if (isFighting || destination == destinationBase)
            return;

        UpdateDestination(destinationBase);

        if (!agent.enabled || agent.speed == 0) return;
        ChangeAnimation(AgentState.Run);
    }

    private void DetectOtherAgents()
    {
        var colls = new Collider[1];
        Physics.OverlapSphereNonAlloc(transform.position, detectionRange, colls,
            GameController.Instance.playerEnemyMask);

        foreach (var c in colls)
        {
            if (c == null) continue;
            var detectedAgent = c.gameObject.GetComponent<Agent>();

            agentType = detectedAgent.agentType;
            gameObject.layer = detectedAgent.gameObject.layer;
            destinationBase = detectedAgent.destinationBase;
            UpdateDestination(destinationBase);
            ChangeAnimation(AgentState.Run);
            renderer.material.color = detectedAgent.renderer.material.color;

            if (agentType == AgentType.Player)
                GameController.Instance.players.Add(this);
            else
                GameController.Instance.enemies.Add(this);

            break;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}