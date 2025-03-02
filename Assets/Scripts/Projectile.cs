using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private GameObject hitEffectPrefab;
    [SerializeField] private TrailRenderer trailRenderer;

    private Enemy target;
    private float damage;
    private ProjectileEffects effects;
    private bool hasHit = false;

    public void Initialize(Enemy target, float damage, ProjectileEffects effects)
    {
        this.target = target;
        this.damage = damage;
        this.effects = effects;

        //update visual effects based on projectile type
        if (trailRenderer != null)
        {
            if (effects.slowEffect > 0)
                trailRenderer.startColor = Color.cyan;
            else if (effects.poisonDamage > 0)
                trailRenderer.startColor = Color.green;
            else if (effects.splashRadius > 0)
                trailRenderer.startColor = Color.yellow;
        }
    }

    private void Update()
    {
        if (target == null || !target.gameObject.activeSelf)
        {
            Destroy(gameObject);
            return;
        }

        //move towards target
        Vector2 direction = (target.transform.position - transform.position).normalized;
        transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;

        //check if we've hit the target
        if (Vector2.Distance(transform.position, target.transform.position) < 0.1f)
        {
            Hit();
        }
    }

    private void Hit()
    {
        if (hasHit) return;
        hasHit = true;

        //apply direct damage
        target.TakeDamage(damage);

        //apply status effects
        if (effects.slowEffect > 0)
            target.ApplySlow(effects.slowEffect, 2f);

        if (effects.poisonDamage > 0)
            target.ApplyPoison(effects.poisonDamage, 3f);

        //handle splash damage
        if (effects.splashRadius > 0)
        {
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, effects.splashRadius);
            foreach (Collider2D collider in colliders)
            {
                Enemy splashTarget = collider.GetComponent<Enemy>();
                if (splashTarget != null && splashTarget != target)
                {
                    float splashDamage = damage * effects.splashDamageMultiplier;
                    splashTarget.TakeDamage(splashDamage);
                }
            }
        }

        //handle life leech
        if (effects.lifeLeechAmount > 0)
        {
            //find nearby friendly turrets to heal
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
            foreach (Collider2D collider in colliders)
            {
                BaseTurret turret = collider.GetComponent<BaseTurret>();
                if (turret != null)
                {
                    float healAmount = damage * effects.lifeLeechAmount;
                    turret.Heal(healAmount);
                }
            }
        }

        //spawn hit effect
        if (hitEffectPrefab != null)
        {
            Instantiate(hitEffectPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
} 