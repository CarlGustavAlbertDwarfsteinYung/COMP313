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
    public Image endScreen;
    public Image wonScreen;
    public Button nextWaveButton;

    private float gameSpeed;

    // Start is called before the first frame update
    void Start()
    {
        // Enables/Disable the Tutorial
        var tutorialSequence = transform.Find("TutorialSequence").gameObject;
        tutorialSequence.SetActive(GameController.currentLevel == "Tutorial");
        
        endScreen.gameObject.SetActive(false);
        wonScreen.gameObject.SetActive(false);
        lifeText.text = $"{GameController.maxLife}";
        waveText.text = $"{GameController.currentWave}";
        dnaText.text = $"{GameController.towerPoints}";

        nextWaveButton.onClick.AddListener(PathogensController.activeController.PlayNextWave);
    }

    private void OnEnable()
    {
        GameController.onLifeLost += UpdateLifeText;
        GameController.onWaveSpawned += UpdateWaveText;
        
        GameController.onGameOver += ShowGameOverScreen;
        GameController.onGameWon += ShowGameWonScreen;

        TowerController.onTowerPlaced += UpdateDnaText;
        GameController.onEnemyDestroyed += UpdateDnaText;
    }
    
    private void OnDisable()
    {
        GameController.onLifeLost -= UpdateLifeText;
        GameController.onWaveSpawned -= UpdateWaveText;
        
        GameController.onGameOver -= ShowGameOverScreen;
        GameController.onGameWon -= ShowGameWonScreen;

        TowerController.onTowerPlaced -= UpdateDnaText;
        GameController.onEnemyDestroyed -= UpdateDnaText;
    }

    private void ShowGameOverScreen() => endScreen.gameObject.SetActive(true);
    
    private void ShowGameWonScreen() => wonScreen.gameObject.SetActive(true);

    private void UpdateWaveText() => waveText.text = $"{GameController.currentWave}";

    private void UpdateLifeText() => lifeText.text = $"{GameController.currentLife}";

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

    public void UpdateDnaText()
    {
        dnaText.text = $"{GameController.towerPoints}";
    }
}
