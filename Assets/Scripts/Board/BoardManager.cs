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
    public Tilemap entranceTilemap;
    public Grid grid;
    private bool dirty = false;
    public static BoardManager instance;

    public List<Character> characters;
    // Start is called before the first frame update
    void Awake() {
        BoardManager.instance = this;
        boardTilemap = GetComponent<Tilemap>();
        boardData.InitGrid(boardTilemap.size.x, boardTilemap.size.y);

        List<Vector2Int> restrictedPositions = new List<Vector2Int>();
        for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
                var tilePos = new Vector3Int(x, y, 0);
                if (wallTilemap.GetTile(tilePos)) {
                    restrictedPositions.Add(new Vector2Int(x - boardTilemap.cellBounds.min.x, y - boardTilemap.cellBounds.min.y));
                } 
            }
        }

        for (int x = entranceTilemap.cellBounds.min.x; x < entranceTilemap.cellBounds.max.x; x++) {
          for (int y = entranceTilemap.cellBounds.min.y; y < entranceTilemap.cellBounds.max.y; y++) {
            var tilePos = new Vector3Int(x, y, 0);
            if (entranceTilemap.GetTile(tilePos)) {
                restrictedPositions.Add(new Vector2Int(x - boardTilemap.cellBounds.min.x, y - boardTilemap.cellBounds.min.y));
            } 
          }
        }

        boardData.FillGridByMinesRandom(40, restrictedPositions);
        var charBoardTile = GetBoardTileOnCharacter(characters[0]);
        boardData.RevealTile(charBoardTile.x, charBoardTile.y);

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

        for (int i = 0; i < characters.Count; i++) {
          characters[i].OnMove.AddListener(OnCharacterMove);
        }
        dirty = true;

        UpdateVisuals();
    }

    void OnCharacterMove(Character character, Vector3 dest) {
      for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
        for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
          var tilePos = new Vector3Int(x, y, 0);
          var boardTile = boardTilemap.GetInstantiatedObject(tilePos);
          if (boardTile && dest != character.transform.position) {
            boardTile.GetComponent<BoardVisualTile>().mistDamaged = false;
          }
        }
      }
    }

    void UpdateVisuals() {
        for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
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

    public Vector2Int GetBoardTileOnCharacter(Character character) {
        var boardTilemapCell = boardTilemap.WorldToCell(character.transform.position);
        return new Vector2Int(boardTilemapCell.x - boardTilemap.cellBounds.min.x, boardTilemapCell.y - boardTilemap.cellBounds.min.y);
    }
    public void ShowNumberOnTiles(Vector3 worldPosition, int dist) {
        for (int x = boardTilemap.cellBounds.min.x; x < boardTilemap.cellBounds.max.x; x++) {
            for (int y = boardTilemap.cellBounds.min.y; y < boardTilemap.cellBounds.max.y; y++) {
                var boardTile = boardTilemap.GetInstantiatedObject(new Vector3Int(x, y, 0));
                if (boardTile) {
                    var boardvisualTile = boardTile.GetComponent<BoardVisualTile>();
                    if (Vector2.Distance(worldPosition, boardTile.transform.position) < dist) {
                        boardvisualTile.SetValueShown(true);
                    }
                    else {
                        boardvisualTile.SetValueShown(false);
                    }
                }

            }
        }
    }
}
