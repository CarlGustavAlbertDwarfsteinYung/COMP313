using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static Action onGameOver = () => { };

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

    private void Awake()
    {
        DontDestroyOnLoad(this);

        instance = this;
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
            //LoadingBarProgress.Progress = nextSceneOp.progress;
            yield return null;
        }

        nextSceneOp.allowSceneActivation = true;

        yield return nextSceneOp;

        SceneManager.SetActiveScene(SceneManager.GetSceneByName(nextScene));
        yield return SceneManager.UnloadSceneAsync(currentScene);

        ResetGame();
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

    private static IEnumerator GameOverCounter()
    {
        yield return new WaitForSeconds(5f);
        instance.SwitchScene("Menu");
    }
}
