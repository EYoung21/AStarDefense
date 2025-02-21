using UnityEngine;

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

    protected void Update() {
        if ((lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            Fire();
        }
    }

    protected void OnTriggerEnter2D(Collider2D other) { //protected allows children to also have this method
        // if the other object has the Asteroid script (we overlap with an asteroid), the destroy the ship and restard the game
        if (other.Faction == Faction.Enemy) {

            //EXPLOSION / HURT ANIMATION??
            
            HealthManager.Instance.RemoveHealth(1);
        }
    }

    void Fire() {
        var enemies = UnitManager.Instance.GetAllCurrentEnemies();
        var closestEnemy = enemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();

        Vector3 directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;

        Instantiate(projectilePrefab, transform.position + directionToEnemy, UnityEngine.Quaternion.identity);
    }




}
