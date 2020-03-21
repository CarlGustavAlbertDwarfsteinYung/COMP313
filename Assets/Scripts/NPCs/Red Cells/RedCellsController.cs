using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCellsController : MonoBehaviour
{
    /// <summary>
    /// The prefab for the red cell
    /// </summary>
    public RedCell redCellPrefab;

    /// <summary>
    /// How many red cells we should spawn per second
    /// </summary>
    public float spawnRate = 1f;

    /// <summary>
    /// List of all the possible directions for the red cells
    /// </summary>
    public List<Waypoints> waypointsList;

    private void OnEnable()
    {
        StartCoroutine(SpawnWaveEnumerator());
    }

    private void OnDisable()
    {
        StopCoroutine(SpawnWaveEnumerator());
    }

    private IEnumerator SpawnWaveEnumerator()
    {
        while (true)
        {
            foreach (var waypoints in waypointsList)
            {
                var newRedCell = Instantiate(redCellPrefab, waypoints.SpawnPoint.position, waypoints.SpawnPoint.rotation, transform);
                newRedCell.waypointsContainer = waypoints.transform;
            }
            
            yield return new WaitForSeconds(1f / spawnRate);
        }
    }
}
