using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;


public class PathogensController : MonoBehaviour
{
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

    public int AliveEniemiesCount { get; private set; } = 0;
    
    public int maxNumberOfWaves => waves.Count;
    
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
    
    /// <summary>
    /// Assign the active pathogen controller
    /// </summary>
    private void OnEnable()
    {
        activeController = this;

        AliveEniemiesCount = 0;
    }

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
        
        AliveEniemiesCount = waveToSpawn.enemyPerWave;

        enemiesAllSpawned = false;

        for (int index = 0; index < waveToSpawn.enemyPerWave; index++)
        {
            SpawnEnemy(transform, waveToSpawn.waveWaypoints.SpawnPoint, this, waveToSpawn.enemyType, waveToSpawn.waveWaypoints);

            if (wave == waves.Count - 1 && index == waveToSpawn.enemyPerWave - 1)
            {
                Debug.Log("We spawned all the enemies");
                enemiesAllSpawned = true;
            }

            yield return new WaitForSeconds(1f / waveToSpawn.spawnRate);
        }
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

        // Execute the spawning callback
        newEnemy.onEnemySpawned?.Invoke();
        newEnemy.onEnemyDestroyed.AddListener( () => --AliveEniemiesCount);
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
