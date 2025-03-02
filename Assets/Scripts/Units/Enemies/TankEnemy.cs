using UnityEngine;

public class TankEnemy : BaseEnemy
{
    //start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  //call the base class Start method
        health = 15;   //high health
        maxHealth = 15;
        damageItDoes = 4;  //high damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 2;  //low speed
        
        //log enemy creation for debugging
        Debug.Log($"TankEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("TankEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        //enemy dies after hitting turret
        UnitManager.Instance.enemyCount--;
        //no currency reward for hitting turret
        
        Destroy(gameObject);
    }
} 