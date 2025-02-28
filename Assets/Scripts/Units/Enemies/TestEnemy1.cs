using UnityEngine;

public class TestEnemy1 : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  //call the base class Start method
        health = 5;
        maxHealth = 5;
        damageItDoes = 1;
        healthBar.UpdateHealthBar(health, maxHealth);
        movement.moveSpeed = 2;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected override void OnEnemyHitTurret(Collider2D other) {
        Debug.Log("TestEnemy1 hit");
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        Destroy(gameObject);
    }
}
