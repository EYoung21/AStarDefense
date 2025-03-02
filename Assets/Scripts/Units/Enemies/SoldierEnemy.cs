using UnityEngine;

public class SoldierEnemy : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 7;    // Medium health
        maxHealth = 7;
        damageItDoes = 2;  // Medium damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 4;  // Medium speed
        
        // Log enemy creation for debugging
        Debug.Log($"SoldierEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("SoldierEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        // Enemy dies after hitting turret
        UnitManager.Instance.enemyCount--;
        // No currency reward for hitting turret
        
        Destroy(gameObject);
    }
} 