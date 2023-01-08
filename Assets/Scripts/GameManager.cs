using System;
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

    public static GameManager Instance;

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
                //showPopUp
                break;
        }
    }
    
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && State == GameState.WaitingInput)
        {
            _boardManager.SelectColumnToSpawn(_inputController.GetTile());
            
        }
    }


       
        
}
