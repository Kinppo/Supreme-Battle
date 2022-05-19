using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.Mathematics;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; protected set; }

    [Header("Panels")] [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [Header("Texts")] public TextMeshProUGUI levelText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI rewardWinText;
    public TextMeshProUGUI damagePrice;
    public TextMeshProUGUI speedPrice;
    public TextMeshProUGUI healthPrice;
    [Header("Tutorials")] public List<GameObject> tutorials;
    [Header("Others")] public GameObject gemPrefab;
    public RectTransform gemIcon;
    private int levelReward;
    private bool rewardTextIsAnimating;
    private Vector3 gemIconPosition;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var l = GameManager.Instance.level;
        levelText.text = "LEVEL " + l;
        rewardText.text = GameManager.Instance.reward.ToString();
        rewardWinText.text = GameManager.Instance.reward.ToString();
        HidePanels();
        gemIconPosition = GetGemIconPositon();
    }

    public void SetPanel(GameState state = GameState.Start)
    {
        startPanel.SetActive(false);
        playPanel.SetActive(false);
        winPanel.SetActive(false);
        losePanel.SetActive(false);

        switch (state)
        {
            case GameState.Start:
                startPanel.SetActive(true);
                break;
            case GameState.Play:
                playPanel.SetActive(true);
                break;
            case GameState.Win:
                winPanel.SetActive(true);
                SetUpgradePrices();
                break;
            case GameState.Lose:
                losePanel.SetActive(true);
                break;
        }

        GameManager.gameState = state;
    }

    public void UpdateReward()
    {
        GameManager.Instance.reward += 1;
        levelReward += 1;
        PlayerPrefs.SetInt("reward", GameManager.Instance.reward);
        InstantiateGem();
        rewardText.text = GameManager.Instance.reward.ToString();
    }

    private void InstantiateGem()
    {
        var gem = Instantiate(gemPrefab,
            GameManager.Instance.selectedLevel.rounds.enemyTroops[GameManager.Instance.activeRound].agentBase.transform
                .position + new Vector3(0, 2.5f, 0), quaternion.identity);

        gem.transform.DOMove(gemIconPosition, 0.7f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                gem.gameObject.SetActive(false);
                gemIcon.DOScale(1.25f, 0.3f).OnComplete(() => { gemIcon.DOScale(1f, 0.3f); });
            });
    }

    private Vector3 GetGemIconPositon()
    {
        RaycastHit hit;
        var ray = InputManager.Instance.cam.ScreenPointToRay(gemIcon.position);
        Physics.Raycast(ray, out hit, Mathf.Infinity);
        return new Vector3(hit.point.x - ((hit.point.x / 6) * 0.9f), 0, hit.point.z - 5.4f);
    }

    private void HidePanels()
    {
        winPanel.transform.position -= new Vector3(Screen.width, 0, 0);
        losePanel.transform.position -= new Vector3(Screen.width, 0, 0);
    }

    public void SetUpgradePrices()
    {
        rewardWinText.text = GameManager.Instance.reward.ToString();
        var r = GameManager.Instance.reward;
        var dp = GameManager.Instance.damage * 10 + 5;
        var sp = Mathf.Round(((GameManager.Instance.speed - 2) * 100) + 5);
        var hp = GameManager.Instance.health * 10 + 5;
        damagePrice.text = dp.ToString();
        speedPrice.text = sp.ToString();
        healthPrice.text = hp.ToString();

        if (dp > r) damagePrice.color = Color.red;
        if (sp > r) speedPrice.color = Color.red;
        if (hp > r) healthPrice.color = Color.red;
    }
}