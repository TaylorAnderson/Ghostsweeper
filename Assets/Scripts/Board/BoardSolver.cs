using System.Collections.Generic;
using System.Collections;
using UnityEngine;
public class BoardSolver {
  
  public Board board;
  private List<Vector2Int> neighbourDirs = new List<Vector2Int>() {
    Vector2Int.left, 
    Vector2Int.right,
    Vector2Int.up,
    Vector2Int.down
  };

  //the amount of iters = pairIters * solveIters
  public int solveIters = 50;

  public bool madeProgress = true;

  private int solveCounter;
  public BoardSolver() {

  }
  public void SolveBoard(Board board, MonoBehaviour scriptToStartCoroutine) {
    solveCounter = solveIters;
    this.board = board;
    while (solveCounter > 0) {
      madeProgress = false;
      if (board.IsSolved()) {
        break;
      }
      if (!board.firstClickMade) {
        board.RevealTile(Mathf.FloorToInt(board.width/2), Mathf.FloorToInt(board.height/2));
        madeProgress = true;
      }
      else {
        DoSolve();
      }
      if (!madeProgress) {
        solveCounter--;
      }
    }
  }


  public void DoSolve() {
    var relevantTiles = GetNumberedTiles();
    for (int i = 0; i < relevantTiles.Count; i++) {
      DoSimpleStrategy(relevantTiles[i]);
    }
    var pairArray = BuildPairArray();
    for (int i = 0; i < pairArray.Count; i++) {
      DoAdvancedStrategy(pairArray[i]);
    }
  }

  public List<List<BoardTile>> BuildPairArray() {
      List<BoardTile> numberedTiles = GetTilesNextToUnrevealed();
      List<List<BoardTile>> pairsArr = new List<List<BoardTile>>();
      for (int i = 0; i < numberedTiles.Count; i++) {
        var tile_a = numberedTiles[i];
        for (int j = 0; j < neighbourDirs.Count; j++) {
          var pos = new Vector2Int(tile_a.x + neighbourDirs[j].x, tile_a.y + neighbourDirs[j].y);
          if (board.TileExists(pos.x, pos.y)) {
            if (IsTileNumbered(pos.x, pos.y) && IsTileNextToUnrevealed(board.GetTile(pos.x, pos.y))) {
              var pair = new List<BoardTile>();
              var tile_b = board.GetTile(pos.x, pos.y);
              pair.Add(tile_a);
              pair.Add(tile_b);

              var pairExistsInArray = false;
              for (int p = 0; p < pairsArr.Count; p++) {
                if (pairsArr[p][0] == tile_a && pairsArr[p][1] == tile_b) {
                  pairExistsInArray = true;
                }
                if (pairsArr[p][0] == tile_b && pairsArr[p][1] == tile_a) {
                  pairExistsInArray = true;
                }
              } 
              if (!pairExistsInArray) {
                pairsArr.Add(pair);
              }
            }
          }
        }
      }
      return pairsArr;
  }
  
  public List<BoardTile> GetTilesNextToUnrevealed() {
    var tiles = new List<BoardTile>();
    var numberedTiles = GetNumberedTiles();
    for (int i = 0; i < numberedTiles.Count; i++) {
      var tile = numberedTiles[i];
      if (IsTileNextToUnrevealed(tile)) {
        tiles.Add(tile);
      }
    }
    return tiles;
  }

  public bool IsTileNextToUnrevealed(BoardTile tile) {
    var surrounding = board.GetSurrounding(tile.x, tile.y);
    for (int i = 0; i < surrounding.Count; i++) {
      var pos = surrounding[i];
      if (!board.GetTile(pos.x, pos.y).isRevealed) {
        return true;
      }
    }
    return false;
  }

