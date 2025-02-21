using UnityEngine;
using System.Linq;

public class BaseTurret : BaseUnit
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }


    public GameObject projectilePrefab;

    public AudioClip shotSound;

    private float lastTimeFired = 0;

    public float rateOfFire;

    public Vector3 directionToEnemy;

    protected virtual void Start() {
        var enemies = UnitManager.Instance.GetAllCurrentEnemies();
        var closestEnemy = enemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();

        if (closestEnemy != null) {
            directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
        }
    }

    protected void Update() {
        if ((lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            Fire();
        }
    }

    protected void OnTriggerEnter2D(Collider2D other) { //protected (and public) allows children to also have this method. protected only allows encapsulation for children
        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        if (other.GetComponent<BaseEnemy>().Faction == Faction.Enemy) {

            //EXPLOSION / HURT ANIMATION??
            
            HealthManager.Instance.RemoveHealth(1);
        }
    }

    protected virtual void Fire() {
        // Store the instantiated projectile in a variable
        GameObject projectile = Instantiate(projectilePrefab, transform.position, UnityEngine.Quaternion.identity);
        // Set direction on the instantiated projectile, not the prefab
        projectile.GetComponent<BaseProjectile>().SetDirection(directionToEnemy);
    }




}
