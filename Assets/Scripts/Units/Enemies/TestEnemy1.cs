using UnityEngine;

public class TestEnemy1 : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 5;
        damageItDoes = 1;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected override void OnEnemyHitTurret() {
        HealthManager.Instance.RemoveHealth(damageItDoes);
        Debug.Log("TestEnemy1 hit");
    }
}
