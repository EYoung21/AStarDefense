using UnityEngine;

public class BasicProjectile : BaseProjectile
{
    //effect values
    private float slowEffectAmount = 0.5f; //50% slow effect
    
    //particle effects for different upgrade types
    [SerializeField] private GameObject frostEffect;
    [SerializeField] private GameObject poisonEffect;
    [SerializeField] private GameObject splashEffect;

    
    protected override void Start()
    {
        base.Start();
        damage = 5;
        lifetime = 5;
        
        //enable appropriate visual effects based on projectile type
        UpdateVisualEffects();
    }

    //update visual effects based on the projectile's special effects
    private void UpdateVisualEffects()
    {
        //enable frost effect if applicable
        if (frostEffect != null) {
            frostEffect.SetActive(hasFrostEffect);
        }
        
        //enable poison effect if applicable
        if (poisonEffect != null) {
            poisonEffect.SetActive(hasPoisonEffect);
        }
        
        //enable splash effect if applicable
        if (splashEffect != null) {
            splashEffect.SetActive(hasSplashEffect);
        }
    }

    protected override void OnProjectileHitEnemy(Collider2D other) {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            //apply base damage
            enemy.TakeDamage(damage);
            
            //apply frost effect if applicable
            if (hasFrostEffect)
            {
                EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.ApplySlow(slowEffectAmount, 2.0f); //apply 50% slow for 2 seconds
                }
            }
            
            //apply poison effect if applicable
            if (hasPoisonEffect)
            {
                //apply poison damage over time (this would need to be implemented in the enemy class)
                Debug.Log($"Applied poison effect to enemy");
                //example: enemy.ApplyPoisonDamage(2.0f, 3.0f); //2 damage per second for 3 seconds
            }
            
            //apply splash effect if applicable
            if (hasSplashEffect)
            {
                //find all enemies in splash radius and damage them
                Debug.Log($"Applied splash effect around enemy");
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, 1.5f);
                foreach (var hitCollider in hitColliders)
                {
                    BaseEnemy splashEnemy = hitCollider.GetComponent<BaseEnemy>();
                    if (splashEnemy != null && splashEnemy != enemy)
                    {
                        //apply splash damage to nearby enemies
                        splashEnemy.TakeDamage(damage * 0.5f); //50% splash damage
                        Debug.Log($"Splash damage applied to nearby enemy");
                    }
                }
            }
        }
    }
}