  public void DoSimpleStrategy(BoardTile tile) {
      List<BoardTile> surroundingTiles = new List<BoardTile>();
      List<BoardTile> surroundingUnrevealedTiles = new List<BoardTile>();
      var surroundingPos = board.GetSurrounding(tile.x, tile.y);
      for (int i = 0; i < surroundingPos.Count; i++) {
        Vector2Int pos = surroundingPos[i];
        BoardTile surroundingTile = board.GetTile(pos.x, pos.y);
        surroundingTiles.Add(surroundingTile);

        if (!surroundingTile.isRevealed) {
          surroundingUnrevealedTiles.Add(surroundingTile);
        }
      }
  
      var tileValue = board.CheckNumber(tile.x, tile.y);
      if (tileValue == surroundingUnrevealedTiles.Count) {
        for (int i = 0; i < surroundingUnrevealedTiles.Count; i++) {
          BoardTile surroundingTile = surroundingUnrevealedTiles[i];
          if (!surroundingTile.isFlagged) {
            madeProgress = true;
          }
          board.FlagTile(surroundingTile.x, surroundingTile.y);
        }
      }
      var flaggedTiles = new List<BoardTile>();
      for (int i = 0; i < surroundingPos.Count; i++) {
        Vector2Int pos = surroundingPos[i];
        BoardTile surroundingTile = board.GetTile(pos.x, pos.y);
        if (surroundingTile.isFlagged) {
          flaggedTiles.Add(tile);
        }
      }
      if (tileValue == flaggedTiles.Count) {
        board.ChordTile(tile.x, tile.y);
        if (GetNonFlaggedNeighbours(tile).Count > 0) {
          madeProgress = true;
        }
      }
  }

  public void DoAdvancedStrategy(List<BoardTile> pair) {
      var tile_a = pair[0];
      var tile_b = pair[1];
      var nfn_a = GetNonFlaggedNeighbours(tile_a);
      var nfn_b = GetNonFlaggedNeighbours(tile_b);
      var a_value = board.CheckNumber(tile_a.x, tile_a.y) - GetFlaggedNeighbours(tile_a).Count;
      var b_value = board.CheckNumber(tile_b.x, tile_b.y) - GetFlaggedNeighbours(tile_b).Count;
      if (a_value == 1 && b_value == 1) {
        if (nfn_a.Count == 2 || nfn_b.Count == 2) {
          List<BoardTile> tilesToClear = new List<BoardTile>();
          if (AreListsTheSame(GetArrayNotInOther(nfn_a, nfn_b), nfn_a)) {
            tilesToClear = GetArrayNotInOther(nfn_b, nfn_a);
          }
          if (AreListsTheSame(GetArrayNotInOther(nfn_b, nfn_a), nfn_b)) {
            tilesToClear = GetArrayNotInOther(nfn_a, nfn_b);
          }
          for (int i = 0; i < tilesToClear.Count; i++) {
            if (!tilesToClear[i].isRevealed) {
              madeProgress = true;
            }
            board.RevealTile(tilesToClear[i].x, tilesToClear[i].y);
          }
        }
      }
      
      if (a_value - b_value == GetArrayNotInOther(nfn_a, nfn_b).Count) {
        RunPairStrategy(nfn_a, nfn_b);
      }
      else if (b_value - a_value == GetArrayNotInOther(nfn_b, nfn_a).Count) {
        RunPairStrategy(nfn_b, nfn_a);
      }
  }
  
  public List<BoardTile> GetIntersection(List<BoardTile> a, List<BoardTile> b) {
    List<BoardTile> list = new List<BoardTile>();
    for (int i = 0; i < a.Count; i++) {
      for (int j = 0; j < b.Count; j++) {
        if (a[i] == b[j]) {
          list.Add(a[i]);
        }
      }
    }
    return list;
  }

  public bool AreListsTheSame(List<BoardTile> listA, List<BoardTile> listB) {
    if (listA.Count != listB.Count) {
      return false;
    }
    bool a_in_b = true;
    for (int i = 0; i < listB.Count; i++) {
      for (int j = 0; j < listA.Count; j++) {
        if (listA[j] != listB[i]) {
          a_in_b = false;
        }
      }
    }
    if (a_in_b) {
      bool b_in_a = true;
      for (int i = 0; i < listA.Count; i++) {
        for (int j = 0; j < listB.Count; j++) {
          if (listA[i] != listB[j]) {
            b_in_a = false;
          }
        }
      }
      return b_in_a;
    }
    return false;
  }
  
