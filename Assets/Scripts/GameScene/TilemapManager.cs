using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

// Enum to be able to easily select a difficulty to test in the editor
public enum DifficultyMeasure
{
    Test,
    Beginner,
    Intermediate,
    Expert,
    Nightmare
}

// Main class, responsible for all tilemap behaviour.
public class TilemapManager : MonoBehaviour
{
    public float doubleClickBuffer;
    // The prefab of a tile used to change a tile's sprite.
    public Tile emptyTile;
    // The sprites used in the game
    public Sprite[] numSprites;
    // The sprite of the flag used, separated from the others for usability purposes
    public Sprite flagSprite;

    // The instance of the board managing script
    private Board board;
    // The instance of the hexagonal grid
    private Grid grid;
    // The game object in which the tilemap is in (in UI)
    private GameObject container;
    // the container of the grid, used to manage the grid's position
    private GameObject gridGameObject;
    // The script to manage the stats in the end
    private EndScript endScreen;
    // The script to manage the stats midgame
    private GameObject gameStats;
    // the Unity tilemap instance connected to the script
    private Tilemap tilemap;
    // is player dead
    private bool isDead;

    private bool isRightClicking;

    // the difficulty measure used
    public DifficultyMeasure difficultyMeasure;
    // actual size of the board
    public float wantedSize;
    // size of one vertex of the board in cells
    private short boardSize;
    // amount of bombs on board
    private short bombAmount;
    // Initialization of manager
    private void Awake()
    {
        // If no difficulty from the menu screen, use difficulty selected in the enum.
        if (Difficulty.currentDifficulty == null) Difficulty.ChangeDifficultyByName(difficultyMeasure.ToString());
        // find the end screen gameobject
        endScreen = GameObject.Find("Canvas").transform.Find("Body").transform.Find("Stats").Find("End Screen").gameObject.GetComponent<EndScript>();
        // find the game stats gameobject
        gameStats = GameObject.Find("StatsText");
        
    }

    void Start()
    {
        // get board size and amount of bombs from the given difficulty
        boardSize = Difficulty.currentDifficulty.boardSize;
        bombAmount = Difficulty.currentDifficulty.bombAmount;
        // Getting components and gameobjects in scene.
        tilemap = GetComponent<Tilemap>();
        grid = transform.parent.GetComponent<Grid>();
        container = GameObject.Find("Canvas").transform.Find("Body").transform.Find("Board").gameObject;
        gridGameObject = transform.parent.transform.parent.gameObject;
        
        // Initializing tilemap to be in the given size.
        for (int cellIndex = 0; cellIndex < Mathf.Pow(boardSize, 2); cellIndex++)
        {
            Tile newTile = Instantiate(emptyTile);
            tilemap.SetTile(new Vector3Int((int)Mathf.Floor(f: cellIndex / boardSize), cellIndex % boardSize, 0 ), newTile);
            newTile.name = cellIndex.ToString();
        }
        // Setting grid scale to fit wanted size
        grid.transform.localScale = new Vector3((wantedSize / tilemap.size.x), (wantedSize / tilemap.size.x));
        gridGameObject.transform.position = 0.8f * Camera.main.ScreenToWorldPoint(new Vector3(-Screen.width/1000f, -Screen.height/1000f, 10/0.8f)); // I don't know why, but it works.

        
        // creating instance of scripted board
        board = new Board(boardSize, bombAmount);
    }

    
    // Checking if mouse is inside the UI container to check if to accept mouse input
    private bool IsMouseInPosition()
    {
        PointerEventData pointerEventData = new PointerEventData(EventSystem.current);
        pointerEventData.position = Input.mousePosition;
        List<RaycastResult> raycastResultList = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerEventData, raycastResultList);
        for (int i = 0; i < raycastResultList.Count; i++)
        {
            if (raycastResultList[i].gameObject.GetComponent<AcceptMouseInput>() != null)
            {
                raycastResultList.RemoveAt(i);
                i--;
            }
        }

