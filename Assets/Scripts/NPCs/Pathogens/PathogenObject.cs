using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class PathogenObject : ScriptableObject
{
    public Sprite pathogenSprite;
    public string pathogenName;

    [TextArea]
    public string pathogenDescription;

    /// <summary>
    /// How fast does this pathogen move
    /// </summary>
    [Range(1f, 10f)]
    public float pathogenSpeed = 1f;

    /// <summary>
    /// How much life does this pathogen have
    /// </summary>
    [Range(1f, 200f)]
    public float pathogenLife = 100f;

    /// <summary>
    /// How many points do we get every-time we destroy this object
    /// </summary>
    [Range(1, 200)]
    public int pathogenReward = 10;
}