  public void RunPairStrategy(List<BoardTile> nfn_a, List<BoardTile> nfn_b) {
      var nfn_a_not_b = GetArrayNotInOther(nfn_a, nfn_b);
      var nfn_b_not_a = GetArrayNotInOther(nfn_b, nfn_a);
      for (int i = 0; i < nfn_a_not_b.Count; i++) {
        var tile = nfn_a_not_b[i];
        if (!tile.isFlagged) {
          madeProgress = true;
        }
        board.FlagTile(tile.x, tile.y);

      }
      for (int i = 0; i < nfn_b_not_a.Count; i++) {
        var tile = nfn_b_not_a[i];
        if (!tile.isRevealed) {
          madeProgress = true;
        }
        board.RevealTile(tile.x, tile.y);
      }
  }
  
  public List<BoardTile> GetArrayNotInOther(List<BoardTile> list_a, List<BoardTile> list_b) {
    List<BoardTile> list = new List<BoardTile>();
    for (int i = 0; i < list_a.Count; i++) {
      BoardTile tile_a = list_a[i];
      bool found_val_in_b = false;
      for (int j = 0; j < list_b.Count; j++) {
        BoardTile tile_b = list_b[j];
        if (tile_b == tile_a) {
          found_val_in_b = true;
        }
      }
      if (!found_val_in_b) {
        list.Add(tile_a);
      }
    }
    return list;
  }
  
  public List<BoardTile> GetFlaggedNeighbours(BoardTile tile) {
    List<BoardTile> flaggedNeighbours = new List<BoardTile>();
    List<Vector2Int> surrounding = board.GetSurrounding(tile.x, tile.y);
    for (int i = 0; i < surrounding.Count; i++) {
      Vector2Int pos = surrounding[i];
      var surroundingTile = board.GetTile(pos.x, pos.y);
      if (surroundingTile.isFlagged){
        flaggedNeighbours.Add(surroundingTile);
      }
    }
    return flaggedNeighbours;
  }

  public List<BoardTile> GetNonFlaggedNeighbours(BoardTile tile) {
    List<BoardTile> nonFlaggedNeighbours = new List<BoardTile>();
    var surrounding = board.GetSurrounding(tile.x, tile.y);
    for (int i = 0; i < surrounding.Count; i++) {
      var pos = surrounding[i];
      var surroundingTile = board.GetTile(pos.x, pos.y);
      if (!surroundingTile.isFlagged && !surroundingTile.isRevealed){
        nonFlaggedNeighbours.Add(surroundingTile);
      }
    }
    return nonFlaggedNeighbours;
  }

  public List<BoardTile> GetNumberedTiles() {
    List<BoardTile> numberedTiles = new List<BoardTile>();
    foreach (BoardTile tile in board.GetTiles()) {
      if (IsTileNumbered(tile.x, tile.y)) {
        numberedTiles.Add(tile);
      }
    }
    return numberedTiles;
  }

  public bool IsTileNumbered(int x, int y) {
    var tile = board.GetTile(x, y);
    return tile.isRevealed && board.CheckNumber(x, y) > 0;
  }
  
  public List<BoardTile> GetPair() {
    List<BoardTile> pair = null;
    List<BoardTile> numberedTiles = GetTilesNextToUnrevealed();

    bool foundNeighbour = false;
    while (!foundNeighbour) {
      var tile_a = numberedTiles[Random.Range(0, numberedTiles.Count-1)];
      for (int j = 0; j < neighbourDirs.Count; j++) {
        var pos = new Vector2Int(tile_a.x + neighbourDirs[j].x, tile_a.y + neighbourDirs[j].y);
        if (board.TileExists(pos.x, pos.y)) {
          if (IsTileNumbered(pos.x, pos.y) && IsTileNextToUnrevealed(board.GetTile(pos.x, pos.y))) {
            pair = new List<BoardTile>();
            var tile_b = board.GetTile(pos.x, pos.y);
            pair.Add(tile_a);
            pair.Add(tile_b);
            return pair;
          }
        }
      }
    }
    return null;
  }

}