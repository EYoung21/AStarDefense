using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject SelectedUnitObject;
    
    [SerializeField] private GameObject roundObject;

    [SerializeField] private GameObject currencyObject;

    [SerializeField] private GameObject healthObject;

    [SerializeField] private GameObject nextRoundButton;

    public static UIManager Instance;

    void Awake() {
        Instance = this;
    }
    
    public void DisplayMainMenu() {
        //show main menu UI, hide other UI elements
        //play button, settings button, exit game button, highscore displayed in upper right corner, map customization options (color? etc.), game saves?

        //only continue if played has started a game (clicked play, etc.)


        throw new System.NotImplementedException();
        // GameManager.Instance.ChangeState(GameState.TurretSelection);
    }

    public void DisplayTurretSelection() {
        //show turret selection UI, hide other UI elements
        //turret selection options, turret preview, turret stats, select turret button, cancel button

        //only continue once player has selected a turret


        throw new System.NotImplementedException();
        // GameManager.Instance.ChangeState(GameState.GenerateGrid);
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
        //show player prep turn UI, hide other UI elements
        //score, block / turret placement options, cancel button

        //only continue once played has clicked next round button
        roundObject.SetActive(true);
        updateRoundUI();

        currencyObject.SetActive(true); 
        updateCurrencyUI();

        healthObject.SetActive(true);
        updateHealthUI();
        
        //show the Next Round button
        if (nextRoundButton != null) {
            nextRoundButton.SetActive(true);
        } else {
            Debug.LogWarning("Next Round button reference is missing!");
        }
    }

    public void updateRoundUI() { //will update the UI with what the round it
        roundObject.GetComponentInChildren<TextMeshProUGUI>().text = "Round: " + RoundManager.Instance.round.ToString();
    }

    public void updateCurrencyUI() { //will update the UI with what the round it
        currencyObject.GetComponentInChildren<TextMeshProUGUI>().text = "Currency: " + CurrencyManager.Instance.currency.ToString();
    }

    public void updateHealthUI() { //will update the UI with what the round it
        healthObject.GetComponentInChildren<TextMeshProUGUI>().text = "Health: " + HealthManager.Instance.health.ToString();
    }

    public void DisplayEnemyWaveTurnUI() {
        //show enemy wave turn UI, hide other UI elements

        //only continue once enemy wave is finished

        throw new System.NotImplementedException();

        // GameManager.Instance.ChangeState(GameState.PlayerPrepTurn);
    }

    //TODO: this is only called when the player dies, so we need to figure out under what conditions to exit the above loop that goes between turns
    public void DisplayGameOver() {
        //show game over UI, display final score, play again button, return to main menu button, exit game button, 

        throw new System.NotImplementedException();
    }
    
    //method to be called by the Next Round button
    public void OnNextRoundButtonClicked() {
        //hide the Next Round button
        if (nextRoundButton != null) {
            nextRoundButton.SetActive(false);
        }

        // We don't need to increment the round here as it's already set to 1 in RoundManager initialization
        // and will be incremented when returning to PlayerPrepTurn after the enemy wave
        
        //transition to enemy wave turn
        GameManager.Instance.ChangeState(GameState.EnemyWaveTurn);
    }
}
