using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TIlemapHover : MonoBehaviour
{
    private Tilemap _tilemap;

    // Start is called before the first frame update
    void Start()
    {
        _tilemap = GetComponent<Tilemap>();
    }

    // Update is called once per frame
    void Update()
    {
        var worldPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        var hoveredTilePos = _tilemap.WorldToCell(worldPoint);
        var hoveredTile = _tilemap.GetTile(hoveredTilePos);

        if (hoveredTile.name == "Tileset_2")
        {
            _tilemap.SetTileFlags(hoveredTilePos, TileFlags.None);
            _tilemap.SetColor(hoveredTilePos, Color.black);
        }

        Debug.Log($"Hovering over: {hoveredTilePos}, Tile: {hoveredTile}");
    }
}
