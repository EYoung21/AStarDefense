using UnityEngine;

public class TestEnemy1 : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 5;
        maxHealth = 5;
        damageItDoes = 1;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected override void OnEnemyHitTurret(Collider2D other) {
        other.GetComponent<BaseTurret>().RemoveHealth(damageItDoes);
        Debug.Log("TestEnemy1 hit");
    }
}
