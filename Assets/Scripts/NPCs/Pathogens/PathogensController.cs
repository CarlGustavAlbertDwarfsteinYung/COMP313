using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathogensController : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public EnemyNPC enemyType;
        public int enemyPerWave;
        public float spawnRate;

        /// <summary>
        /// Time until the next wave get's triggered
        /// </summary>
        public float timeToNextWave = 25f;
    }

    private int _currentWave = 0;
    public int maxNumberOfWaves => waves.Count;

    public Transform spawnZone;
    public List<Wave> waves = new List<Wave>();
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.U))
            SpawnWave(_currentWave++);
    }

    public void PlayNextWave()
    {
        SpawnWave(_currentWave++);
    }

    public void SpawnWave(int wave)
    {
        if (wave >= waves.Count)
            return;

        GameController.WaveSpawned();
        StartCoroutine(SpawnWaveEnumerator(wave));
        StartCoroutine(CountdownToNextWave(wave));
    }

    private IEnumerator SpawnWaveEnumerator(int wave)
    {
        var waveToSpawn = waves[wave];

        for (int index = 0; index < waveToSpawn.enemyPerWave; index++)
        {
            waveToSpawn.enemyType.Spawn(transform, spawnZone, this);
            yield return new WaitForSeconds(waveToSpawn.spawnRate);
        }
    }

    private IEnumerator CountdownToNextWave(int wave)
    {
        yield return new WaitForSeconds(waves[wave].timeToNextWave);

        PlayNextWave();
    }

    public void RegisterHit()
    {
        GameController.RegisterHit();
    }
}
