using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    private static Color fullyTransparent = new Color(1, 1, 1, 0);
    private static Color halfTransparent = new Color(1, 1, 1, 0.5f);

    /// <summary>
    /// The sprite renderer for the White Cell
    /// </summary>
    private SpriteRenderer _cellRenderer;
    
    /// <summary>
    /// The sprite renderer for the Range sprite
    /// </summary>
    private SpriteRenderer _rangeRenderer;

    /// <summary>
    /// The sprite renderer for the Gun sprite
    /// </summary>
    private SpriteRenderer _gunRenderer;

    /// <summary>
    /// A list of enemy in range
    /// </summary>
    private List<EnemyNPC> _enemies = new List<EnemyNPC>();

    /// <summary>
    /// Where should the attack cell spawn from
    /// </summary>
    private Transform _attackCellSpawnPoint;
    /// <summary>
    /// Are we rendering a sprite on the tower
    /// </summary>
    private bool HasSprite { get; set; } = false;

    /// <summary>
    /// Can this tower fire? (read-only)
    /// </summary>
    private bool CanFire => HasSprite && _enemies.Count > 0;

    /// <summary>
    /// A reference to the attack cell prefab
    /// </summary>
    public AttackCell attackCellPrefab;

    /// <summary>
    /// What kind of white cell does this tower posses
    /// </summary>
    public WhiteCellObject towerDescriptor;

    private void Awake()
    {
        Physics2D.queriesHitTriggers = false;

        _cellRenderer = transform.Find("CellSprite").GetComponent<SpriteRenderer>();
        _rangeRenderer = transform.Find("Range").GetComponent<SpriteRenderer>();
        _gunRenderer = transform.Find("TowerGun").GetComponent<SpriteRenderer>();
        _attackCellSpawnPoint = transform.Find("AttackCellSpawnPoint");

        _rangeRenderer.color = fullyTransparent;
        _gunRenderer.color = fullyTransparent;
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        StartCoroutine(FireBullets());
    }

    private void OnDisable()
    {
        StopCoroutine(FireBullets());
    }

    // Update is called once per frame
    void Update()
    {
        if (CanFire)
        {
            _gunRenderer.transform.up = -(_enemies[0].transform.position - _gunRenderer.transform.position);
        }
    }

    public IEnumerator FireBullets()
    {
        while (true)
        {
            if (CanFire)
            {
                var newAttackCell = Instantiate(attackCellPrefab, Vector3.zero, Quaternion.identity, _attackCellSpawnPoint);

                newAttackCell.transform.localPosition = Vector3.zero;
                newAttackCell.cellDamage = towerDescriptor.cellDamage;

                // Rotate the attack cell to aim at an enemy
                Vector3 vectorToTarget = _enemies[0].transform.position - transform.position;
                float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
                newAttackCell.transform.rotation = q;

                yield return new WaitForSeconds(1f / towerDescriptor.cellFireRate);
            }
            else
            {
                yield return null;
            }
        }
    }

    private void OnMouseEnter()
    {
        if (GameController.SelectedWhiteCell != null && !HasSprite)
        {
            /*_cellRenderer.sprite = Sprite.Create(GameController.SelectedWhiteCell.texture, GameController.SelectedWhiteCell.textureRect,
                GameController.SelectedWhiteCell.pivot);*/
            _cellRenderer.sprite = GameController.SelectedWhiteCell.cellSprite;
        }

        _rangeRenderer.color = halfTransparent;
    }

    private void OnMouseExit()
    {
        if (GameController.SelectedWhiteCell != null && !HasSprite)
        {
            _cellRenderer.sprite = null;
        }

        _rangeRenderer.color = fullyTransparent;
    }

    private void OnMouseUp()
    {
        if (GameController.SelectedWhiteCell != null && !HasSprite)
        {
            _cellRenderer.sprite = GameController.SelectedWhiteCell.cellSprite;
            towerDescriptor = GameController.SelectedWhiteCell;
            HasSprite = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Virus"))
        {
            _enemies.Add(other.GetComponent<EnemyNPC>());

            if (CanFire )
                _gunRenderer.color = halfTransparent;

            Debug.Log($"{_enemies.Count} enemies in range");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Virus"))
        {
            _enemies.Remove(other.GetComponent<EnemyNPC>());

            if (_enemies.Count == 0)
                _gunRenderer.color = fullyTransparent;

            Debug.Log($"{_enemies.Count} enemies in range");
        }
    }
}
