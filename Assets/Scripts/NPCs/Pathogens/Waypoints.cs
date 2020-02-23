using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    private static Transform _waypointsTransform;

    public static Transform WaypointsTransform
    {
        get
        {
            if (_waypointsTransform != null)
                return _waypointsTransform;

            _waypointsTransform = GameObject.Find("Waypoints").transform;
            return _waypointsTransform;
        }
    }
}
