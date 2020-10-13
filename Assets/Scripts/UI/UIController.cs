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

        GameController.onLifeLost += () => lifeText.text = $"{GameController.currentLife}";
        GameController.onWaveSpawned += () => waveText.text = $"{GameController.currentWave}";
        
        GameController.onGameOver += () => endScreen.gameObject.SetActive(true);
        GameController.onGameWon += () => wonScreen.gameObject.SetActive(true);

        TowerController.onTowerPlaced += UpdateDnaText;
        GameController.onEnemyDestroyed += UpdateDnaText;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TogglePause()
    {
        Time.timeScale = Time.timeScale == 1f ? 0f : 1f;
    }

    public void ToggleSpeed(float newSpeed)
    {
        Time.timeScale = newSpeed;
    }

    public void UpdateDnaText()
    {
        dnaText.text = $"{GameController.towerPoints}";
    }
}
