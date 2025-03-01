using UnityEngine;

public class SpeedEnemy : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 3;    // Low health
        maxHealth = 3;
        damageItDoes = 1;  // Low damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 7;  // High speed
        
        // Log enemy creation for debugging
        Debug.Log($"SpeedEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("SpeedEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        // Add currency when enemy hits turret and dies
        UnitManager.Instance.enemyCount--;
        CurrencyManager.Instance.AddCurrency(2);
        
        Destroy(gameObject);
    }
} 