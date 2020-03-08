using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class WhiteCellObject : ScriptableObject
{
    public Sprite cellSprite;
    public string cellName;

    /// <summary>
    /// Description of this white cells (displayed in the encyclopedia)
    /// </summary>
    [TextArea]
    public string cellDescription;

    /// <summary>
    /// How many bullet per second can we fire (per second)
    /// </summary>
    [Range(1f, 100f)]
    public float cellFireRate = 1f;

    /// <summary>
    /// How much damage does this cell deal per hit
    /// </summary>
    [Range(1f, 100f)]
    public float cellDamage = 1f;

    /// <summary>
    /// How much does it cost to place this cell
    /// </summary>
    [Range(10, 50)]
    public int cellCost = 10;
}
