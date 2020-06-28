using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum DifficultyMeasure
{
    Beginner,
    Intermediate,
    Expert
}

public class TilemapManager : MonoBehaviour
{
    public Vector2 padding;
    public Tile emptyTile;
    public Sprite[] numSprites;
    public Sprite flagSprite;

    private Board board;
    private Grid grid;
    private GameObject container;
    private GameObject gridGameObject;
    private EndScript endScreen;
    private Tilemap tilemap;

    public DifficultyMeasure difficultyMeasure;
    public float wantedSize;
    private byte boardSize;
    private byte bombAmount;
    // Start is called before the first frame update
    private void Awake()
    {
        endScreen = GameObject.Find("Canvas").transform.Find("Body").Find("Board").Find("End Screen").gameObject.GetComponent<EndScript>();
    }

    void Start()
    {
        if (Difficulty.currentDifficulty == null) Difficulty.ChangeDifficultyByName(difficultyMeasure.ToString());
        boardSize = Difficulty.currentDifficulty.boardSize;
        bombAmount = Difficulty.currentDifficulty.bombAmount;
        // Getting components
        tilemap = GetComponent<Tilemap>();
        grid = transform.parent.GetComponent<Grid>();
        container = GameObject.Find("Canvas").transform.Find("Body").transform.Find("Board").gameObject;
        gridGameObject = transform.parent.transform.parent.gameObject;
        
        // Building board cells
        for (int cellIndex = 0; cellIndex < Mathf.Pow(boardSize, 2); cellIndex++)
        {
            Tile newTile = Instantiate(emptyTile);
            tilemap.SetTile(new Vector3Int((int)Mathf.Floor(cellIndex / boardSize), cellIndex % boardSize, 0 ), newTile);
            newTile.name = cellIndex.ToString();
        }
        // Getting offset to make grid fit in screen
        // Setting grid scale, going on a temp one
        grid.transform.localScale = new Vector3(wantedSize / tilemap.size.x, wantedSize / tilemap.size.x);
        // Setting grid midpoint
        Vector2 wantedMidpoint = container.transform.position;
        gridGameObject.transform.position = new Vector2(wantedMidpoint.x - boardSize*grid.transform.localScale.x / 2f, wantedMidpoint.y - boardSize*grid.transform.localScale.y / 2.9f);
        board = new Board(boardSize, bombAmount);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            LeftClick(WorldToBoardCoords(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(1))
        {
            RightClick(WorldToBoardCoords(Input.mousePosition));
        }
        else if (Input.GetMouseButtonUp(2))
        {
            MiddleClick(WorldToBoardCoords(Input.mousePosition));
        }
        
    }

    private void LeftClick(Vector3Int tileCoords)
    {
        if (!board.isInitialized) board.InitializeBoard(tileCoords);
        try
        {
            if (board.IsRevealed(tileCoords) || board.IsFlagged(tileCoords)) return;
            board.SetRevealed(tileCoords);
            if (tileCoords.x < 0 || tileCoords.x > boardSize - 1 || tileCoords.y < 0 || tileCoords.y > boardSize - 1) return;
            int newSpriteIndex = board.bombNumByCellCoords(tileCoords);
            if (board.IsBomb(tileCoords))
            {
                newSpriteIndex = numSprites.Length - 1;
                End(false);
            }

            if (newSpriteIndex == 0)
            {
                foreach (Cell neighbor in board.GetNeighbors(tileCoords))
                {
                    if (neighbor.isRevealed) continue;
                    LeftClick(board.CellCoords(neighbor));
                }
            }

            Tile newTile = Instantiate(emptyTile);
            newTile.sprite = numSprites[newSpriteIndex];
            tilemap.SetTile(tileCoords, newTile);
            if(board.checkWin()) End(true);
        }
        catch (NullReferenceException)
        {
            return;
        }
    }

    private void RightClick(Vector3Int coords)
    {
        if (!board.isInitialized) return;
        Tile newTile = Instantiate(emptyTile);
        if (board.IsRevealed(coords)) return;
        board.ReverseFlagCell(coords);
        if (board.IsFlagged(coords))
        {
            newTile.sprite = flagSprite;
        }
        tilemap.SetTile(coords, newTile);
        if(board.checkWin()) End(true);
    }

    private void MiddleClick(Vector3Int coords)
    {
        if (!board.isInitialized) return;
        Cell clickedCell = board.GetCellByCoords(coords);
        if (clickedCell.bombsNearby != clickedCell.flagsNearby) return;
        foreach (Cell neighbor in clickedCell.neighbors)
        {
            LeftClick(board.CellCoords(neighbor));
        }
        if(board.checkWin()) End(true);
    }

    private void End(bool isWin)
    {
        gridGameObject.SetActive(false);
        endScreen.gameObject.SetActive(true);
        endScreen.End(isWin, DateTime.Now - StatsScript.startTime);
    }

    public void Restart()
    {
        board.Restart();
        for (int cellIndex = 0; cellIndex < Mathf.Pow(boardSize, 2); cellIndex++)
        {
            Tile newTile = Instantiate(emptyTile);
            tilemap.SetTile(new Vector3Int((int)Mathf.Floor(cellIndex / boardSize), cellIndex % boardSize, 0 ), newTile);
            newTile.name = cellIndex.ToString();
        }

        board.flagsOnBoard = 0;
        gridGameObject.SetActive(true);
        endScreen.gameObject.SetActive(false);
        StatsScript.startTime = DateTime.Now;
    }
    

    public Vector3Int WorldToBoardCoords(Vector3 coords)
    {
        return grid.WorldToCell(Camera.main.ScreenToWorldPoint(coords));
    }
}

// The scripted "Cell" class the software will use for calculations of bombs nearby and tile location in general
internal class Cell
{
    // Identifier for neighbor assignment
    public int id;
    //
    public bool isRevealed;
    // List of neighbors
    public List<Cell> neighbors;
    public bool isFlagged;
    public readonly bool isBomb;

