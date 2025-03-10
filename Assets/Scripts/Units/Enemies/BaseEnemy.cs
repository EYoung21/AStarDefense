using UnityEngine;

public class BaseEnemy : BaseUnit
{
    public bool IsAlive { get; private set; } = true; //default to alive


    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;


    [SerializeField] protected float damageItDoes;

    [SerializeField] protected int baseCurrencyReward = 3; //base currency reward for this enemy type
    [SerializeField] protected bool useRoundScaledReward = true; //whether to scale reward with round number

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
            //play enemy despawn sound
            if (SFXManager.Instance != null) {
                SFXManager.Instance.PlayEnemyDespawnSound();
            }
            
            Destroy(gameObject);
            UnitManager.Instance.enemyCount--;
            
            //calculate currency reward based on enemy type and round if applicable
            int reward = baseCurrencyReward;
            
            //apply round scaling if enabled and RoundManager exists
            if (useRoundScaledReward && RoundManager.Instance != null) {
                //increase reward based on round number
                int roundBonus = Mathf.FloorToInt(RoundManager.Instance.round / 5);
                reward += roundBonus;
            }
            
            //add currency
            CurrencyManager.Instance.AddCurrency(reward);
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
        //play enemy despawn sound
        if (SFXManager.Instance != null) {
            SFXManager.Instance.PlayEnemyDespawnSound();
        }
        
        //individual to each enemy
    }
}
