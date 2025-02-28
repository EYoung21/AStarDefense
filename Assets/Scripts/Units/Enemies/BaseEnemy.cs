using UnityEngine;

public class BaseEnemy : BaseUnit
{

    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;


    [SerializeField] protected float damageItDoes;

    [SerializeField] protected FloatingHealthBar healthBar;
    [SerializeField] protected EnemyMovement movement;

    //start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        movement = GetComponent<EnemyMovement>();
        if (movement == null)
        {
            movement = gameObject.AddComponent<EnemyMovement>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage) {
        health -= damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0) {
            Destroy(gameObject);
            UnitManager.Instance.enemyCount--;
            CurrencyManager.Instance.AddCurrency(1);
        }
    }



    protected void OnTriggerEnter2D(Collider2D other) { //protected (and public) allows children to also have this method. protected only allows encapsulation for children
        if (other.GetComponent<BaseTurret>() != null) {
            //EXPLOSION / HURT ANIMATION??
            Debug.Log("Center turret hit. On trigger entered. Enemy ->hit-> center");
            
            OnEnemyHitTurret(other);
        }
    }

    protected virtual void OnEnemyHitTurret(Collider2D other) {
        //individual to each enemy
    }
}
