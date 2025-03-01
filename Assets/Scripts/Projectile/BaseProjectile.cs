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
    
    // Reference to the sprite renderer for color changes
    protected SpriteRenderer spriteRenderer;
    
    // Special effect flags
    public bool hasFrostEffect = false;
    public bool hasPoisonEffect = false;
    public bool hasSplashEffect = false;

    public void SetDirection(Vector3 direction) {
        directionToEnemy = direction;
    }
    
    // Method to set the projectile color based on turret upgrades
    public virtual void SetProjectileColor(Color color) {
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }
        
        if (spriteRenderer != null) {
            spriteRenderer.color = color;
            Debug.Log($"Projectile color set to: {color}");
        } else {
            Debug.LogWarning("Could not find SpriteRenderer on projectile to change color");
        }
    }
    
    // Method to set special effects on the projectile
    public virtual void SetProjectileEffects(bool frost, bool poison, bool splash) {
        hasFrostEffect = frost;
        hasPoisonEffect = poison;
        hasSplashEffect = splash;
        
        // Log the effects for debugging
        if (frost || poison || splash) {
            Debug.Log($"Projectile effects set - Frost: {frost}, Poison: {poison}, Splash: {splash}");
        }
    }

    protected virtual void Start() {
        // Get the sprite renderer if not already assigned
        if (spriteRenderer == null) {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }
        }
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
        // Debug.Log("Is collison projectile -> obj");
        
        BaseUnit unit = other.GetComponent<BaseUnit>();
        if (unit != null && unit.Faction == Faction.Enemy) {
            // Debug.Log("Is collision projectile -> enemy");
            
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