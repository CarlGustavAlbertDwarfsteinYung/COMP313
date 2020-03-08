using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCell : MonoBehaviour
{
    /// <summary>
    /// The index of the current waypoint
    /// </summary>
    private int _currentWayPoint = 0;

    /// <summary>
    /// The position of the next waypoint our pathogens should move next
    /// </summary>
    private Vector3 _nextWayPoint;

    /// <summary>
    /// The transform with all the waypoints to follow
    /// </summary>
    public Transform waypointsContainer;

    /// <summary>
    /// The speed of the red cells
    /// </summary>
    public float redCellSpeed = 2f;

    // Start is called before the first frame update
    void Start()
    {
        waypointsContainer = Waypoints.WaypointsTransform;

        // Get all the waypoints
        _nextWayPoint = waypointsContainer.GetChild(_currentWayPoint).position;
    }

    void Update()
    {
        // Check if we reached the end, if yes, skip the rest of the function
        if (_currentWayPoint == waypointsContainer.childCount)
            return;

        // Smoothly move the camera towards that target position
        transform.position = Vector3.MoveTowards(transform.position, _nextWayPoint, redCellSpeed * Time.deltaTime);

        // Check if arrived at destination
        if (_nextWayPoint == transform.position && _currentWayPoint < waypointsContainer.childCount)
        {
            // Check if we arrived at the hearth, if yes destroy this GameObject
            if (_currentWayPoint == waypointsContainer.childCount - 1)
            {
                ++_currentWayPoint;
                Destroy(gameObject);
                return;
            }

            _nextWayPoint = waypointsContainer.GetChild(++_currentWayPoint).position;
        }
    }
}
