using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WhiteCellObject : ScriptableObject
{
    public Sprite cellSprite;
    public string cellName;

    [TextArea]
    public string cellDescription;

    /// <summary>
    /// How many bullet per second can we fire
    /// </summary>
    [Range(1, 100)]
    public float cellFireRate;

    [Range(1, 100)]
    public float cellDamage;
}
