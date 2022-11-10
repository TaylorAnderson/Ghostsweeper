using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine {
    public int x;
    public int y;
    public Mine(int x, int y) {
        this.x = x;
        this.y = y;
    }
}

public class BoardTile {
    public bool isRevealed;
    public bool isFlagged;
    public bool isDebugMarked;
    public int x;
    public int y;
}
public struct BoardTileLookup {
    public BoardTile tile;
    public int x;
    public int y;
    public BoardTileLookup(BoardTile tile, int x, int y) {
        this.tile = tile;
        this.x = x;
        this.y = y;
    }

}
public class Board
{
    
    public static Board instance;
    public List<Mine> mines;
    private List<List<BoardTile>> grid = null;
    public bool failed = false;
    public bool firstClickMade = false;
    private List<Vector2Int> coordinateList = new List<Vector2Int>() {
        new Vector2Int(-1, 0),
        new Vector2Int( 1, 0),
        new Vector2Int( 0, 1),
        new Vector2Int( 0, -1),
        new Vector2Int( 1, 1),
        new Vector2Int(-1, 1),
        new Vector2Int( 1,-1),
        new Vector2Int(-1,-1),
    };

    public int width {
        get {
            if (grid == null) {
                Debug.Log("You tried to get the width of the grid, but the grid doesn't exist yet. Returning -1");
                return -1;
            }
            else {
                return grid[0].Count;
            }
        }
    }
    
    public int height {
        get {
            if (grid == null) {
                Debug.Log("You tried to get the width of the grid, but the grid doesn't exist yet. Returning -1");
                return -1;
            }
            else {
                return grid.Count;
            }
        }
    }

    public void InitGrid(int width, int height) {
        mines = new List<Mine>();
        grid = new List<List<BoardTile>>();
        for (int y = 0; y < height; y++) {
            grid.Add(new List<BoardTile>());
            for (int x = 0; x < width; x++) {
                var tile = new BoardTile();
                tile.x = x;
                tile.y = y;
                grid[y].Add(tile);
            }
        }
    }
    
    public void Reset() {
        foreach (BoardTile tile in GetTiles()) {
            tile.isFlagged = false;
            tile.isRevealed = false;
            tile.isDebugMarked = false;
        }
        firstClickMade = false;
        failed = false;
        mines = new List<Mine>();
    }
    public void FillGridByMinesRandom(int mineCount, List<Vector2Int> bannedPositions = null) {
        for (int i = 0; i < mineCount; i++) {
            var mine = new Mine(Random.Range(0, width-1), Random.Range(0, height-1));
            while (GetMine(mine.x, mine.y) != null || PositionIsBanned(mine.x, mine.y, bannedPositions)) {
                mine.x = Random.Range(0, width-1);
                mine.y = Random.Range(0, height-1);
            }
            mines.Add(mine);
        }

    }

    public bool PositionIsBanned(int x, int y, List<Vector2Int> bannedPositions) {
        if (bannedPositions == null) {
            return false;
        }
        var pos = new Vector2(x, y);
        for (int i = 0; i < bannedPositions.Count; i++) {
            if (bannedPositions[i].x == pos.x && bannedPositions[i].y == pos.y) {
                return true;   
            }
        }
        return false;
    }

    public void FillGridByMinesPerRow(List<int> minesPerRow, bool bottomToTop) {
        int iter = bottomToTop ? -1 : 1;
        
        int start = bottomToTop ? minesPerRow.Count - 1 : 0;
        int mineRowIndex = 0;
        for (int row = start; bottomToTop ? row >= 0 : row < minesPerRow.Count; row += iter) {
            for (int i = 0; i < minesPerRow[mineRowIndex]; i++) {

                var mine = new Mine(Random.Range(0, grid[row].Count-1), row);
                while (GetMine(mine.x, mine.y) != null) {
                    mine.x = Random.Range(0, grid[row].Count-1);
                }
                mines.Add(mine);
            }
            mineRowIndex ++;
        }
    }
    
    public IEnumerable<BoardTile> GetTiles() {
        for (int y = 0; y < height; y++) {
            for (int x = 0; x < width; x++) {
                yield return GetTile(x, y);
            }
        }
    }
    
