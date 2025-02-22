using UnityEngine;

public class TestEnemy1 : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 5;
    }

    protected void OnTriggerEnter2D(Collider2D other) { //protected (and public) allows children to also have this method. protected only allows encapsulation for children
        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        Debug.Log("On trigger entered. Enemy -> obj");
        
        if (other.GetComponent<BaseUnit>().Faction == Faction.Turret) {
            
            //EXPLOSION / HURT ANIMATION??
            Debug.Log("Turret should be hit. On trigger entered. Enemy ->hit-> turret");
            
            HealthManager.Instance.RemoveHealth(1);
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
