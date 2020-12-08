/*
 * Author: Matteo
 * Last Modified by: Matteo
 * Date Last Modified: 2020-04-14
 * Program Description: Manages the waypoints for the path that the enemies will be spawned for each levels in Game
 * Revision History:
 *      - Initial Setup
 */

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
