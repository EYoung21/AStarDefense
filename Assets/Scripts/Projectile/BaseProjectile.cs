using UnityEngine;
using System.Linq;

public class BaseProjectile : MonoBehaviour
{
    public string projectileType;

    [SerializeField] private float speed;

    [SerializeField] private float damage;

    private Vector3 directionToEnemy;

    public void SetDirection(Vector3 direction) {
        directionToEnemy = direction;
    }

    protected void Start() {
    }

    protected void Update() {
        transform.position += directionToEnemy * speed * Time.deltaTime;
    }

    protected virtual void OnTriggerEnter2D(Collider2D other) { //protected (and public) allows children to also have this method. protected only allows encapsulation for children
        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        Debug.Log("Is collison");
        
        if (other.GetComponent<BaseEnemy>().Faction == Faction.Enemy) {

            Debug.Log("Is enemy faction");
            
            //EXPLOSION / HURT ANIMATION??
            
            other.GetComponent<BaseEnemy>().TakeDamage(damage);
        }
    }
    

}