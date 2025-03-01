using UnityEngine;

public class DroneEnemy : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 2;    // Very low health
        maxHealth = 2;
        damageItDoes = 1;  // Very low damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 5;  // Medium-high speed
        
        // Log enemy creation for debugging
        Debug.Log($"DroneEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("DroneEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        // Add currency when enemy hits turret and dies
        UnitManager.Instance.enemyCount--;
        CurrencyManager.Instance.AddCurrency(1);  // Low currency reward
        
        Destroy(gameObject);
    }
} 