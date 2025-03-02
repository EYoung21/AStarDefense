using UnityEngine;

public class SpeedEnemy : BaseEnemy
{
    //start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  //call the base class Start method
        health = 3;    //low health
        maxHealth = 3;
        damageItDoes = 1;  //low damage
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 7;  //high speed
        
        //log enemy creation for debugging
        Debug.Log($"SpeedEnemy created with health: {health}, damage: {damageItDoes}, speed: {movement.moveSpeed}");
    }

    protected override void OnEnemyHitTurret(Collider2D other)
    {
        Debug.Log("SpeedEnemy hit turret");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        //enemy dies after hitting turret
        UnitManager.Instance.enemyCount--;
        //no currency reward for hitting turret
        
        Destroy(gameObject);
    }
} 