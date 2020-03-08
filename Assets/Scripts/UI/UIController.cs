using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI lifeText;
    public TextMeshProUGUI waveText;
    public Image endScreen;

    // Start is called before the first frame update
    void Start()
    {
        endScreen.gameObject.SetActive(false);
        lifeText.text = $"{GameController.maxLife}";
        waveText.text = $"{GameController.currentWave}";

        GameController.onLifeLost += () => lifeText.text = $"{GameController.currentLife}";
        GameController.onWaveSpawned += () => waveText.text = $"{GameController.currentWave}";
        GameController.onGameOver += () => endScreen.gameObject.SetActive(true);
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
}
