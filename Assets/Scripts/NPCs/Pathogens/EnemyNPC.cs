using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[DisallowMultipleComponent]
public class EnemyNPC : MonoBehaviour
{
    public PathogensController controller;

    private int _currentWayPoint = 0;
    private Vector3 _nextWayPoint;

    private Transform[] _waypoints;
    public Transform waypointsContainer;

    public float enemySpeed = 1f;

    /// <summary>
    /// Callback called when the Enemy gets first spawned
    /// </summary>
    public UnityEvent onEnemySpawned;

    /// <summary>
    /// Callback called when the Enemy gets damaged by a tower
    /// </summary>
    public UnityEvent onEnemyDamaged;

    /// <summary>
    /// Callback called when the Enemy gets killed
    /// </summary>
    public UnityEvent onEnemyDestroyed;

    /// <summary>
    /// Callback called when the Enemy reaches the end and attack the hearth
    /// </summary>
    public UnityEvent onEnemyAttackHearth;

    // Start is called before the first frame update
    void Start()
    {
        waypointsContainer = Waypoints.WaypointsTransform;

        // Get all the waypoints
        _nextWayPoint = waypointsContainer.GetChild(_currentWayPoint).position;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we reached the end, if yes, skip the rest of the function
        if (_currentWayPoint == waypointsContainer.childCount)
            return;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.MoveTowards(transform.position, _nextWayPoint, enemySpeed * Time.deltaTime);

        // Check if arrived at destination
        if (_nextWayPoint == transform.position && _currentWayPoint < waypointsContainer.childCount)
        {
            // Check if we arrived at the hearth, if yes destroy this GameObject
            if (_currentWayPoint == waypointsContainer.childCount - 1)
            {
                onEnemyAttackHearth.Invoke();
                Destroy(gameObject, 2f);
                ++_currentWayPoint;
                controller.RegisterHit();
                return;
            }

            _nextWayPoint = waypointsContainer.GetChild(++_currentWayPoint).position;
        }
    }

    /// <summary>
    /// Spawn an enemy NPC type
    /// </summary>
    /// <param name="parent">The parent GameObject where this gameObject should be spawned</param>
    /// <param name="spawnPoint">Where in the World this gameObject should spawn</param>
    /// <param name="pathogensController">The PathogensController used to control the NPC</param>
    public void Spawn(Transform parent, Transform spawnPoint, PathogensController pathogensController)
    {
        var newEnemy = Instantiate(gameObject, spawnPoint.position, spawnPoint.rotation, parent);
        var enemyNpcComponent = newEnemy.GetComponent<EnemyNPC>();

        enemyNpcComponent.controller = pathogensController;

        // Execute the spawning callback
        enemyNpcComponent.onEnemySpawned?.Invoke();
    }
}
