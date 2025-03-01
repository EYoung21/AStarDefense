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
        
        //apply round-based scaling to health and damage
        if (RoundManager.Instance != null)
        {
            float healthMultiplier = RoundManager.Instance.GetEnemyHealthMultiplier();
            float damageMultiplier = RoundManager.Instance.GetEnemyDamageMultiplier();
            
            health *= healthMultiplier;
            maxHealth *= healthMultiplier;
            damageItDoes *= damageMultiplier;
            
            //update health bar with new max health
            if (healthBar != null)
            {
                healthBar.UpdateHealthBar(health, maxHealth);
            }
            
            if (healthMultiplier > 1.0f || damageMultiplier > 1.0f)
            {
                Debug.Log($"Enemy scaled: Health x{healthMultiplier}, Damage x{damageMultiplier}");
            }
        }
    }

    //update is called once per frame
    void Update()
    {
        
    }

    public void TakeDamage(float damage) {
        health -= damage;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health <= 0) {
            Destroy(gameObject);
            UnitManager.Instance.enemyCount--;
            CurrencyManager.Instance.AddCurrency(2);
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
