using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class BaseTurret : BaseUnit
{
    // // start is called once before the first execution of update after the monobehaviour is created
    // void Start()
    // {
        
    // }

    // // update is called once per frame
    // void Update()
    // {
        
    // }


    [SerializeField] protected FloatingHealthBar healthBar;
    public GameObject projectilePrefab;

    public AudioClip shotSound;

    private float lastTimeFired = 0;

    public float rateOfFire;

    public Vector3 directionToEnemy;

    public float maxHealth;

    public float health;

    protected virtual void Start() {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
    }

    //forward mouse events to the occupied tile
    void OnMouseEnter() {
        if (OccupiedTile != null) {
            //call the tile's onmouseenter method
            OccupiedTile.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMouseExit() {
        if (OccupiedTile != null) {
            //call the tile's onmouseexit method
            OccupiedTile.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMouseDown() {
        if (OccupiedTile != null) {
            //call the tile's onmousedown method
            OccupiedTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        }
    }

    protected void Update() {
        if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            if ((lastTimeFired + 1 / rateOfFire) < Time.time) {
                lastTimeFired = Time.time;
                Fire();
            }
        }
    }

    protected virtual void Fire() {
        //store the instantiated projectile in a variable

        var enemies = UnitManager.Instance.GetAllCurrentEnemies();
        var closestEnemy = enemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();

        if (closestEnemy != null) {
            directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
        }

        float ydir = directionToEnemy.y;
        float xdir = directionToEnemy.x;

        float correctAngle = Mathf.Atan2(ydir, xdir) * Mathf.Rad2Deg; //finds angle in rads and converts to degrees

        correctAngle = correctAngle - 90;

        transform.rotation = Quaternion.AngleAxis(correctAngle, Vector3.forward); //the axis we want is the world's global z-axis, this equals to vector3.forward, or new vector3(0,0,1)


        GameObject projectile = Instantiate(projectilePrefab, transform.position, UnityEngine.Quaternion.identity);
        //set direction on the instantiated projectile, not the prefab
        projectile.GetComponent<BaseProjectile>().SetDirection(directionToEnemy);
    }


    public void RemoveHealth(float amount) {
        health -= amount;
        healthBar.UpdateHealthBar(health, maxHealth);

        Vector2 centerPosition = new Vector2(
            GridManager.Instance._width / 2, 
            GridManager.Instance._height / 2
        );
        
        Vector2 currPosition = new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        if (centerPosition == currPosition) { //if we're the center turret
            HealthManager.Instance.RemoveHealth(amount); //will handle game over if health <= 0
        }
        
        if (health <= 0) {
            Destroy(gameObject); //destroy the (noncentral) turret if it runs out of health
        }
        
    }

    public void AddHealth(float amount) {
        health += amount;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health > maxHealth) {
            health = maxHealth;
        }
    }
}
