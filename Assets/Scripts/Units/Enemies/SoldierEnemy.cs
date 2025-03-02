using UnityEngine;

public class SoldierEnemy : BaseEnemy
{
    //start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  //call the base class Start method
        health = 7;    //medium health
        maxHealth = 7;
        damageItDoes = 2;  //medium damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 4;  //medium speed
        
        //log enemy creation for debugging
        Debug.Log($"SoldierEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("SoldierEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        //enemy dies after hitting turret
        UnitManager.Instance.enemyCount--;
        //no currency reward for hitting turret
        
        Destroy(gameObject);
    }
} 