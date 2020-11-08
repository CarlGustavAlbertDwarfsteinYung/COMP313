using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static Action onGameOver = () => { };

    public static Action onGameWon = () => { };

    public static Action onLifeLost = () => { };

    public static Action onGameReset = () => { };

    public static Action onWaveSpawned = () => { };

    public static Action onEnemyDestroyed = () => { };

    public static int maxLife = 20;
    public static int maxWave = 10;
    
    public static int currentLife { get; private set; }

    public static int currentWave { get; private set; }

    private static GameController instance;

    /// <summary>
    /// The white cell we selected though the UI
    /// </summary>
    public static WhiteCellObject SelectedWhiteCell { get; set; }

    /// <summary>
    /// How many points we have (used to place towers)
    /// </summary>
    public static int towerPoints { get; set; } = 100;

    /// <summary>
    /// The current level
    /// </summary>
    public static string currentLevel { get; private set; }

    /// <summary>
    /// The furthest level unlocked
    /// </summary>
    public static string maxUnlockedLevel => instance._saveGame.maxUnlockedLevel;

    /// <summary>
    /// The save game data
    /// </summary>
    private SaveGameData _saveGame = new SaveGameData();

    static GameController()
    {
        ResetGame();
    }
    
    private void Awake()
    {
        DontDestroyOnLoad(this);

        instance = this;
    }

    private void OnEnable()
    {
        LoadGameData(Application.persistentDataPath + "save.pack");
    }

    private void OnDisable()
    {
        SaveGameData(Application.persistentDataPath + "save.pack");
    }

    private void Start()
    {
        onEnemyDestroyed += () =>
        {
            Debug.Log($"enemiesAllSpawned: ${PathogensController.enemiesAllSpawned}");
            Debug.Log($"EnemyNPC.AliveEniemiesCount: ${PathogensController.activeController.AliveEniemiesCount}");

            if (PathogensController.enemiesAllSpawned && PathogensController.activeController.AliveEniemiesCount == 0 && currentLife > 0)
            {
                Debug.Log("We won!");
                onGameWon();
                instance.StartCoroutine(GameWonCounter());
            }
        };
    }

    private void SaveGameData(string path)
    {
        var saveGameText = JsonUtility.ToJson(_saveGame);

        File.WriteAllText(path, saveGameText);
    }

    private void LoadGameData(string path)
    {
        if (!File.Exists(path))
            return;

        var saveGameText = File.ReadAllText(path);

        _saveGame = JsonUtility.FromJson<SaveGameData>(saveGameText);
    }

    public void SetLevel(string levelName)
    {
        currentLevel = levelName;

        if (levelName == "Tutorial")
        {
            _saveGame.maxUnlockedLevel = "Level_1";
            maxLife = 5;
        }
        else if (string.IsNullOrEmpty(_saveGame.maxUnlockedLevel) || String.Compare(_saveGame.maxUnlockedLevel, levelName, StringComparison.Ordinal) < 0)
        {
            maxLife = 20;
            _saveGame.maxUnlockedLevel = levelName == "Tutorial" ? "Level_1" : levelName;
        }
    }

    public void SwitchScene(string nextScene)
    {
        StartCoroutine(SwitchSceneCoroutine(nextScene));
    }

    private IEnumerator SwitchSceneCoroutine(string nextScene)
    {
        var currentScene = SceneManager.GetActiveScene();
        var nextSceneOp = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        nextSceneOp.allowSceneActivation = false;
        
        while (nextSceneOp.progress < 0.9f)
        {
            yield return null;
        }

        nextSceneOp.allowSceneActivation = true;

        yield return nextSceneOp;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
        yield return SceneManager.UnloadSceneAsync(currentScene);

        ResetGame();

        Time.timeScale = 1f;    // Reset Game Speed
    }

    public static void ResetGame()
    {
        currentLife = maxLife;
        currentWave = 0;
        towerPoints = 100;
        onGameReset.Invoke();
    }

    public static void WaveSpawned()
    {
        currentWave++;
        onWaveSpawned();
    }

    public static void RegisterHit()
    {
        currentLife--;
        onLifeLost.Invoke();

        // we lost
        if (currentLife == 0)
        {
            onGameOver();
            instance.StartCoroutine(GameOverCounter());
        }
    }

    private static IEnumerator GameWonCounter()
    {
        yield return new WaitForSeconds(5f);

        int currentLevel = 0;

        if (GameController.currentLevel == "Tutorial")
        {
            currentLevel = 2;
        }
        else
        {
            currentLevel = Int32.Parse(GameController.currentLevel.Substring(GameController.currentLevel.LastIndexOf("_", StringComparison.Ordinal) + 1));
            currentLevel++;
        }
        

        instance.SetLevel("Level_" + currentLevel);
        instance.SwitchScene("Game");
    }

    private static IEnumerator GameOverCounter()
    {
        yield return new WaitForSeconds(5f);
        instance.SwitchScene("Menu");
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
