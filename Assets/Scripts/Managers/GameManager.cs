using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //this is a GameManager script, a good practice thing to have in video games that kind of holds everything together

    public static GameManager Instance;

    public GameState GameState;

    void Awake() {
        Instance = this;
    }

    void Start() {
        ChangeState(GameState.GenerateGrid);
    }

    public void ChangeState(GameState newState) {
        GameState = newState;
        switch (newState) {
            case GameState.GenerateGrid: //
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnInitialTurret:
                break;
            case GameState.PlayerPrepTurn: //block / turret placement
                break;
            case GameState.EnemyWaveTurn: //enemy spawning, movement towards center, astar, turret projectiles, etc.
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameState {
    GenerateGrid = 0,
    SpawnInitialTurret = 1,
    PlayerPrepTurn = 2,
    EnemyWaveTurn = 3,
}