using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;

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
    
    // Flag to track if this is the central turret
    protected bool isCentralTurret = false;
    
    // Flag to track if this turret is currently selected for manual control
    protected bool isSelected = false;

    public float maxHealth;

    public float health;

    protected virtual void Start() {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        
        // Check if this is the central turret
        Vector2 centerPosition = new Vector2(
            GridManager.Instance._width / 2, 
            GridManager.Instance._height / 2
        );
        
        Vector2 currPosition = new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        isCentralTurret = (centerPosition == currPosition);
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
        
        //if this is the central turret, toggle selection state
        //only allow selection for manual shooting during enemy turn
        if (isCentralTurret && GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            isSelected = true;
            Debug.Log("Central turret selected for manual control");
        }
    }
    
    // Method to handle selection state changes
    public void SetSelected(bool selected) {
        isSelected = selected;
        if (selected) {
            Debug.Log(gameObject.name + " selected");
        } else {
            Debug.Log(gameObject.name + " deselected");
        }
    }
    
    // Method to get the selected state for other scripts
    public void GetSelectedState(Action<bool> callback) {
        callback?.Invoke(isSelected);
    }

    protected void Update() {
        //check for space key to deselect turret
        if (Input.GetKeyDown(KeyCode.Space) && isSelected) {
            isSelected = false;
            Debug.Log("Turret deselected with space key - reverting to automatic mode");
            UnitManager.Instance.SetSelectedUnit(null);
            
            //force update of tile highlights by simulating mouse movement
            //this will make tiles respond to mouse hover again
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10; //set this to be the distance from the camera
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null) {
                hit.collider.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
                hit.collider.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
            }
        }
        
        //central turret behavior
        if (isCentralTurret) {
            //in player prep mode: always follow mouse but don't shoot
            if (GameManager.Instance.GameState == GameState.PlayerPrepTurn) {
                FollowMouse();
            }
            //in enemy wave mode: manual control if selected, automatic if not
            else if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
                if (isSelected) {
                    HandleManualControl();
                } else {
                    AutomaticFiring();
                }
            }
        }
        //non-central turrets: always automatic in enemy wave mode
        else if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            AutomaticFiring();
        }
    }
    
    // Just follow the mouse without firing
    protected virtual void FollowMouse() {
        // Get mouse position in world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure we're in the same z-plane
        
        // Calculate direction from turret to mouse
        Vector3 directionToMouse = (mousePos - transform.position).normalized;
        
        // Calculate rotation angle
        float ydir = directionToMouse.y;
        float xdir = directionToMouse.x;
        float angle = Mathf.Atan2(ydir, xdir) * Mathf.Rad2Deg - 90;
        
        // Apply rotation
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // Store direction for firing
        directionToEnemy = directionToMouse;
    }
    
    // Automatic firing at enemies
    protected virtual void AutomaticFiring() {
        if ((lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            Fire();
        }
    }
    
    // Handle manual aiming and firing for the central turret
    protected virtual void HandleManualControl() {
        // Follow the mouse
        FollowMouse();
        
        // Fire on left mouse button click with rate limit
        if (Input.GetMouseButtonDown(0) && (lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            FireManually();
        }
    }
    
    // Fire method for manual control
    protected virtual void FireManually() {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        projectile.GetComponent<BaseProjectile>().SetDirection(directionToEnemy);
        Debug.Log("Manual fire!");
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
