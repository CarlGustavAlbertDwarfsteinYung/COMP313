using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCell : MonoBehaviour
{
    /// <summary>
    /// The speed of the cell moving towards the enemy
    /// </summary>
    public float cellSpeed = 10f;

    /// <summary>
    /// How much damage does this cell contribute
    /// </summary>
    public float cellDamage = 10f;

    /// <summary>
    /// The attack cell SpriteRenderer
    /// </summary>
    private SpriteRenderer _spriteRenderer;

    private Rigidbody2D _rigidbody;

    /// <summary>
    /// Help property to set the attack cell sprite
    /// </summary>
    public Sprite AttackCellSprite
    {
        set => _spriteRenderer.sprite = value;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += transform.right * (Time.deltaTime * cellSpeed);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Virus"))
        {
            other.GetComponent<EnemyNPC>().ReceiveHit(cellDamage);
            Destroy(gameObject);
        }
    }
}
