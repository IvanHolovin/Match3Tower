using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class BoardManager : MonoBehaviour
{
    [SerializeField] private Vector2Int _size;
    [SerializeField] private Tile _tilePrefab;
    [SerializeField] private Transform _tilesBoard;
    
    private List<Tile> TileWasChanged = new List<Tile>();
    private List<Task> moveTasks = new List<Task>(); 
    private List<Task> swapTasks = new List<Task>();
    private BlockSpawner _blockSpawner;
    private Tile[,] _grid;
    private Dictionary<Tile, Tile[]> neighborDictionary = new Dictionary<Tile, Tile[]>();

    private Block _currentSpawnedBlock;
    
    public Tile[] Neighbors(Tile tile)
    {
        return neighborDictionary[tile];
    }

    void Awake()
    {
        _blockSpawner = GetComponent<BlockSpawner>();
    }

    public void StartBoard()
    {
        InitializeBoard(_size);
        _currentSpawnedBlock = _blockSpawner.SpawnBlock();
    }

    private void InitializeBoard(Vector2Int size)
    {

        _grid = new Tile[size.x, size.y];
        Vector2 offset = new Vector2((size.x - 1f) * 0.5f, (size.y - 1f) * 0.5f);

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                _grid[x, y] = Instantiate(_tilePrefab);
                _grid[x, y].transform.SetParent(_tilesBoard, false);
                _grid[x, y].transform.localScale = new Vector3(0.9f, 0.9f, 1f);
                _grid[x, y].transform.localPosition = new Vector3(
                    x - offset.x, 0f, y - offset.y);
                _grid[x, y].Init(x, y);
                
            }
        }

        for (int y = 0; y < size.y; y++)
        {
            for (int x = 0; x < size.x; x++)
            {
                List<Tile> neighbors = new List<Tile>();
                if (y < size.y - 1)
                    neighbors.Add(_grid[x, y + 1]);
                if (x < size.x - 1)
                    neighbors.Add(_grid[x + 1, y]);
                if (y > 0)
                    neighbors.Add(_grid[x, y - 1]);
                if (x > 0)
                    neighbors.Add(_grid[x - 1, y]);
                neighborDictionary.Add(_grid[x, y], neighbors.ToArray());
            }
        }
    }

    private Tile FindLastNonOccupiedOrNull(int column)
    {
        Tile freeTile = null;
        for (int y = 0; y < _size.y; y++)
        {
            if (_grid[column,y].OccupiedBlock == null)
                freeTile = _grid[column, y];
        }
        return freeTile;
    }

    private Tile FindFirstNonOccupiedorNull(int column)
    {
        if (_grid[column, 0].OccupiedBlock == null)
            return _grid[column, 0];
        else
            return null;
    }

    public void SelectColumnToSpawn(Tile tile)
    {
        Tile spawnTile;
        Tile destinationTile;
        if (tile != null)
        {
            destinationTile = FindLastNonOccupiedOrNull(tile.X);
            spawnTile = FindFirstNonOccupiedorNull(tile.X);
            if (spawnTile != null && spawnTile.OccupiedBlock == null )
            {
                _currentSpawnedBlock.transform.position = spawnTile.transform.position; 
                
                MoveBlock(_currentSpawnedBlock,destinationTile,0.7f);
                
                AddTileToSwapList(_currentSpawnedBlock.currentTile);
                
                _currentSpawnedBlock = _blockSpawner.SpawnBlock();
            }
            
        }
    }

    private List<Block> CheckNeighbor(Block currentBlock)
    {
        List<Block> neighborBlocks = new List<Block>();
        Tile[] neighbor = Neighbors(currentBlock.currentTile);
        foreach (var tile in neighbor)
        {
            if (tile.OccupiedBlock != null)
            {
                neighborBlocks.Add(tile.OccupiedBlock);
            }
        }
        return neighborBlocks;
    }
    
    
    
    private void DefineStrategy(Tile currentTile)
    {
        TileWasChanged.Remove(currentTile);
        if (currentTile.OccupiedBlock != null)
        {
            Block currentBlock = currentTile.OccupiedBlock;
            List<Block> neighbors = CheckNeighbor(currentBlock);
            List<Block> blockWithSameTopDonut =
                neighbors.FindAll(b => b.donutsList.Last().Id == currentBlock.donutsList.Last().Id);
            if (neighbors.Count > 0)
            {
                switch (currentBlock.donutCount)
                {
                    case 1: 
                        
                        Block blockWithTwoSame = 
                            blockWithSameTopDonut.Find(b => b.donutsList.Count == 2 && b.donutsList[0].Id == currentBlock.currentTopDonut.Id && b.donutsList[1].Id == currentBlock.currentTopDonut.Id);
                        if (blockWithTwoSame != null)
                        {
                            TransferDonut(currentBlock, blockWithTwoSame);
                            return;
                        }
                        
                        Block blockWithTwoSameOnTopOfThree = 
                            blockWithSameTopDonut.Find(b => b.donutsList.Count == 3 && b.donutsList[1].Id == currentBlock.currentTopDonut.Id && b.donutsList[2].Id == currentBlock.currentTopDonut.Id);
                        if (blockWithTwoSameOnTopOfThree != null)
                        {
                            TransferTwoDonut(blockWithTwoSameOnTopOfThree, currentBlock);
                            TileWasChanged.Add(blockWithTwoSameOnTopOfThree.currentTile);
                            return;
                        }

                        if(blockWithSameTopDonut.Count > 0)
                        {
                            TileWasChanged.Add(currentBlock.currentTile);
                            TileWasChanged.Add(blockWithSameTopDonut[0].currentTile);
                            TransferDonut(blockWithSameTopDonut[0], currentBlock);
                            return;
                        }
                        break;

                    case 2:

                        if (currentBlock.currentTopDonut.Id == currentBlock.donutsList[0].Id)
                        {
                            Block blockWithTwoSameDiffOnBottomAndSameOnTop =
                                blockWithSameTopDonut.Find(b =>
                                    b.donutsList.Count == 3 && b.donutsList[0].Id != currentBlock.currentTopDonut.Id &&
                                    b.donutsList[1].Id == b.donutsList[0].Id);
                            if (blockWithTwoSameDiffOnBottomAndSameOnTop != null)
                            {
                                TileWasChanged.Add(blockWithTwoSameDiffOnBottomAndSameOnTop.currentTile);
                                TransferDonut(blockWithTwoSameDiffOnBottomAndSameOnTop, currentBlock);
                                return;
                            }
                            Block blockWithOneSame =
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 1);
                            if (blockWithOneSame != null)
                            {
                                TransferDonut(blockWithOneSame, currentBlock);
                                return;
                            }

                            if (blockWithSameTopDonut.Count > 0)
                            {
                                TileWasChanged.Add(blockWithSameTopDonut[0].currentTile);
                                TransferDonut(blockWithSameTopDonut[0], currentBlock);
                            }
                        }
                        else
                        {
                            List<Block> listWithOneBlocks = blockWithSameTopDonut.FindAll(b => b.donutsList.Count == 1);
                            if (listWithOneBlocks.Count > 1)
                            {
                                TileWasChanged.Add(currentBlock.currentTile);
                                TransferDonut(listWithOneBlocks[0], currentBlock);
                                TransferTwoDonut(currentBlock,listWithOneBlocks[1]);
                                return;
                            }
                            
                            Block blockWithTwoSameAsTop = 
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 2 && b.donutsList[0].Id == currentBlock.currentTopDonut.Id && b.donutsList[1].Id == currentBlock.currentTopDonut.Id);
                            if (blockWithTwoSameAsTop != null)
                            {
                                TileWasChanged.Add(currentBlock.currentTile);
                                TransferDonut(currentBlock, blockWithTwoSameAsTop);
                                return;
                            }
                            
                            Block blockWithOneSame =
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 1);
                            Block blockWithFewDifferent = blockWithSameTopDonut.Find(b => b.donutsList.Count >= 2);
                            
                            if(blockWithOneSame != null && blockWithFewDifferent != null)
                            {
                                
                                TileWasChanged.Add(currentBlock.currentTile);
                                TileWasChanged.Add(blockWithFewDifferent.currentTile);
                                TransferDonut(blockWithFewDifferent, currentBlock);
                                TransferTwoDonut(currentBlock, blockWithOneSame);
                                return;
                            }
                            
                            Block blockWithOne =
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 1);
                            if (blockWithOne != null)
                            {
                                TileWasChanged.Add(currentBlock.currentTile);
                                TransferDonut(currentBlock, blockWithOne);
                                return;
                            }
                            
                            Block blockWithThreeTwoTopSame = 
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 3 && b.donutsList[1].Id == b.donutsList[2].Id);
                            if (blockWithThreeTwoTopSame != null)
                            {
                                TransferDonut(blockWithThreeTwoTopSame, currentBlock);
                                return;
                            }
                            
                            Block blockWithThreeTwoBotSame = 
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 3 && b.donutsList[0].Id == b.donutsList[1].Id);
                            if (blockWithThreeTwoBotSame != null)
                            {
                                TileWasChanged.Add(blockWithThreeTwoBotSame.currentTile);
                                TransferDonut(blockWithThreeTwoBotSame, currentBlock);
                                return;
                            }
                            
                            
                            
                            if (blockWithSameTopDonut.Count > 0)
                            {
                                TileWasChanged.Add(currentBlock.currentTile);
                                TileWasChanged.Add(blockWithSameTopDonut[0].currentTile);
                                TransferDonut(blockWithSameTopDonut[0], currentBlock);
                            }
                        }
                        break;
                    
                    case 3:
                        if (currentBlock.currentTopDonut.Id == currentBlock.donutsList[1].Id)
                        {
                            Block blockWithOneSame =
                                blockWithSameTopDonut.Find(b => b.donutsList.Count == 1);
                            if (blockWithOneSame != null)
                            {
                                TileWasChanged.Add(currentBlock.currentTile);
                                TransferTwoDonut(currentBlock, blockWithOneSame);
                                return;
                            }
                            
                        }
                        
                        
                        Block blockWithTwoSameAsMyTop = 
                            blockWithSameTopDonut.Find(b => b.donutsList.Count == 2 && b.donutsList[0].Id == currentBlock.currentTopDonut.Id && b.donutsList[1].Id == currentBlock.currentTopDonut.Id);
                        if (blockWithTwoSameAsMyTop != null)
                        {
                            TileWasChanged.Add(currentBlock.currentTile);
                            TransferDonut(currentBlock, blockWithTwoSameAsMyTop);
                            return;
                        }
                        
                        Block blockWithOneSameWhileMyDiff =
                            blockWithSameTopDonut.Find(b => b.donutsList.Count == 1);
                        if (blockWithOneSameWhileMyDiff != null)
                        {
                            TileWasChanged.Add(currentBlock.currentTile);
                            TransferDonut(currentBlock, blockWithOneSameWhileMyDiff);
                            return;
                        }

                        if (currentBlock.currentTopDonut.Id == currentBlock.donutsList[1].Id)
                        {
                            Block blockWithTwoDiffMyTwoSame = blockWithSameTopDonut.Find(b => b.donutCount == 2);
                            if (blockWithTwoDiffMyTwoSame != null)
                            {
                                return;
                            }
                        }
                        
                        Block blockWithTwoDiff = blockWithSameTopDonut.Find(b => b.donutCount == 2);
                        if (blockWithTwoDiff != null)
                        {
                            
                            TileWasChanged.Add(currentBlock.currentTile);
                            TransferDonut(currentBlock, blockWithTwoDiff);
                            return;
                        }
                        
                        
                        
                        break;
                }
            }
        }
    }

    private async void AddTileToSwapList(Tile tile)
    {
        GameManager.Instance.GameStateUpdater(GameState.InProgress);
        TileWasChanged.Add(tile);
        do
        {
            await Task.WhenAll(moveTasks);
            
            DefineStrategy(TileWasChanged.Last());
            
            await Task.WhenAll(swapTasks);
            
            TryToMoveBlocksUp();
            
        } while (TileWasChanged.Count > 0);
        GameManager.Instance.GameStateUpdater(GameState.WaitingInput);
    }

    private void TryToMoveBlocksUp()
    {
        Debug.Log("trying to call");
        for (int y = _size.y-2; y >= 0; y--)
        {
            for (int x = _size.x-1; x >= 0; x--)
            {
                if (_grid[x, y].OccupiedBlock != null)
                {
                    Tile newPos = FindLastNonOccupiedOrNull(x);
                    if (newPos != null && newPos.Y > y)
                    {
                        MoveBlock(_grid[x, y].OccupiedBlock,newPos,0.2f);
                        TileWasChanged.Add(newPos);
                    }  
                }
            }
        }
    }

    private void TransferDonut(Block blockFrom, Block blockTo)
    {
        Donut donutToSwap = blockFrom.RemoveDonut();
        blockTo.AddDonut(donutToSwap);
        swapTasks.Add(donutToSwap.transform.DOJump(blockTo.PlaceToMoveDonut().position, 0.5f,1,0.5f).AsyncWaitForCompletion());
        swapTasks.Add(blockTo.transform.DOShakeScale(1f, 0.2f).OnComplete(() => blockTo.ResetDonutsLocalPositionOrKill()).AsyncWaitForCompletion());
    }

    private void TransferTwoDonut(Block blockFrom, Block blockTo)
    {
        Donut firstDonutToSwap = blockFrom.RemoveDonut();
        blockTo.AddDonut(firstDonutToSwap);
        swapTasks.Add(firstDonutToSwap.transform.DOJump(blockTo.PlaceToMoveDonut().position, 0.5f, 1, 0.5f).OnComplete(()=>TransferDonut(blockFrom,blockTo)).AsyncWaitForCompletion());
    }

    private void MoveBlock(Block block, Tile to ,float time)
    {
        block.MoveToNewTile(to);
        moveTasks.Add(block.transform.DOMove(to.transform.position, time)
            .SetEase(Ease.Linear).AsyncWaitForCompletion());
    }
    
}