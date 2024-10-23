using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum GameState { MoveAI, MovePlayer, FireAI, FirePlayer}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    UIManager _UIManager;
    PlayerController _playerController;
    AIMovement _aiMovement;

    GameState state = GameState.MoveAI;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void StartGame(UIManager ui)
    {
        _UIManager = ui;
        Invoke("Move", 0.5f); //Give a small delay so AI dont start very fast
    }
    private void Move()
    {
        _aiMovement = GameObject.FindGameObjectsWithTag("Enemy")[0].GetComponent<AIMovement>();
        _playerController = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerController>();
        _aiMovement.StartMove();
        _UIManager.SetTurn(state);
    }

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape))
        {
            Application.Quit();
        }
    }
    void UpdateState()
    {
        if (state == GameState.FirePlayer) state = 0;
        else state++;

        _UIManager.SetTurn(state);

        switch(state)
        {
            case GameState.MoveAI:
                _aiMovement.StartMove();
                break;

            case GameState.MovePlayer:
                _playerController.StartMove();
                break;

            case GameState.FireAI:
                _aiMovement.StartShooting();
                break;

            case GameState.FirePlayer:
                _playerController.StartShooting();
                break;

            default:
                Debug.LogWarning("Unknown state");
                break;
        }
    }


    //Could be one method just in case wanna do something different for future
    public void TankMoved(TankType t)
    {
        UpdateState();
    }

    public void TankFired(TankType t)
    {
        UpdateState();
    }
}
