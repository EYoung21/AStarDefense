using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //this is a GameManager script, a good practice thing to have in video games that kind of holds everything together

    public static GameManager Instance;

    public GameState GameState;

    public int globalNumberOfEnemiesToSpawn;

    void Awake() {
        Instance = this;
    }

    void Start() {
        ChangeState(GameState.GenerateGrid);

        //start with a moderate number of enemies in the first round
        globalNumberOfEnemiesToSpawn = 5;
    }

    public void ChangeState(GameState newState) {
        GameState = newState;
        switch (newState) {
            // case GameState.MainMenu:
            //     UIManager.Instance.DisplayMainMenu();
            //     // Show main menu UI, hide other UI elements
            //     // Play button, settings button, exit game button, highscore displayed in upper right corner, map customization options (color? etc.), game saves?
            //     break;
            // case GameState.TurretSelection:
            //     UIManager.Instance.DisplayTurretSelection();
            //     // Show turret selection UI, hide other UI elements
            //     // Turret selection options, turret preview, turret stats, select turret button, cancel button
            //     break;
            case GameState.GenerateGrid: //
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnInitialTurret:
                UnitManager.Instance.SpawnInitialTurret(); //these different game managers are singletons, so we can access them directly
                break;
            //TODO: Then we want to oscillate between the player prep turn and the enemy wave turn indefinetly until the player loses
            case GameState.PlayerPrepTurn: //block / turret placement
                // Increment round counter when returning to prep phase
                if (RoundManager.Instance != null)
                {
                    RoundManager.Instance.IncrementRound(1);
                    Debug.Log($"Round {RoundManager.Instance.round} started");
                }
                
                //increment the number of enemies to spawn for the next round
                if (RoundManager.Instance.round > 1) {
                    // Increase enemy count more aggressively in later rounds
                    if (RoundManager.Instance.round <= 5) {
                        globalNumberOfEnemiesToSpawn += 2;
                    } else if (RoundManager.Instance.round <= 10) {
                        globalNumberOfEnemiesToSpawn += 3;
                    } else {
                        globalNumberOfEnemiesToSpawn += 4;
                    }
                    Debug.Log("Increased enemy count to: " + globalNumberOfEnemiesToSpawn);
                }
                
                UIManager.Instance.DisplayPlayerPrepTurnUI();
                break;
            case GameState.EnemyWaveTurn: //enemy spawning, movement towards center, astar, turret projectiles, etc.
                // UIManager.Instance.DisplayEnemyWaveTurnUI();

                UnitManager.Instance.BeginEnemyWave();

                break;
            case GameState.GameOver:
                // Show game over UI, display final score, play again button, return to main menu button, exit game button,
                UIManager.Instance.DisplayGameOver();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
}

public enum GameState {
    MainMenu = 0,
    TurretSelection = 1,
    GenerateGrid = 2,
    SpawnInitialTurret = 3,
    SpawnGameUI = 4,
    PlayerPrepTurn = 5,
    EnemyWaveTurn = 6,
    GameOver = 7,
}