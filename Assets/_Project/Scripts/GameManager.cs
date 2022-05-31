using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum GameState
{
    Start,
    Build,
    Play,
    Upgrade,
    Lose,
    Win
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; protected set; }
    public static GameState gameState = GameState.Start;
    public ParticleSystem confetti;
    public int health, damage, soldiersNumber;
    public float speed;
    public Material fog;
    public List<Color32> operatorColors = new List<Color32>();
    public List<Color32> colors = new List<Color32>();
    public List<Level> levels = new List<Level>();
    [HideInInspector] public Level selectedLevel;
    [HideInInspector] public int level, reward;
    [HideInInspector] public int activeRound;
    [HideInInspector] public int tutorialIndex;
    private int isPlayed;
    
    void Awake()
    {
        Instance = this;
        level = PlayerPrefs.GetInt("level", 1);
        reward = PlayerPrefs.GetInt("reward", 0);
        speed = PlayerPrefs.GetFloat("speed", 2.1f);
        health = PlayerPrefs.GetInt("health", 1);
        damage = PlayerPrefs.GetInt("damage", 1);
        isPlayed = PlayerPrefs.GetInt("isPlayed", 0);
        soldiersNumber = PlayerPrefs.GetInt("soldiersNumber", 2);
        UIManager.Instance.SetPanel();
        LoadLevel();
        TinySauce.OnGameStarted(level.ToString());
    }

    private void Update()
    {
        if (gameState == GameState.Start && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space)))
        {
            UIManager.Instance.SetPanel(GameState.Play);
            GameController.Instance.ActivateAgents();
        }
  
    }

    public void Win()
    {
        GameController.Instance.StopAgents();
        GameController.Instance.KillAllEnemies();
        GameController.Instance.SetWinAnimationToAgents();
        gameState = GameState.Win;
        confetti.Play();
        StartCoroutine(EnableWinPanel());
        isPlayed = 1;
        PlayerPrefs.SetInt("isPlayed", isPlayed);
        TinySauce.OnGameFinished(true, 1, level.ToString());
    }

    private IEnumerator EnableWinPanel()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.SetPanel(GameState.Win);
    }

    public void Lose()
    {
        GameController.Instance.StopAgents();
        GameController.Instance.SetWinAnimationToAgents();
        UIManager.Instance.SetPanel(GameState.Lose);
        isPlayed = 1;
        PlayerPrefs.SetInt("isPlayed", isPlayed);
        TinySauce.OnGameFinished(false, 1, level.ToString());
    }

    public void Restart()
    {
        AudioManager.Instance.Vibrate();
        AudioManager.Instance.PlaySound(AudioManager.Instance.click);
        SceneManager.LoadScene(0);
    }

    public void Next()
    {
        AudioManager.Instance.Vibrate();
        AudioManager.Instance.PlaySound(AudioManager.Instance.click);
        level++;
        PlayerPrefs.SetInt("level", level);
        SceneManager.LoadScene(0);
        LoadLevel();
    }

    private void LoadLevel()
    {
        var l = level;
        if (l > levels.Count)
            l = Random.Range(2, levels.Count + 1);

        selectedLevel = levels[l - 1];
        selectedLevel.gameObject.SetActive(true);
        activeRound = 0;

        selectedLevel.rounds.playerTroops[0].agentBase.power = selectedLevel.rounds.playerTroops[0].basePower;
        foreach (var round in selectedLevel.rounds.enemyTroops)
            round.agentBase.power = round.basePower;

        fog.SetColor("_BaseColor", colors[SelectColor(l)]);

        var rounds = selectedLevel.rounds;
        GameController.Instance.UpdateNaveMesh();
        GameController.Instance.SpawnPlayers(rounds.playerTroops[0]);
        GameController.Instance.SpawnEnemies(rounds.enemyTroops[0]);
        if (selectedLevel.hasTutorial) LoadTutorial();
        
        if (isPlayed == 1) UIManager.Instance.SetPanel(GameState.Upgrade);
    }

    private static int SelectColor(int l)
    {
        var index = 0;
        if (l is > 3 and < 7)
            index = 1;
        else if (l is > 6 and < 9)
            index = 2;
        else if (l >= 9) index = 3;

        return index;
    }


    public void UpgradeDamage()
    {
        var dp = damage * 10 + 5;
        if (dp > reward) return;
        damage += 1;
        reward -= dp;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetInt("damage", damage);
        PlayerPrefs.SetInt("reward", reward);
        AudioManager.Instance.PlaySound(AudioManager.Instance.upgrade);
    }

    public void UpgradeSpeed()
    {
        var sp = (int) Mathf.Round(((speed - 2) * 100) + 5);
        if (sp > reward) return;
        speed += 0.1f;
        reward -= sp;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetFloat("speed", speed);
        PlayerPrefs.SetInt("reward", reward);
        AudioManager.Instance.PlaySound(AudioManager.Instance.upgrade);
        GameController.Instance.PlayBlastEffect();
    }

    public void UpgradeHealth()
    {
        var hp = health * 10 + 5;
        if (hp > reward) return;
        health += 1;
        reward -= hp;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetInt("health", health);
        PlayerPrefs.SetInt("reward", reward);
        AudioManager.Instance.PlaySound(AudioManager.Instance.upgrade);
    }

    public void LoadTutorial()
    {
        var tuts = UIManager.Instance.tutorials;

        if (tutorialIndex < tuts.Count) UIManager.Instance.tutorials[tutorialIndex].SetActive(true);
        if (tutorialIndex != 0) UIManager.Instance.tutorials[tutorialIndex - 1].SetActive(false);
        tutorialIndex++;
    }

    public void UpgradeArmy()
    {
        var nbr = soldiersNumber * 10 + 5;
        if (nbr > reward) return;
        soldiersNumber += 1;
        reward -= nbr;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetInt("soldiersNumber", soldiersNumber);
        PlayerPrefs.SetInt("reward", reward);
        AudioManager.Instance.PlaySound(AudioManager.Instance.upgrade);
        GameController.Instance.AddPlayer();
    }

    public void CloseUpgradePanel()
    {
        AudioManager.Instance.Vibrate();
        AudioManager.Instance.PlaySound(AudioManager.Instance.click);
        UIManager.Instance.SetPanel();
    }

    private void OnApplicationQuit()
    {
        PlayerPrefs.SetInt("isPlayed", 0);
    }
}