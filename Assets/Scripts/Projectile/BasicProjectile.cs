using UnityEngine;

public class BasicProjectile : BaseProjectile
{
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        damage = 1;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected override void OnProjectileHitEnemy(Collider2D other) {
        other.GetComponent<BaseEnemy>().TakeDamage(damage);
    }

    

}
