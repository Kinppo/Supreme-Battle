using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; protected set; }

    [Header("Panels")] 
    [SerializeField] private GameObject startPanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject winPanel;
    [SerializeField] private GameObject losePanel;
    [Header("Texts")] 
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI levelNbrText;
    public TextMeshProUGUI levelNextNbrText;
    public TextMeshProUGUI rewardText;
    public TextMeshProUGUI rewardWinText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        var l = GameManager.Instance.level;
        levelText.text = "LEVEL " + l;
        HidePanels();
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
                rewardWinText.text = GameManager.Instance.reward.ToString();
                break;
            case GameState.Lose:
                losePanel.SetActive(true);
                break;
        }

        GameManager.gameState = state;
    }

    public void UpdateReward()
    {
        var reward = GameManager.Instance.reward++;
        rewardText.text = reward.ToString();
    }

    private void HidePanels()
    {
        winPanel.transform.position -= new Vector3(Screen.width, 0, 0);
        losePanel.transform.position -= new Vector3(Screen.width, 0, 0);
    }
}