    public Cell(int id, bool isBomb)
    {
        this.id = id;
        neighbors = new List<Cell>();
        this.isBomb = isBomb;
    }

    // Iterating over the neighbors List to see how many bombs are nearby
    public byte bombsNearby
    {
        get
        {
            byte bombsCount = 0;
            foreach (Cell neighbor in neighbors)
            {
                if (neighbor.isBomb) bombsCount++;
            }
            return bombsCount;
        }
    }
    public byte flagsNearby
    {
        get
        {
            byte flagCount = 0;
            foreach (Cell neighbor in neighbors)
            {
                if (neighbor.isFlagged) flagCount++;
            }
            return flagCount;
        }
    }

    // Mutually adding neighbors
    public void AddNeighbor(Cell newNeighbor)
    {
        newNeighbor.neighbors.Add(this);
        this.neighbors.Add(newNeighbor);
    }

    public override string ToString()
    {
        return "Cell id: " + id;
    }
}



// The scripted instance of the board
internal class Board
{
    // Will be the active board for easy access
    public static Board board;
    public bool isInitialized;

    // All cells in the board
    private List<Cell> cells;
    private int boardSize;
    private int bombAmount;
    public int flagsOnBoard;

    public Board(byte boardSize, byte bombAmount)
    {
        this.boardSize = boardSize;
        this.bombAmount = bombAmount;
        InitializeBoard();
    }
    
    public bool IsBomb(Vector3Int coords)
    {
        return GetCellByCoords(coords).isBomb;
    }
    public int bombNumByCellCoords(Vector3Int coords)
    {
        if (coords.x + coords.y * boardSize > cells.Count || coords.x + coords.y * boardSize < 0) return -1;
        return GetCellByCoords(coords).bombsNearby;
    }
    
    public static int[] GetUniqueRandomArray(int min, int max, int count) {
        int[] result = new int[count];
        List<int> numbersInOrder = new List<int>();
        for (var x = min; x < max; x++) {
            numbersInOrder.Add(x);
        }
        for (var x = 0; x < count; x++) {
            var randomIndex = UnityEngine.Random.Range(0, numbersInOrder.Count);
            result[x] = numbersInOrder[randomIndex];
            numbersInOrder.RemoveAt(randomIndex);
        }

        return result;
    }
    
    public static int[] GetUniqueRandomArrayWithoutArray(int min, int max, int count, int[] forbidden) {
        int[] result = new int[count];
        List<int> numbersInOrder = new List<int>();
        for (var x = min; x < max; x++)
        {
            if (!forbidden.Contains(x)) numbersInOrder.Add(x);
        }
        for (var x = 0; x < count; x++) {
            var randomIndex = UnityEngine.Random.Range(0, numbersInOrder.Count);
            result[x] = numbersInOrder[randomIndex];
            numbersInOrder.RemoveAt(randomIndex);
        }

        return result;
    }

    public bool checkWin()
    {
        foreach (Cell cell in cells)
        {
            if (!cell.isRevealed && !cell.isBomb) return false;
            if (cell.isBomb && !cell.isFlagged) return false;
            if (cell.isFlagged && !cell.isBomb) return false;
        }
        return true;
    }

