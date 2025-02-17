using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    [SerializeField] private GameObject SelectedUnitObject;


    public static UIManager Instance;

    void Awake() {
        Instance = this;
    }
    
    public void DisplayMainMenu() {
        // Show main menu UI, hide other UI elements
        // Play button, settings button, exit game button, highscore displayed in upper right corner, map customization options (color? etc.), game saves?

        //only continue if played has started a game (clicked play, etc.)


        throw new System.NotImplementedException();
        // GameManager.Instance.ChangeState(GameState.TurretSelection);
    }

    public void DisplayTurretSelection() {
        // Show turret selection UI, hide other UI elements
        // Turret selection options, turret preview, turret stats, select turret button, cancel button

        //only continue once player has selected a turret


        throw new System.NotImplementedException();
        // GameManager.Instance.ChangeState(GameState.GenerateGrid);
    }

    public void DisplayGeneralGameUI() {

        throw new System.NotImplementedException();

        // GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
        // TODO: Spawn game UI, hide other UI elements
        // currency, play / pause, health, wave # displayed, settings, etc.
    }

    public void ToggleShowSelectedUnit(BaseUnit unit) {
        if (unit == null) {
            SelectedUnitObject.SetActive(false);
            return;
        }
        
        SelectedUnitObject.GetComponentInChildren<TextMeshProUGUI>().text = unit.UnitName;
        SelectedUnitObject.SetActive(true);
    }

    //at some point in the following two functions, we need to call GameManager.Instance.ChangeState(GameState.GameOver);
    public void DisplayPlayerPrepTurnUI() {
        // Show player prep turn UI, hide other UI elements
        // Block / turret placement options, cancel button

        //only continue once played has clicked next round button


        throw new System.NotImplementedException();

        // GameManager.Instance.ChangeState(GameState.EnemyWaveTurn);
    }

    public void DisplayEnemyWaveTurnUI() {
        // Show enemy wave turn UI, hide other UI elements

        //only continue once enemy wave is finished

        throw new System.NotImplementedException();

        // GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
    }

    //TODO: this is only called when the player dies, so we need to figure out under what conditions to exit the above loop that goes between turns
    public void DisplayGameOver() {
        // Show game over UI, display final score, play again button, return to main menu button, exit game button, 

        throw new System.NotImplementedException();
    }
    
    

}
