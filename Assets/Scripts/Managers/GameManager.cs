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
    
    [Header("Round Settings")]
    [Tooltip("Base number of enemies in round 1")]
    public int baseEnemyCount = 5;
    
    [Tooltip("Additional enemies per round for rounds 2-5")]
    public int earlyRoundEnemyIncrement = 2;
    
    [Tooltip("Additional enemies per round for rounds 6-10")]
    public int midRoundEnemyIncrement = 3;
    
    [Tooltip("Additional enemies per round for rounds 11+")]
    public int lateRoundEnemyIncrement = 4;
    
    [Header("Game State")]
    public bool gameIsPaused = false;
    public float gameSpeed = 1.0f;

    void Awake() {
        Instance = this;
    }

    void Start() {
        ChangeState(GameState.GenerateGrid);

        //start with a moderate number of enemies in the first round
        globalNumberOfEnemiesToSpawn = baseEnemyCount;
    }
    
    void Update() {
        //handle game speed controls
        if (Input.GetKeyDown(KeyCode.P)) {
            TogglePause();
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha1)) {
            SetGameSpeed(1.0f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) {
            SetGameSpeed(1.5f);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) {
            SetGameSpeed(2.0f);
        }
    }
    
    public void TogglePause() {
        gameIsPaused = !gameIsPaused;
        Time.timeScale = gameIsPaused ? 0 : gameSpeed;
        Debug.Log(gameIsPaused ? "Game Paused" : "Game Resumed");
    }
    
    public void SetGameSpeed(float speed) {
        gameSpeed = speed;
        if (!gameIsPaused) {
            Time.timeScale = gameSpeed;
        }
        Debug.Log($"Game speed set to {gameSpeed}x");
    }

    public void ChangeState(GameState newState) {
        GameState = newState;
        switch (newState) {
            // case GameState.MainMenu:
            //     UIManager.Instance.DisplayMainMenu();
            //     //show main menu UI, hide other UI elements
            //     //play button, settings button, exit game button, highscore displayed in upper right corner, map customization options (color? etc.), game saves?
            //     break;
            // case GameState.TurretSelection:
            //     UIManager.Instance.DisplayTurretSelection();
            //     //show turret selection UI, hide other UI elements
            //     //turret selection options, turret preview, turret stats, select turret button, cancel button
            //     break;
            case GameState.GenerateGrid: //
                GridManager.Instance.GenerateGrid();
                break;
            case GameState.SpawnInitialTurret:
                UnitManager.Instance.SpawnInitialTurret(); //these different game managers are singletons, so we can access them directly
                break;
            //TODO: Then we want to oscillate between the player prep turn and the enemy wave turn indefinetly until the player loses
            case GameState.PlayerPrepTurn: //block / turret placement
                //log the current round
                if (RoundManager.Instance != null)
                {
                    Debug.Log($"Round {RoundManager.Instance.round} started");
                }
                
                //increment the number of enemies to spawn for the next round
                if (RoundManager.Instance.round > 1) {
                    //increase enemy count more aggressively in later rounds
                    int currentRound = RoundManager.Instance.round;
                    
                    if (currentRound <= 5) {
                        globalNumberOfEnemiesToSpawn += earlyRoundEnemyIncrement;
                    } else if (currentRound <= 10) {
                        globalNumberOfEnemiesToSpawn += midRoundEnemyIncrement;
                    } else {
                        globalNumberOfEnemiesToSpawn += lateRoundEnemyIncrement;
                    }
                    
                    //add bonus enemies at milestone rounds
                    if (currentRound % 5 == 0) {
                        globalNumberOfEnemiesToSpawn += 2; //extra enemies every 5 rounds
                    }
                    
                    Debug.Log($"Round {currentRound}: Spawning {globalNumberOfEnemiesToSpawn} enemies");
                }
                
                UIManager.Instance.DisplayPlayerPrepTurnUI();
                break;
            case GameState.EnemyWaveTurn: //enemy spawning, movement towards center, astar, turret projectiles, etc.
                // UIManager.Instance.DisplayEnemyWaveTurnUI();

                UnitManager.Instance.BeginEnemyWave();

                break;
            case GameState.GameOver:
                //show game over UI, display final score, play again button, return to main menu button, exit game button,
                UIManager.Instance.DisplayGameOver();
                
                //stop time when game is over
                Time.timeScale = 0;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }
    
    //helper method to get the current round number
    public int GetCurrentRound() {
        return RoundManager.Instance != null ? RoundManager.Instance.round : 0;
    }
    
    //helper method to get the current difficulty tier
    public int GetCurrentDifficultyTier() {
        return RoundManager.Instance != null ? RoundManager.Instance.difficultyTier : 1;
    }
    
    //helper method to get a description of the current round
    public string GetCurrentRoundDescription() {
        if (RoundManager.Instance == null) return "Round information not available";
        return RoundManager.Instance.GetRoundDifficultyDescription();
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