    public void InitializeBoard(Vector3Int emptyLocation)
    {
        board = this;
        if (GetCellByCoords(emptyLocation) == null) return;
        //
        isInitialized = true;
        //
        int emptyTileIndex = GetIdByCoords(emptyLocation);
        // Amount of cells needed
        int cellAmount = (int)Mathf.Pow(boardSize, 2);
        // Checking that bomb requirement is reasonable
        if (bombAmount >cellAmount) return;
        // Creating the location of the bombs by cell index
        List<Cell> neighbors = GetNeighbors(emptyLocation);
        neighbors.Add(GetCellByCoords(emptyLocation));
        int[] neighborsId = new int[neighbors.Count];
        for (int i = 0; i < neighborsId.Length; i++)
        {
            neighborsId[i] = neighbors[i].id;
        }
        int[] bombLocations = GetUniqueRandomArrayWithoutArray(0, cellAmount - 1, bombAmount, neighborsId);
        // Initializing cells List
        cells = new List<Cell>();
        // Creating the cells
        for (int cellIndex = 0; cellIndex < Mathf.Pow(boardSize, 2); cellIndex++)
        {
            Cell newCell = new Cell(cellIndex, Array.Exists(bombLocations, element => element == cellIndex));
            cells.Add(newCell);
            if (cellIndex % boardSize != 0)
            {
                newCell.AddNeighbor(cells[cellIndex - 1]);
            }
            if (cellIndex >= boardSize)
            {
                newCell.AddNeighbor(cells[cellIndex - boardSize]);
            }
            if (Mathf.Ceil(cellIndex / boardSize) % 2 == 0)
            {
                if (cellIndex >= boardSize + 1 && cellIndex % boardSize != 0)
                {
                    newCell.AddNeighbor(cells[cellIndex - boardSize - 1]);
                }
            }
            else
            {
                if (cellIndex >= boardSize - 1 && cellIndex % boardSize != boardSize - 1)
                {
                    newCell.AddNeighbor(cells[cellIndex - boardSize + 1]);
                }
            }
        }
    }
    
    public void InitializeBoard()
    {
        // Amount of cells needed
        int cellAmount = (int)Mathf.Pow(boardSize, 2);
        // Checking that bomb requirement is reasonable
        if (bombAmount >cellAmount) return;
        // Creating the location of the bombs by cell index
        int[] bombLocations = GetUniqueRandomArray(0, cellAmount - 1, bombAmount);
        // Initializing cells List
        cells = new List<Cell>();
        // Creating the cells
        for (int cellIndex = 0; cellIndex < Mathf.Pow(boardSize, 2); cellIndex++)
        {
            Cell newCell = new Cell(cellIndex, Array.Exists(bombLocations, element => element == cellIndex));
            cells.Add(newCell);
            if (cellIndex % boardSize != 0)
            {
                newCell.AddNeighbor(cells[cellIndex - 1]);
            }
            if (cellIndex >= boardSize)
            {
                newCell.AddNeighbor(cells[cellIndex - boardSize]);
            }
            if (Mathf.Ceil(cellIndex / boardSize) % 2 == 0)
            {
                if (cellIndex >= boardSize + 1 && cellIndex % boardSize != 0)
                {
                    newCell.AddNeighbor(cells[cellIndex - boardSize - 1]);
                }
            }
            else
            {
                if (cellIndex >= boardSize - 1 && cellIndex % boardSize != boardSize - 1)
                {
                    newCell.AddNeighbor(cells[cellIndex - boardSize + 1]);
                }
            }
        }
    }

    public void Restart()
    {
        isInitialized = false;
        InitializeBoard();
    }

    public Cell GetCellByCoords(Vector3Int coords)
    {
        if (coords.x > boardSize || coords.x < 0 || coords.y > boardSize || coords.y < 0) return null;
        return cells[coords.x + coords.y * boardSize];
    }
    
    public int GetIdByCoords(Vector3Int coords)
    {
        if (coords.x > boardSize || coords.x < 0 || coords.y > boardSize || coords.y < 0) return -1;
        return coords.x + coords.y * boardSize;
    }
    

    public List<Cell> GetNeighbors(Vector3Int coords)
    {
        return GetCellByCoords(coords).neighbors;
    }
    
    public Vector3Int CellCoords(Cell cell)
    {
        return new Vector3Int(cell.id % boardSize, (int)Mathf.Floor(cell.id / boardSize), 0);
    }

    public void SetRevealed(Vector3Int coords)
    {
        GetCellByCoords(coords).isRevealed = true;
    }

    public bool IsRevealed(Vector3Int coords)
    {
        return GetCellByCoords(coords).isRevealed;
    }

    public void ReverseFlagCell(Vector3Int coords)
    {
        if (Difficulty.currentDifficulty.bombAmount - flagsOnBoard <= 0 && !GetCellByCoords(coords).isFlagged) return;
        GetCellByCoords(coords).isFlagged = !GetCellByCoords(coords).isFlagged;
        if (GetCellByCoords(coords).isFlagged) flagsOnBoard++;
        else flagsOnBoard--;
    }

    public bool IsFlagged(Vector3Int coords)
    {
        return GetCellByCoords(coords).isFlagged;
    }
}
