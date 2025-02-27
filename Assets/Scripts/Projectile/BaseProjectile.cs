using UnityEngine;
using System.Linq;

public class BaseProjectile : MonoBehaviour
{
    public string projectileType;

    [SerializeField] private float speed;

    [SerializeField] protected float damage; //protected allows children classes to access

    [SerializeField] protected float lifetime;
    //then can set lifetime to specific values in each projectile instance (wouldn't have to destrpy offscreen
    //could just time how long it takes to get to edge then destroy it. or maybe use this to implement range).

    private Vector3 directionToEnemy;

    public void SetDirection(Vector3 direction) {
        directionToEnemy = direction;
    }

    protected virtual void Start() {
    }

    protected void Update() {
        transform.position += directionToEnemy * speed * Time.deltaTime;

        lifetime -= Time.deltaTime;
		if (lifetime <= 0) {
			Destroy(gameObject);
		}
    }
    

    protected void OnTriggerEnter2D(Collider2D other) { //protected (and public) allows children to also have this method. protected only allows encapsulation for children
        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        Debug.Log("Is collison projectile -> obj");
        
        if (other.GetComponent<BaseUnit>().Faction == Faction.Enemy) {

            Debug.Log("Is collision projectile -> enemy");
            
            //EXPLOSION / HURT ANIMATION??
            
            OnProjectileHitEnemy(other);
            // TODO: destroy our game object after collision.
            Destroy(gameObject);
        }
    }

    protected virtual void OnProjectileHitEnemy(Collider2D other) {
        //individual to each projectile
    }
    

}