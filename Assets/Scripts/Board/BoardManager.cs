using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
[RequireComponent(typeof(Tilemap))]
public class BoardManager : MonoBehaviour
{
    public Tilemap boardTilemap;
    private Board boardData = new Board();
    public Tilemap wallTilemap;
    public Grid grid;
    private bool dirty = false;
    public static BoardManager instance;
    // Start is called before the first frame update
    void Start() {
        BoardManager.instance = this;
        boardTilemap = GetComponent<Tilemap>();
        boardData.InitGrid(boardTilemap.size.x, boardTilemap.size.y);

        List<Vector2Int> wallPositions = new List<Vector2Int>();
        for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                if (wallTilemap.GetTile(tilePos)) {
                    wallPositions.Add(new Vector2Int(x - boardTilemap.cellBounds.min.x, y - boardTilemap.cellBounds.min.y));
                } 
            }
        }

        boardData.FillGridByMinesRandom(50, wallPositions);
        boardData.RevealTile(Mathf.RoundToInt(boardData.width/2) - 1, 2);

        for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                if (wallTilemap.GetTile(tilePos)) {
                    boardData.RevealTile(x - boardTilemap.cellBounds.x, y - boardTilemap.cellBounds.y, true);
                } 
            }
        }

        for (int i = 0; i < boardTilemap.transform.childCount-1; i++) {
            var tile = boardTilemap.transform.GetChild(i).GetComponent<BoardVisualTile>();
            tile.OnCleared.AddListener(OnTileClear);
        }
        dirty = true;
    }

    void UpdateVisuals() {
        for (int x = boardTilemap.cellBounds.min.x; x < wallTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < wallTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                var boardTilePos = new Vector3Int(tilePos.x - boardTilemap.cellBounds.min.x, tilePos.y - boardTilemap.cellBounds.min.y);
                var tileObject = boardTilemap.GetInstantiatedObject(tilePos);
                if (tileObject) {
                    var tile = tileObject.GetComponent<BoardVisualTile>();
                    tile.gridX = boardTilePos.x;
                    tile.gridY = boardTilePos.y;
                    var tileData = boardData.GetTile(boardTilePos.x, boardTilePos.y);
                    tile.SetRevealed(tileData.isRevealed);
                    tile.SetValue(boardData.CheckNumber(boardTilePos.x, boardTilePos.y));
                    tile.SetMine(boardData.GetMine(boardTilePos.x, boardTilePos.y) != null);
                }

            }
        }
        dirty = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (dirty) {
            UpdateVisuals();
        }
    }
    void OnTileClear(BoardVisualTile tile) {
        boardData.RevealTile(tile.gridX, tile.gridY);
        dirty = true;
    }

    List<BoardVisualTile> GetTilesAround(Vector3 worldPosition, int dist) {
        List<BoardVisualTile> tiles;
        var cellGridPos = boardTilemap.WorldToCell(worldPosition);
        for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
            }
        }
        return tiles;

    }
}