        return raycastResultList.Count <= 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsMouseInPosition() || isDead) return;
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
        if (tileCoords.x < 0 || tileCoords.x > boardSize - 1 || tileCoords.y < 0 || tileCoords.y > boardSize - 1) return;
        if (!board.isInitialized) board.InitializeBoard(tileCoords);
        try
        {
            if ((board.IsRevealed(tileCoords) && !SecretScript.allMiddleClick) || board.IsFlagged(tileCoords)) return;
            if (board.IsRevealed(tileCoords) && SecretScript.allMiddleClick)
            {
                MiddleClick(tileCoords);
                return;
            }
            board.SetRevealed(tileCoords);
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
        if (board.IsRevealed(coords) && !SecretScript.allMiddleClick) return;
        if (board.IsRevealed(coords) && SecretScript.allMiddleClick)
        {
            MiddleClick(coords);
            return;
        }
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
        if (!clickedCell.isRevealed || clickedCell.bombsNearby != clickedCell.flagsNearby) return;
        foreach (Cell neighbor in clickedCell.neighbors.Where(neighbor => !neighbor.isRevealed))
        {
            LeftClick(board.CellCoords(neighbor));
        }
        if(board.checkWin()) End(true);
    }

    private void End(bool isWin)
    {
        StatsScript.StopTime();
        FlashingTextScript.statusText.DisplayText(isWin? "Success" : "Game Over", 0.2f, 1);
        // gridGameObject.SetActive(false);
        if (!isWin) DeathReveal();
        isDead = true;
        endScreen.gameObject.SetActive(true);
        gameStats.SetActive(false);
        endScreen.End(isWin, DateTime.Now - StatsScript.startTime);
    }

    private void DeathReveal()
    {
        for (int id = 0; id < boardSize * boardSize; id++)
        {
            Cell currentCell = board.GetCellById(id);
            if (currentCell.isRevealed) continue;
            Tile newTilePlace = Instantiate(emptyTile);
            int wantedSpriteIndex = 0;
            if (currentCell.isBomb && currentCell.isFlagged)
            {
                wantedSpriteIndex = 9;
            }
            else if (currentCell.isBomb)
            {
                wantedSpriteIndex = 7;
            }
            else if (currentCell.isFlagged)
            {
                wantedSpriteIndex = 8;
            }
            else
            {
                wantedSpriteIndex = currentCell.bombsNearby;
            }

            newTilePlace.sprite = numSprites[wantedSpriteIndex];
            tilemap.SetTile(board.CellCoords(currentCell), newTilePlace);
        }
    }

    public void Restart()
    {
        StatsScript.StopTime();
        isDead = false;
        board.Restart();
        gameStats.SetActive(true);
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

    public Board(short boardSize, short bombAmount)
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
            if (cell.isFlagged && !cell.isBomb) return false;
        }
        return true;
    }

    public void InitializeBoard(Vector3Int emptyLocation)
    {
        board = this;
        StatsScript.StartTime();
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
    
    public void InitializeBoard() // For before first click
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

    public int BombsFlagged()
    {
        int bombsFlagged = 0;
        foreach (Cell cell in cells)
        {
            if (cell.isFlagged && cell.isBomb) bombsFlagged++;
        }

        return bombsFlagged;
    }

    public int FlagsMisplaced()
    {
        int flags = 0;
        foreach (Cell cell in cells)
        {
            if (cell.isFlagged && !cell.isBomb) flags++;
        }

        return flags;
    }
    
    public int FlagsPlaced()
    {
        int flags = 0;
        foreach (Cell cell in cells)
        {
            if (cell.isFlagged) flags++;
        }

        return flags;
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

    public Vector3Int GetCoordsById(int id)
    {
        return new Vector3Int(id/boardSize,id%boardSize,0);
    }

    public Cell GetCellById(int id)
    {
        return cells[id];
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
