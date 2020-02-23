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
    }

    private int _currentWave = 0;
    public int maxNumberOfWaves => waves.Count;

    public Transform spawnZone;
    public List<Wave> waves = new List<Wave>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            SpawnWave(_currentWave++);
        }
    }

    public void SpawnWave(int wave)
    {
        if (wave >= waves.Count)
        {
            return;
        }
        GameController.WaveSpawned();
        StartCoroutine(SpawnWaveEnumarator(wave));
    }

    private IEnumerator SpawnWaveEnumarator(int wave)
    {
        var waveToSpawn = waves[wave];

        for (int index = 0; index < waveToSpawn.enemyPerWave; index++)
        {
            waveToSpawn.enemyType.Spawn(transform, spawnZone, this);
            yield return new WaitForSeconds(waveToSpawn.spawnRate);
        }
    }

    public void RegisterHit()
    {
        GameController.RegisterHit();
    }
}
