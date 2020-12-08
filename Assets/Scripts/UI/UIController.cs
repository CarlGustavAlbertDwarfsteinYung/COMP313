/*
 * Author: Matteo
 * Last Modified by: Leslie
 * Date Last Modified: 2020-12-08
 * Program Description: Handles the overall control of the UI display in Game
 * Revision History:
 *      - Initial Setup
 *      - Added Score
 *      - Displayed the DNA/towerPoints on the UI again
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI dnaText;
    public TextMeshProUGUI enemiesLeftText;
    public TextMeshProUGUI timeToNextWaveText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI youLostScoreText;
    public TextMeshProUGUI youWonScoreText;
    public Image endScreen;
    public Image wonScreen;
    public Button nextWaveButton;

    private float gameSpeed;
    private int score = 0;

    // Start is called before the first frame update
    void Start()
    {
        // Enables/Disables the Tutorial
        var tutorialSequence = transform.Find("TutorialSequence").gameObject;
        tutorialSequence.SetActive(GameController.currentLevel == "Tutorial");

        ResetUI();
        
        nextWaveButton.onClick.AddListener(PathogensController.activeController.PlayNextWave);
    }

    /// <summary>
    /// Resets the information on the game UI
    /// </summary>
    public void ResetUI()
    {
        endScreen.gameObject.SetActive(false);
        wonScreen.gameObject.SetActive(false);

        UpdateTimeToNextWave(-1);
        UpdateLifeText();
        UpdateDnaText();
        UpdateWaveText();
        UpdateEnemiesCount();
    }

    private void OnEnable()
    {
        GameController.onLifeLost += UpdateLifeText;
        
        GameController.onWaveSpawned += UpdateWaveText;
        GameController.onWaveSpawned += UpdateEnemiesCount;
        
        GameController.onGameOver += ShowGameOverScreen;
        GameController.onGameWon += ShowGameWonScreen;
        
        GameController.onCountdownTick += UpdateTimeToNextWave;

        TowerController.onTowerPlaced += UpdateDnaText;
        GameController.onEnemyDestroyed += UpdateDnaText;
        GameController.onEnemyDestroyed += UpdateEnemiesCount;
        GameController.onEnemyDestroyed += UpdateScore;
        
        GameController.onGameReset += ResetUI;
    }
    
    private void OnDisable()
    {
        GameController.onLifeLost -= UpdateLifeText;
        
        GameController.onWaveSpawned -= UpdateWaveText;
        GameController.onWaveSpawned -= UpdateEnemiesCount;
        
        GameController.onGameOver -= ShowGameOverScreen;
        GameController.onGameWon -= ShowGameWonScreen;

        TowerController.onTowerPlaced -= UpdateDnaText;
        GameController.onEnemyDestroyed -= UpdateDnaText;
        GameController.onEnemyDestroyed -= UpdateEnemiesCount;
        
        GameController.onCountdownTick -= UpdateTimeToNextWave;
        
        GameController.onGameReset -= ResetUI;
    }

    private void ShowGameOverScreen() => endScreen.gameObject.SetActive(true);

    private void ShowGameWonScreen() => wonScreen.gameObject.SetActive(true);

    private void UpdateWaveText() => waveText.text = $"{ PathogensController.activeController.MAXNumberOfWaves - (GameController.currentWave) }";
    private void UpdateEnemiesCount() => enemiesLeftText.text = $"{ PathogensController.activeController.AliveEnemiesCount }";
    private void UpdateScore()
    {
        score += PathogensController.EnemyPoints;
        scoreText.text = $"{ score }";
        youWonScoreText.text = "Score: " + $"{ score }";
        youLostScoreText.text = "Score: " + $"{ score }";
    }

    private void UpdateTimeToNextWave(int timeLeft) => timeToNextWaveText.text = timeLeft > -1 ? $"{ timeLeft }s" : "";
    private void UpdateLifeText() => lifeText.text = $"{ GameController.currentLife }";

    private void UpdateDnaText() => dnaText.text = $"{ GameController.towerPoints }";

    public void TogglePause()
    {
        if (Time.timeScale != 0)
        {
            // Pause the game
            gameSpeed = Time.timeScale;
            Time.timeScale = 0f;
        }
        else
        {
            // Resume the game
            Time.timeScale = gameSpeed;
        }
    }

    public void ToggleSpeed(float newSpeed)
    {
        if (Time.timeScale != 0)
        {
            // During Playing
            Time.timeScale = newSpeed;
        }
        else
        {
            // During Paused
            gameSpeed = newSpeed;
        }
    }

    public void PlayAgain()
    {
        GameController.instance.PlayAgain();
    }

    public void MainMenu()
    {
        GameController.instance.MainMenu();
    }
}
