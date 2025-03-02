using UnityEngine;

public class BasicProjectile : BaseProjectile
{
    // Effect values
    private float slowEffectAmount = 0.5f; // 50% slow effect
    
    // Particle effects for different upgrade types
    [SerializeField] private GameObject frostEffect;
    [SerializeField] private GameObject poisonEffect;
    [SerializeField] private GameObject splashEffect;
    
    // // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        damage = 1;
        lifetime = 5;
        
        // Enable appropriate visual effects based on projectile type
        UpdateVisualEffects();
    }

    // Update visual effects based on the projectile's special effects
    private void UpdateVisualEffects()
    {
        // Enable frost effect if applicable
        if (frostEffect != null) {
            frostEffect.SetActive(hasFrostEffect);
        }
        
        // Enable poison effect if applicable
        if (poisonEffect != null) {
            poisonEffect.SetActive(hasPoisonEffect);
        }
        
        // Enable splash effect if applicable
        if (splashEffect != null) {
            splashEffect.SetActive(hasSplashEffect);
        }
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }

    protected override void OnProjectileHitEnemy(Collider2D other) {
        BaseEnemy enemy = other.GetComponent<BaseEnemy>();
        if (enemy != null)
        {
            // Apply base damage
            enemy.TakeDamage(damage);
            
            // Apply frost effect if applicable
            if (hasFrostEffect)
            {
                EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
                if (movement != null)
                {
                    movement.ApplySlow(slowEffectAmount, 2.0f); // Apply 50% slow for 2 seconds
                }
            }
            
            // Apply poison effect if applicable
            if (hasPoisonEffect)
            {
                // Apply poison damage over time (this would need to be implemented in the enemy class)
                Debug.Log($"Applied poison effect to enemy");
                // Example: enemy.ApplyPoisonDamage(2.0f, 3.0f); // 2 damage per second for 3 seconds
            }
            
            // Apply splash effect if applicable
            if (hasSplashEffect)
            {
                // Find all enemies in splash radius and damage them
                Debug.Log($"Applied splash effect around enemy");
                Collider2D[] hitColliders = Physics2D.OverlapCircleAll(enemy.transform.position, 1.5f);
                foreach (var hitCollider in hitColliders)
                {
                    BaseEnemy splashEnemy = hitCollider.GetComponent<BaseEnemy>();
                    if (splashEnemy != null && splashEnemy != enemy)
                    {
                        // Apply splash damage to nearby enemies
                        splashEnemy.TakeDamage(damage * 0.5f); // 50% splash damage
                        Debug.Log($"Splash damage applied to nearby enemy");
                    }
                }
            }
        }
    }

    

}
