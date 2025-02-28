using UnityEngine;

public class TestTurret1 : BaseTurret
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Start()
    // {
        
    // }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected override void Start() {
        base.Start();

        rateOfFire = 1;
        maxHealth = 100;
        health = 100;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    //override for virtual method
    // protected override void Update() {
    //     base.Update();
        
    // }


}
