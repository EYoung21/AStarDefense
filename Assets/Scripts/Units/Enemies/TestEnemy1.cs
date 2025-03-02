using UnityEngine;

public class TestEnemy1 : BaseEnemy
{
    //start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  //call the base class Start method
        health = 5;
        maxHealth = 5;
        damageItDoes = 2;
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 4;
    }

    protected override void OnEnemyHitTurret(Collider2D other) {
        Debug.Log("TestEnemy1 hit");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        
        //enemy dies after hitting turret
        UnitManager.Instance.enemyCount--;
        //no currency reward for hitting turret
        
        Destroy(gameObject);
    }
}
