using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public enum GameState
{
    WaitingInput,
    InProgress,
    Lose
}

public class GameManager : MonoBehaviour
{
    [SerializeField] private InputController _inputController;
    [SerializeField] private BoardManager _boardManager;

    private int _score;
    
    public static GameManager Instance;
    public int Score => _score;
    

    public GameState State { get; private set; }
        
    private void Awake()
    {
        Instance = this;
        GameStateUpdater(GameState.WaitingInput);
    }

    private void Start()
    {
        _boardManager.StartBoard();
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && State == GameState.WaitingInput)
        {
            _boardManager.SelectColumnToSpawn(_inputController.GetTile());
            
        }
    }

    public void GameStateUpdater(GameState newState)
    {
        State = newState;
        
        switch (newState)
        {
            case GameState.WaitingInput:
                break;
            case GameState.InProgress:
                break;
            case GameState.Lose:
                Debug.Log("loose");
                break;
        }
    }

    public void AddScore(int count)
    {
        _score += count;
    }
    

       
        
}
