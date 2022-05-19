using DG.Tweening;
using TMPro;
using UnityEngine;

public class Base : MonoBehaviour
{
    public Animator animator;
    public float power;
    public TextMeshPro powerText;
    public AgentType agentType;

    private void Start()
    {
        powerText.text = power.ToString();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (GameManager.gameState != GameState.Play) return;

        if (collision.gameObject.layer == 7 && agentType == AgentType.Player)
        {
            animator.SetTrigger("Hit");
            StartCoroutine(collision.gameObject.GetComponent<Agent>().DestroyAgent(0f));
            GameManager.Instance.Lose();
        }
        else if (collision.gameObject.layer == 6 && agentType == AgentType.Enemy)
        {
            animator.SetTrigger("Hit");
            power--;
            UIManager.Instance.UpdateReward();
            powerText.text = power.ToString();
            StartCoroutine(collision.gameObject.GetComponent<Agent>().DestroyAgent(0f));

            if (power <= 0)
            {
                transform.DOScale(0, 0.75f).SetEase(Ease.InOutBack);
                GameManager.Instance.Win();
            }
        }
    }
}