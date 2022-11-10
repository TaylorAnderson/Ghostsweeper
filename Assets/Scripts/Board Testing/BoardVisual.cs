using System;
using System.Runtime.Serialization.Formatters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class BoardVisual : MonoBehaviour
{
    public Board board;
    public GameObject tilePrefab;

    public List<BoardVisualTileUI> tiles = new List<BoardVisualTileUI>();
    private GridLayoutGroup gridLayout;

    public int w;
    public int h;
    public int mines;

    private bool dirty = true;
    // Start is called before the first frame update
    void Start()
    {
        gridLayout = GetComponent<GridLayoutGroup>();
        board = new Board();
        board.InitGrid(w, h);
        board.FillGridByMinesRandom(mines);
        Init();

        var solver = new BoardSolver();
        // StartCoroutine(solver.SolveBoard(board, this));
        solver.SolveBoard(board, this);
        dirty = true;
    }

    public void Update() {
        dirty = true;
        if (dirty) {
            UpdateVisual();
        }
    }

    public void Init() {
        if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedRowCount)
        gridLayout.constraintCount = board.height;

        if (gridLayout.constraint == GridLayoutGroup.Constraint.FixedColumnCount) {
            gridLayout.constraintCount = board.width;
        }
        
        foreach (BoardTile tile in board.GetTiles()) {
            var t = Instantiate(tilePrefab);
            t.transform.parent = transform;
            BoardVisualTileUI tileViz = t.GetComponent<BoardVisualTileUI>();
            tileViz.gridX = tile.x;
            tileViz.gridY = tile.y;
            tileViz.onClicked.AddListener(OnTileClicked);
            tileViz.onRightClicked.AddListener(OnTileRightClicked);
            tiles.Add(tileViz);
        }
        dirty = true;
    }

    public void UpdateVisual() {
        foreach (BoardTile tile in board.GetTiles()) {
            var tileData = tile;
            var tileViz = GetTileAt(tile.x, tile.y);
            
            tileViz.SetRevealed(tile.isRevealed);

            tileViz.SetFlagged(tileData.isFlagged);

            tileViz.SetMine(board.GetMine(tile.x, tile.y) != null);

            tileViz.SetDebug(tileData.isDebugMarked);

            if (tile.isRevealed) {
                tileViz.SetNumber(board.CheckNumber(tile.x, tile.y));
            }
        }

        dirty = false;
    }
    private BoardVisualTileUI GetTileAt(int x, int y) {
        foreach (var tile in tiles) {
            if (tile.gridX == x && tile.gridY == y) {
                return tile;
            }
        }
        return null;
    }

    public void OnTileClicked(BoardVisualTileUI vizTile) {
        BoardTile tile = board.GetTile(vizTile.gridX, vizTile.gridY);
        if (tile.isRevealed) {
            board.ChordTile(vizTile.gridX, vizTile.gridY);
        }
        else {
            board.RevealTile(vizTile.gridX, vizTile.gridY);
        }
        dirty = true;
    }
    public void OnTileRightClicked(BoardVisualTileUI vizTile) {
        BoardTile tile = board.GetTile(vizTile.gridX, vizTile.gridY);
        if (!tile.isRevealed) {
            board.FlagTile(vizTile.gridX, vizTile.gridY);

        }
        dirty = true;
    }
}
