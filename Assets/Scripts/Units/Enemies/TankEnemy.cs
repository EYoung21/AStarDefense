using UnityEngine;

public class TankEnemy : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 15;   // High health
        maxHealth = 15;
        damageItDoes = 4;  // High damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 2;  // Low speed
        
        // Log enemy creation for debugging
        Debug.Log($"TankEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("TankEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        // Add currency when enemy hits turret and dies
        UnitManager.Instance.enemyCount--;
        CurrencyManager.Instance.AddCurrency(4);  // More currency for defeating a tank
        
        Destroy(gameObject);
    }
} 