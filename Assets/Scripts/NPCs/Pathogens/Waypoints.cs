using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    /// <summary>
    /// The point where all the NPC will spawn (read-only)
    /// </summary>
    public Transform SpawnPoint => transform.GetChild(0);

    /// <summary>
    /// The point where all the NPC will get de-spawned (read-only)
    /// </summary>
    public Transform EndPoint => transform.GetChild(transform.childCount - 1);
}
