using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Serializable]
public enum TileType
{
    Normal,
    Platform
}

public class CustomTilemap : Tile
{
    public TileType TileType;

#if UNITY_EDITOR
// The following is a helper that adds a menu item to create a RoadTile Asset
    [UnityEditor.MenuItem("Assets/Create/MyFirstGameJam/Tile")]
    public static void CreateRoadTile()
    {
        string path = UnityEditor.EditorUtility.SaveFilePanelInProject("Save Road Tile", "New Road Tile", "Asset", "Save Road Tile", "Assets");
        if (path == "")
            return;
        UnityEditor.AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<CustomTilemap>(), path);
    }
#endif
}

public static class TilemapExtension
{
    public static TileBase GetTileFromCollider(TilemapCollider2D col, Vector2 point)
    {
        var tileMap = col.GetComponent<Tilemap>();
        var grid = tileMap.layoutGrid;
        var pos = grid.WorldToCell(point);
        return tileMap.GetTile(pos);
    }
}
