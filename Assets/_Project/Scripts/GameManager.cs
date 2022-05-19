using System.Collections;
using System.Collections.Generic;
using Beebyte.Obfuscator;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

public enum GameState
{
    Start,
    Build,
    Play,
    Lose,
    Win
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; protected set; }
    public static GameState gameState = GameState.Start;
    public ParticleSystem confetti;
    public int health, damage;
    public float speed;
    public List<Color32> operatorColors = new List<Color32>();
    public List<Color32> colors = new List<Color32>();
    public List<Level> levels = new List<Level>();
    [HideInInspector] public Level selectedLevel;
    [HideInInspector] public int level, reward;
    [HideInInspector] public int activeRound;
    [HideInInspector] public int tutorialIndex;

    void Awake()
    {
        Instance = this;
        level = PlayerPrefs.GetInt("level", 1);
        reward = PlayerPrefs.GetInt("reward", 0);
        speed = PlayerPrefs.GetFloat("speed", 2.1f);
        health = PlayerPrefs.GetInt("health", 1);
        damage = PlayerPrefs.GetInt("damage", 1);
        UIManager.Instance.SetPanel(GameState.Play);
        LoadLevel();
        TinySauce.OnGameStarted(level.ToString());
    }


    public void Win()
    {
        GameController.Instance.StopAgents();
        GameController.Instance.KillAllEnemies();
        GameController.Instance.SetWinAnimationToAgents();
        gameState = GameState.Win;
        confetti.Play();
        StartCoroutine(EnableWinPanel());
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
        TinySauce.OnGameFinished(false, 1, level.ToString());
    }

    [SkipRename]
    public void Restart()
    {
        AudioManager.Instance.Vibrate();
        AudioManager.Instance.PlaySound(AudioManager.Instance.click);
        SceneManager.LoadScene(0);
    }

    [SkipRename]
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

        if (selectedLevel.hasTutorial) LoadTutorial();
    }

    [SkipRename]
    public void UpgradeDamage()
    {
        var dp = damage * 10 + 5;
        if (dp > reward) return;
        damage += 1;
        reward -= dp;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetInt("damage", damage);
        PlayerPrefs.SetInt("reward", reward);
    }

    [SkipRename]
    public void UpgradeSpeed()
    {
        var sp = (int) Mathf.Round(((speed - 2) * 100) + 5);
        if (sp > reward) return;
        speed += 0.1f;
        reward -= sp;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetFloat("speed", speed);
        PlayerPrefs.SetInt("reward", reward);
    }

    [SkipRename]
    public void UpgradeHealth()
    {
        var hp = health * 10 + 5;
        if (hp > reward) return;
        health += 1;
        reward -= hp;
        UIManager.Instance.SetUpgradePrices();
        PlayerPrefs.SetInt("health", health);
        PlayerPrefs.SetInt("reward", reward);
    }

    public void LoadTutorial()
    {
        var tuts = UIManager.Instance.tutorials;

        if (tutorialIndex < tuts.Count) UIManager.Instance.tutorials[tutorialIndex].SetActive(true);
        if (tutorialIndex != 0) UIManager.Instance.tutorials[tutorialIndex - 1].SetActive(false);
        tutorialIndex++;
    }
}