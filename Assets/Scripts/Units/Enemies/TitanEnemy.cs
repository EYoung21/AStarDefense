using UnityEngine;

public class TitanEnemy : BaseEnemy
{
    //start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  //call the base class Start method
        health = 30;   //very high health
        maxHealth = 30;
        damageItDoes = 8;  //very high damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 1.5f;  //very low speed
        
        //log enemy creation for debugging
        Debug.Log($"TitanEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("TitanEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        //enemy dies after hitting turret
        UnitManager.Instance.enemyCount--;
        //no currency reward for hitting turret
        
        Destroy(gameObject);
    }
    
    //override TakeDamage to make the Titan more resistant to damage
    public new void TakeDamage(float damage) {
        //titans take 20% less damage from all sources
        float reducedDamage = damage * 0.8f;
        health -= reducedDamage;
        healthBar.UpdateHealthBar(health, maxHealth);
        
        Debug.Log($"Titan took {reducedDamage} damage (reduced from {damage})");
        
        if (health <= 0) {
            Destroy(gameObject);
            UnitManager.Instance.enemyCount--;
            CurrencyManager.Instance.AddCurrency(10);
        }
    }
} 