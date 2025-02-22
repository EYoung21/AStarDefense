using UnityEngine;

public class BasicProjectile : BaseProjectile
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        lifetime = 3;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected virtual void OnTriggerEnter2D(Collider2D other) { //protected (and public) allows children to also have this method. protected only allows encapsulation for children
        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        Debug.Log("Is collison projectile -> obj");
        
        if (other.GetComponent<BaseUnit>().Faction == Faction.Enemy) {

            Debug.Log("Is collision projectile -> enemy");
            
            //EXPLOSION / HURT ANIMATION??
            
            other.GetComponent<BaseEnemy>().TakeDamage(damage);
            // TODO: destroy our game object after collision.
            Destroy(gameObject);
        }
    }
}
