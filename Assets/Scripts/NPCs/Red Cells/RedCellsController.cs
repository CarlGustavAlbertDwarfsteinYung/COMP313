using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedCellsController : MonoBehaviour
{
    /// <summary>
    /// In which gameObject should the cell appear
    /// </summary>
    public Transform spawnZone;

    /// <summary>
    /// The prefab for the red cell
    /// </summary>
    public RedCell redCellPrefab;

    /// <summary>
    /// How many red cells we should spawn per second
    /// </summary>
    public float spawnRate = 1f;

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
            Instantiate(redCellPrefab, spawnZone.position, spawnZone.rotation, transform);
            yield return new WaitForSeconds(1f / spawnRate);
        }
    }
}
