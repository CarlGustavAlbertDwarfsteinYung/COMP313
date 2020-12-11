/*
 * Author: Matteo
 * Last Modified by: Leslie
 * Date Last Modified: 2020-12-08
 * Program Description: Handles the overall control of the flow and interactions in the Game
 * Revision History:
 *      - Initial Setup
 *      - Added PlayAgain and MainMenu function
 *      - Added SaveLevelStateOnGameEnd and SaveGameLevel function
 */

using System;
using System.Collections;
using System.IO;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class GameController : MonoBehaviour
{
    public static Action onGameOver = () => { };

    public static Action onGameWon = () => { };

    public static Action onLifeLost = () => { };

    public static Action onGameReset = () => { };

    public static Action<int> onCountdownTick = (timeLeft) => { };

    public static Action onWaveSpawned = () => { };

    public static Action onWaveCleared = () => { };

    public static Action onEnemyDestroyed = () => { };

    public static int maxLife = 20;

    public AudioMixer audioMixer;

    public static int currentLife { get; private set; }

    public static int currentWave { get; private set; }

    public static GameController instance;

    private static int final_level = 12;

    /// <summary>
    /// The white cell we selected though the UI
    /// </summary>
    public static WhiteCellObject SelectedWhiteCell { get; set; }

    /// <summary>
    /// How many points we have (used to place towers)
    /// </summary>
    public static int towerPoints { get; set; } = 50;

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

    public AudioSource audioSource;

    private int _savedMaxLevel = 1;
    private bool _hasWon = false;
    private int _enemyPoints = 0;
    public bool hasLost = false;

    static GameController()
    {
        ResetGame();
    }

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(instance);

        SceneManager.activeSceneChanged += SceneManager_activeSceneChanged;
    }

    private void SceneManager_activeSceneChanged(Scene arg0, Scene arg1)
    {
        if (arg1.name == "Game")
        {
            if ( arg1.isLoaded && audioSource == null )
            {
                instance.audioSource = GameObject.FindGameObjectWithTag("soundEffects").GetComponent<AudioSource>();
            }
        }
    }

    private void OnEnable()
    {
        _saveGame.maxUnlockedLevel = PlayerPrefs.GetString("MaxLevelUnlocked", "Level_1");
    }

    private void OnDisable()
    {
        PlayerPrefs.SetString("MaxLevelUnlocked", _saveGame.maxUnlockedLevel);
    }

    private void Start()
    {
        onEnemyDestroyed += () =>
        {
            UIController.score += instance._enemyPoints;

            // we win if all the enemies that are spawned are dead and the health is still greater than 0
            if (PathogensController.enemiesAllSpawned && PathogensController.activeController.AliveEnemiesCount == 0 && currentLife > 0)
            {
                onGameWon();
                _hasWon = true;
                instance.hasLost = false;
            }
            else if (PathogensController.activeController.WaveSpawningComplete && PathogensController.activeController.AliveEnemiesCount == 0)
            {
                onWaveCleared();
            }

            instance._savedMaxLevel = SaveGameLevel();
            SaveLevelStateOnGameEnd(instance._savedMaxLevel);
        };


        instance.SetVolume(-17.0f);
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

    internal void SetEnemyPoints(int pathogenReward)
    {
        instance._enemyPoints = pathogenReward;
    }

    /// <summary>
    /// Sets the level based on the level selector
    /// </summary>
    /// <param name="levelName"></param>
    public void SetLevel(string levelName)
    {
        currentLevel = levelName;

        if ( !(levelName == "Tutorial") && ( string.IsNullOrEmpty(_saveGame.maxUnlockedLevel) || ( String.Compare(_saveGame.maxUnlockedLevel, levelName, StringComparison.Ordinal) < 0 ) ) )
        {
            currentLevel = levelName == "Tutorial" ? "Level_1" : levelName;
        }

        switch(currentLevel)
        {
            case "Tutorial":
            case "Level_1":
                maxLife = 5;
                towerPoints = 20;
            break;

            case "Level_2":
            case "Level_3":
            case "Level_4":
            case "Level_5":
                maxLife = 10;
                towerPoints = 30;
            break;

            case "Level_6":
            case "Level_7":
            case "Level_8":
            case "Level_9":
            case "Level_10":
                maxLife = 10;
                towerPoints = 40;
            break;

            case "Level_11":
            case "Level_12":
                maxLife = 10;
                towerPoints = 50;
            break;

            default:
                towerPoints = 50;
                maxLife = 20;
            break;
        }

        ResetGame();
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

        Time.timeScale = 1f; // Reset Game Speed
    }

    /// <summary>
    /// Resets the game life, wave and tower points
    /// </summary>
    public static void ResetGame()
    {
        currentLife = maxLife;
        currentWave = 0;
        UIController.score = 0;
        onGameReset.Invoke();
    }

    /// <summary>
    /// Increments the number of waves once the wave is spawned
    /// </summary>
    public static void WaveSpawned()
    {
        currentWave++;
        onWaveSpawned();
    }

    /// <summary>
    /// Decreases the life value when the enemy reaches the end point alive
    /// </summary>
    public static void RegisterHit()
    {
        currentLife--;
        onLifeLost.Invoke();

        // we lost
        if (currentLife == 0)
        {
            onGameOver();

            instance.hasLost = true; 
            instance._hasWon = true;
        }
    }

    /// <summary>
    /// Play next level after winning current level
    /// </summary>
    /// <returns></returns>
    private static IEnumerator GameWonCounter()
    {
        yield return new WaitForSeconds(2f);

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
        
        if(currentLevel <= final_level)
        {
            instance.SetLevel("Level_" + currentLevel);
            instance.SwitchScene("Game");
        }
        else
        {
            instance.SwitchScene("Menu");
        }
    }

    /// <summary>
    /// Returns to main menu when you lost game after 5 seconds
    /// </summary>
    private static IEnumerator GameOverCounter()
    {
        yield return new WaitForSeconds(2f);
        instance.SwitchScene("Menu");
    }

    /// <summary>
    /// Used to set the volume of the game
    /// </summary>
    /// <param name="volume"></param>
    public void SetVolume(float volume)
    {
        audioMixer.SetFloat("volume", volume);
    }

    /// <summary>
    /// When 'Exit' is clicked from the UI this exits the game
    /// </summary>
    public void QuitGame()
    {
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
    Application.Quit();
#elif (UNITY_WEBGL)
    Application.OpenURL("about:blank");
#endif
    }

    /// <summary>
    /// Used for saving the game state when exiting the game or going back to main menu
    /// </summary>
    /// <param name="currentLevel"></param>
    public void SaveLevelStateOnGameEnd(int currentLevel)
    {
        string[] saved_level = _saveGame.maxUnlockedLevel.Split('_');
        int parsedLevel = int.Parse(saved_level[1].Trim());

        if (currentLevel < parsedLevel)
        {
            return;
        }
        
        if( _hasWon )
        {
            currentLevel = instance._savedMaxLevel < final_level ? parsedLevel + 1 : parsedLevel;

            _saveGame.maxUnlockedLevel = "Level_" + currentLevel;
            PlayerPrefs.SetString("MaxLevelUnlocked", _saveGame.maxUnlockedLevel);
        }
    }


    public int SaveGameLevel()
    {
        return Int32.Parse(GameController.currentLevel.Substring(GameController.currentLevel.LastIndexOf("_", StringComparison.Ordinal) + 1));
    }

    /// <summary>
    /// On GameOver/GameWon Scene, play the same level once the Play Again button is clicked
    /// </summary>
    public void PlayAgain()
    {
        if (instance._savedMaxLevel <= final_level)
        {
            instance.SetLevel("Level_" + instance._savedMaxLevel);
            instance.SwitchScene("Game");
        }
    }

    /// <summary>
    /// On GameOver/GameWon Scene, open the Menu scene once the Main Menu button is clicked
    /// </summary>
    public void MainMenu()
    {
        instance.SwitchScene("Menu");
    }

    public void PlaySoundEffects(AudioClip audioClip)
    {
        if( audioSource != null )
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
    }
}
