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
    
    /// <summary>
    /// The current enemy life
    /// </summary>
    [SerializeField]
    private float _currentLife = 100f;

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

    /// <summary>
    /// What kind of pathogen does this element contain
    /// </summary>
    public PathogenObject pathogenObject;

    /// <summary>
    /// The sprite of the pathogen to spawn
    /// </summary>
    private SpriteRenderer _pathogenSprite;

    private void Awake()
    {
        _pathogenSprite = transform.Find("Sprite").GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start()
    {
        waypointsContainer = Waypoints.WaypointsTransform;

        // Get all the waypoints
        _nextWayPoint = waypointsContainer.GetChild(_currentWayPoint).position;

        _pathogenSprite.sprite = pathogenObject.pathogenSprite;

        // Set the right amount of hit points on the pathogen
        _currentLife = pathogenObject.pathogenLife;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if we reached the end, if yes, skip the rest of the function
        if (_currentWayPoint == waypointsContainer.childCount)
            return;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.MoveTowards(transform.position, _nextWayPoint, pathogenObject.pathogenSpeed * Time.deltaTime);

        // Check if arrived at destination
        if (_nextWayPoint == transform.position && _currentWayPoint < waypointsContainer.childCount)
        {
            // Check if we arrived at the hearth, if yes destroy this GameObject
            if (_currentWayPoint == waypointsContainer.childCount - 1)
            {
                onEnemyAttackHearth.Invoke();
                KillEnemy();
                ++_currentWayPoint;
                controller.RegisterHit();
                return;
            }

            _nextWayPoint = waypointsContainer.GetChild(++_currentWayPoint).position;
        }
    }
    
    public void ReceiveHit(float damageAmount)
    {
        _currentLife -= damageAmount;
        onEnemyDamaged?.Invoke();

        if (_currentLife <= 0)
            KillEnemy();
    }
    
    private void KillEnemy()
    {
        Debug.Log("Enemy killed");
        GetComponent<Collider2D>().enabled = false;
        onEnemyDestroyed?.Invoke();
        Destroy(gameObject, 2f);
    }
}
