using UnityEngine;

public class RoundManager : MonoBehaviour
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    public static RoundManager Instance;

    void Awake() {
        Instance = this;
    }  

    public int round = 1; //we will increment round as rounds end

    public void IncrementRound(int amount) {
        round += amount;
    }
}
