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
        //if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        Debug.Log("On trigger entered. Enemy -> obj");
        
        //get the center position (where the turret should be)
        Vector2 centerPosition = new Vector2(
            GridManager.Instance._width / 2, 
            GridManager.Instance._height / 2
        );
        
        //get the position of the other object
        Vector2 otherPosition = new Vector2(
            Mathf.RoundToInt(other.transform.position.x),
            Mathf.RoundToInt(other.transform.position.y)
        );
        
        //check if the other object is at the center position
        if (otherPosition == centerPosition) {
            //EXPLOSION / HURT ANIMATION??
            Debug.Log("Center turret hit. On trigger entered. Enemy ->hit-> center");
            
            HealthManager.Instance.RemoveHealth(1);
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
