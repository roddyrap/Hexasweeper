using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class TilemapManager : MonoBehaviour
{
    public Tile emptyTile;
    public Sprite[] numSprites;

    private Board board;
    private Grid grid;
    private Tilemap tilemap;
    public byte boardSize;
    public byte bombAmount;
    // Start is called before the first frame update
    void Start()
    {
        // Getting components
        tilemap = GetComponent<Tilemap>();
        grid = transform.parent.GetComponent<Grid>();
        board = new Board(boardSize, bombAmount);
        // Building board cells
        for (int cellIndex = 0; cellIndex < Mathf.Pow(boardSize, 2); cellIndex++)
        {
            Tile newTile = Instantiate(emptyTile);
            tilemap.SetTile(new Vector3Int((int)Mathf.Floor(cellIndex / boardSize), cellIndex % boardSize, 0), newTile);
            newTile.name = cellIndex.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            SwtichSprite(grid.WorldToCell(Camera.main.ScreenToWorldPoint(Input.mousePosition)));
        }
        
    }

    private void SwtichSprite(Vector3Int tileCoords)
    {
        Debug.Log("Clicked cell: " + board.GetCellByCoords(tileCoords).id);
        foreach (Cell neighbor in board.GetNeighbors(tileCoords))
        {
            Debug.Log("Neighbor: " + neighbor);
        }
        if (board.IsRevealed(tileCoords)) return;
        board.SetRevealed(tileCoords);
        if ( tileCoords.x < 0 || tileCoords.x > boardSize-1 || tileCoords.y < 0 || tileCoords.y > boardSize-1) return;
        int newSpriteIndex = board.bombNumByCellCoords(tileCoords);
        if (board.IsBomb(tileCoords))
        {
            newSpriteIndex = numSprites.Length-1;
        }

        if (newSpriteIndex == 0)
        {
            foreach (Cell neighbor in board.GetNeighbors(tileCoords))
            {
                if (neighbor.isRevealed) continue;
                SwtichSprite(board.CellCoords(neighbor));
            }
        }
        Tile newTile = Instantiate(emptyTile);
        newTile.sprite = numSprites[newSpriteIndex];
        tilemap.SetTile(tileCoords, newTile);
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
class Board
{
    // Will be the active board for easy access
    public static Board board;

    // All cells in the board
    private List<Cell> cells;
    private int boardSize;

    public Board(byte boardSize, byte bombAmount)
    {
        this.boardSize = boardSize;
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
            if (cellIndex > 0)
            {
                newCell.AddNeighbor(cells[cellIndex - 1]);
                if (cellIndex > boardSize)
                {
                    newCell.AddNeighbor(cells[cellIndex - boardSize]);
                    if (cellIndex > boardSize + 1)
                    {
                        newCell.AddNeighbor(cells[cellIndex - boardSize - 1]);
                    }
                }
            }
        }
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

    public Cell GetCellByCoords(Vector3Int coords)
    {
        return cells[coords.x + coords.y * boardSize];
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
}
