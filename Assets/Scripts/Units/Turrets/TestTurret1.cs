using UnityEngine;

public class TestTurret1 : BaseTurret
{
    protected override void Start() {
        base.Start();

        rateOfFire = 1;
        maxHealth = 20;
        health = 20;
        healthBar.UpdateHealthBar(health, maxHealth);
    }

    //override for virtual method
    //protected override void Update() {
    //     base.Update();
        
    // }


}
