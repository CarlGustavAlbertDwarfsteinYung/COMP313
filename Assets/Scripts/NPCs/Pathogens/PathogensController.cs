﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PathogensController : MonoBehaviour
{
    [Serializable]
    public class Wave
    {
        public PathogenObject enemyType;
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

    /// <summary>
    /// The prefab for the pathogen
    /// </summary>
    public EnemyNPC pathogenPrefab;
    
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
            SpawnEnemy(transform, spawnZone, this, waveToSpawn.enemyType);
            yield return new WaitForSeconds(waveToSpawn.spawnRate);
        }
    }

    /// <summary>
    /// Spawn an enemy NPC type
    /// </summary>
    /// <param name="parent">The parent GameObject where this gameObject should be spawned</param>
    /// <param name="spawnPoint">Where in the World this gameObject should spawn</param>
    /// <param name="pathogensController">The PathogensController used to control the NPC</param>
    /// <param name="pathogenObject">The data of the pathogen we are spawining</param>
    public void SpawnEnemy(Transform parent, Transform spawnPoint, PathogensController pathogensController, PathogenObject pathogenObject)
    {
        var newEnemy = Instantiate(pathogenPrefab, spawnPoint.position, spawnPoint.rotation, parent);

        newEnemy.controller = pathogensController;
        newEnemy.pathogenObject = pathogenObject;

        // Execute the spawning callback
        newEnemy.onEnemySpawned?.Invoke();
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
