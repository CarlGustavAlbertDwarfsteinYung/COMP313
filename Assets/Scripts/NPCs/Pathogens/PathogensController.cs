/*
 * Author: Matteo
 * Last Modified by: Leslie
 * Date Last Modified: 2020-12-08
 * Program Description: Controller for the Pathogens that are spawned in game
 * Revision History:
 *      - Initial Setup
 *      - Added EnemyPoints which is used to update the score in UIController
 */

using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public class PathogensController : MonoBehaviour
{
    /// <summary>
    /// Coroutine for the next wave countdown
    /// </summary>
    private Coroutine _countDownCoroutine;
    
    /// <summary>
    /// Class used to store and represent a wave data
    /// </summary>
    [Serializable]
    public class WaveData
    {
        [AssetsOnly]
        [PreviewField(ObjectFieldAlignment.Center)]
        [Title("@ enemyType ? enemyType.pathogenName : \"Select a pathogen!\"", HorizontalLine = false, TitleAlignment = TitleAlignments.Centered)]
        [TableColumnWidth(100, false)]
#if UNITY_EDITOR
        [OnInspectorGUI(nameof(DrawPreview))]
#endif
        public PathogenObject enemyType;

        [VerticalGroup("Wave info")]
        [BoxGroup("Wave info/Spawn info")]
        [PropertyRange(1, 100)]
        public int enemyPerWave = 1;

        [VerticalGroup("Wave info")]
        [BoxGroup("Wave info/Spawn info")]
        [PropertyTooltip("How many enemies should be spawned per second?")]
        [PropertyRange(1, 10)]
        [SuffixLabel("/second")]
        public int spawnRate = 1;

        /// <summary>
        /// Which waypoint container should we use?
        /// </summary>
        [VerticalGroup("Wave info")]
        [BoxGroup("Wave info/Spawn pos")]
        [SceneObjectsOnly]
        public Waypoints waveWaypoints;

        /// <summary>
        /// Time until the next wave get's triggered
        /// </summary>
        [VerticalGroup("Wave info")]
        [BoxGroup("Wave info/Time to next wave")]
        [SuffixLabel("seconds", true)]
        public float timeToNextWave = 25f;

#if UNITY_EDITOR
        private void DrawPreview()
        {
            if (enemyType == null)
                return;

            var texture = AssetPreview.GetAssetPreview(enemyType.pathogenSprite);
            
            GUILayout.BeginVertical();
            GUILayout.Label(texture);
            GUILayout.EndVertical();
        }
#endif
    }

    private int _currentWave = 0;

    public int AliveEnemiesCount { get; private set; } = 0;
    
    public int MAXNumberOfWaves => waves.Count;

    public static int EnemyPoints = 10;
    
    public bool WaveSpawningComplete { get; private set; } = false;
    
    /// <summary>
    /// List of all the waves per level
    /// </summary>
    [TableList]
    public List<WaveData> waves = new List<WaveData>();

    /// <summary>
    /// The prefab for the pathogen
    /// </summary>
    public EnemyNPC pathogenPrefab;

    /// <summary>
    /// The active pathogen controller
    /// </summary>
    public static PathogensController activeController { get; private set; }

    public static bool enemiesAllSpawned { get; private set; }

    private bool clearedWaves = false;
    private bool spawnOnceOnStart = true;
    
    /// <summary>
    /// Assign the active pathogen controller
    /// </summary>
    private void OnEnable()
    {
        activeController = this;

        AliveEnemiesCount = 0;

        GameController.onWaveCleared += () =>
        {
            if (_countDownCoroutine != null)
                activeController.StopCoroutine(_countDownCoroutine);

            clearedWaves = true;
            activeController.StartCoroutine(CountdownToNextWave(4f));
        };
    }

    public void PlayNextWave()
    {
        if ( clearedWaves && !spawnOnceOnStart )
        {
            SpawnWave(_currentWave++);
            clearedWaves = false; // reset the cleared waves
        }

        if( spawnOnceOnStart ) // used to trigger the initial spawn
        {
            SpawnWave(_currentWave++);
            spawnOnceOnStart = false;
        }
    }

    public void SpawnWave(int wave)
    {
        if (wave >= waves.Count)
            return;
        
        StartCoroutine(SpawnWaveEnumerator(wave));
        _countDownCoroutine = StartCoroutine(CountdownToNextWave(wave));
    }

    private IEnumerator SpawnWaveEnumerator(int wave)
    {
        var waveToSpawn = waves[wave];
        
        AliveEnemiesCount += waveToSpawn.enemyPerWave;

        enemiesAllSpawned = false;
        
        GameController.WaveSpawned();

        WaveSpawningComplete = false;

        for (int index = 0; index < waveToSpawn.enemyPerWave; index++)
        {
            yield return new WaitForSeconds(1f / waveToSpawn.spawnRate);
            
            SpawnEnemy(transform, waveToSpawn.waveWaypoints.SpawnPoint, this, waveToSpawn.enemyType, waveToSpawn.waveWaypoints);

            if (wave == waves.Count - 1 && index == waveToSpawn.enemyPerWave - 1)
            {
                enemiesAllSpawned = true;
            }
        }

        WaveSpawningComplete = true;
    }

    /// <summary>
    /// Spawn an enemy NPC type
    /// </summary>
    /// <param name="parent">The parent GameObject where this gameObject should be spawned</param>
    /// <param name="spawnPoint">Where in the World this gameObject should spawn</param>
    /// <param name="pathogensController">The PathogensController used to control the NPC</param>
    /// <param name="pathogenObject">The data of the pathogen we are spawining</param>
    /// <param name="waypoints">The list of waypoints the cell should follow</param>
    public void SpawnEnemy(Transform parent, Transform spawnPoint, PathogensController pathogensController, PathogenObject pathogenObject, Waypoints waypoints)
    {
        var newEnemy = Instantiate(pathogenPrefab, spawnPoint.position, spawnPoint.rotation, parent);

        newEnemy.controller = pathogensController;
        newEnemy.pathogenObject = pathogenObject;
        newEnemy.waypointsContainer = waypoints.transform;

        EnemyPoints = newEnemy.pathogenObject.pathogenReward;

        // Execute the spawning callback
        newEnemy.onEnemySpawned?.Invoke();
        newEnemy.onEnemyDestroyed.AddListener( () => --AliveEnemiesCount);
    }

    private IEnumerator CountdownToNextWave(int wave)
    {
        GameController.onCountdownTick(-1);
        
        for (var timeToNextWave = waves[wave].timeToNextWave; timeToNextWave > 0f; timeToNextWave -= 1f)
        {
            if ( timeToNextWave < 5 )
                GameController.onCountdownTick((int) timeToNextWave);
            
            yield return new WaitForSeconds(1f);
        }

        if(clearedWaves)
            PlayNextWave();
    }
    
    private IEnumerator CountdownToNextWave(float timeLeft)
    {
        for (var timeToNextWave = timeLeft; timeToNextWave > 0f; timeToNextWave -= 1f)
        {
            GameController.onCountdownTick((int) timeToNextWave);
            
            yield return new WaitForSeconds(1f);
        }

        PlayNextWave();
    }

    public void RegisterHit()
    {
        GameController.RegisterHit();
    }
}