    public Mine GetMine(int x, int y) {
        for(int i= 0; i < mines.Count; i++) {
            if (mines[i].x == x && mines[i].y == y) {
                return mines[i];
            }
        }
        return null;
    }

    public List<Vector2Int> GetSurrounding(int x, int y) {
        List<Vector2Int> tilePositions = new List<Vector2Int>();
        for (int i = 0; i < coordinateList.Count; i++) {
            var dir = coordinateList[i];
            if (TileExists(x + dir.x, y + dir.y)) {
                tilePositions.Add(new Vector2Int(x + dir.x, y + dir.y));
            }
        }
        return tilePositions;
    }

    public bool TileExists(int x, int y) {
        return (x >= 0) && (y >= 0) && (y < grid.Count) && (x < grid[0].Count);
    }

    public int CheckNumber(int x, int y) {
        var number = 0;
        var positions = GetSurrounding(x, y);
        for (int i = 0; i < positions.Count; i++) {
            if (GetMine(positions[i].x, positions[i].y) != null) {
                number++;
            }
        }
        return number;
    }
    
    public BoardTile GetTile(int x, int y) {
        return grid[y][x];
    }

    public void RevealTile(int x, int y, bool forceNotExpand = false) {
        var tile = GetTile(x, y);
        if (tile.isRevealed) {
            return;
        }
        tile.isRevealed = true;
        tile.isFlagged = false;
        var mine = GetMine(x, y);
        if (mine != null) {
            if (firstClickMade) {
                failed = true;
                return; 
            }
            else {
                mines.Remove(mine);
            }
        }
        if ((!firstClickMade || CheckNumber(x, y) == 0) && !forceNotExpand) {
            firstClickMade = true;
            var positions = GetSurrounding(x, y);
            for (int i = 0; i < positions.Count; i++) {
                if (GetMine(positions[i].x, positions[i].y) == null) {
                    RevealTile(positions[i].x, positions[i].y);
                }
            }
        }
    }

    public void FlagTile(int x, int y) {
        var tile = GetTile(x, y);
        if (tile.isRevealed) {
            Debug.Log("ERROR! Tried to flag an already revealed tile");
            return;
        }
        tile.isFlagged = true;
    }

    public void MarkTileForDebug(int x, int y) {
        GetTile(x, y).isDebugMarked = true;
    }

    public void RemoveFlag(int x, int y) {
        GetTile(x, y).isFlagged = false;
    }
    
    public void ChordTile(int x, int y) {
        var tile = GetTile(x, y);
        if (!tile.isRevealed) {
            return;
        }
        if (CheckNumber(x, y) != GetSurroundingFlags(x, y)) {
            return;
        }
        var surrounding = GetSurrounding(x, y);
        for (int i = 0; i < surrounding.Count; i++) {
            var tilePos = surrounding[i];
            if (!GetTile(tilePos.x, tilePos.y).isFlagged) {
                RevealTile(tilePos.x, tilePos.y);
            }
        }
    }
    
    public int GetSurroundingFlags(int x, int y) {
        var surrounding = GetSurrounding(x, y);
        var flags = 0;
        for (int i = 0; i < surrounding.Count; i++) {
            if (GetTile(surrounding[i].x, surrounding[i].y).isFlagged) {
                flags ++;
            }
        }
        return flags;
    }

    public bool IsSolved(bool debug = false) {
        var flaggedAllMines = true;
        for (int i = 0; i < mines.Count; i++) {
            if (!GetTile(mines[i].x, mines[i].y).isFlagged) {
                flaggedAllMines = false;
            }
        }
        var flaggedTooMany = false;
        foreach (BoardTile tile in GetTiles()) {
            if (tile.isFlagged) {
                var foundMine = false;
                for (int i = 0; i < mines.Count; i++) {
                    if (mines[i].x == tile.x && mines[i].y == tile.y) {
                        foundMine = true;
                    }
                }
                if (!foundMine) {
                    flaggedTooMany = true;
                }
            }
        }
        if (debug) {
            Debug.Log("Flagged all mines: " + flaggedAllMines);
            Debug.Log("Flagged too many: " + flaggedTooMany);
            Debug.Log("Hit a mine: " + failed);
        }

        return flaggedAllMines && !flaggedTooMany && !failed;
    }
}
