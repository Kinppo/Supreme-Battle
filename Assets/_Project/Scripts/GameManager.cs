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
    public List<Color32> operatorColors = new List<Color32>();
    public List<Color32> colors = new List<Color32>();
    public List<Level> levels = new List<Level>();
    [HideInInspector] public Level selectedLevel;
    [HideInInspector] public int level, reward;
    [HideInInspector] public int activeRound;

    void Awake()
    {
        Instance = this;
        level = PlayerPrefs.GetInt("level", 1);
        UIManager.Instance.SetPanel(GameState.Play);
        LoadLevel();
        //TinySauce.OnGameStarted(level.ToString());
    }


    public void Win()
    {
        gameState = GameState.Win;
        confetti.Play();
        StartCoroutine(EnableWinPanel());
        //TinySauce.OnGameFinished(true, 1, level.ToString());
    }

    private IEnumerator EnableWinPanel()
    {
        yield return new WaitForSeconds(1.5f);
        UIManager.Instance.SetPanel(GameState.Win);
    }

    public void Lose()
    {
        UIManager.Instance.SetPanel(GameState.Lose);
        //TinySauce.OnGameFinished(false, 1, level.ToString());
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
    }
}