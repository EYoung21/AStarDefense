using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 2f;
    public float maxHealth = 100f;
    private float currentHealth;

    [Header("Status Effects")]
    private float currentSlowAmount = 0f;
    private float baseSpeed;
    private bool isPoisoned = false;
    
    [SerializeField] private GameObject poisonEffectPrefab;
    [SerializeField] private GameObject freezeEffectPrefab;
    private GameObject activeStatusEffect;

    private void Start()
    {
        currentHealth = maxHealth;
        baseSpeed = moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        //add currency when enemy dies
        CurrencyManager.Instance.AddCurrency(2);
        Destroy(gameObject);
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        //apply the strongest slow effect
        if (slowAmount > currentSlowAmount)
        {
            currentSlowAmount = slowAmount;
            moveSpeed = baseSpeed * (1 - slowAmount);
            
            //visual feedback
            if (freezeEffectPrefab != null)
            {
                if (activeStatusEffect != null) Destroy(activeStatusEffect);
                activeStatusEffect = Instantiate(freezeEffectPrefab, transform);
            }
            
            StartCoroutine(SlowEffect(duration));
        }
    }

    public void ApplyPoison(float poisonDamage, float duration)
    {
        if (!isPoisoned)
        {
            isPoisoned = true;
            
            //visual feedback
            if (poisonEffectPrefab != null)
            {
                if (activeStatusEffect != null) Destroy(activeStatusEffect);
                activeStatusEffect = Instantiate(poisonEffectPrefab, transform);
            }
            
            StartCoroutine(PoisonEffect(poisonDamage, duration));
        }
    }

    private IEnumerator SlowEffect(float duration)
    {
        yield return new WaitForSeconds(duration);
        currentSlowAmount = 0f;
        moveSpeed = baseSpeed;
        if (activeStatusEffect != null) Destroy(activeStatusEffect);
    }

    private IEnumerator PoisonEffect(float poisonDamage, float duration)
    {
        float elapsed = 0;
        float tickRate = 0.5f; //damage every 0.5 seconds
        
        while (elapsed < duration)
        {
            TakeDamage(poisonDamage * tickRate);
            elapsed += tickRate;
            yield return new WaitForSeconds(tickRate);
        }
        
        isPoisoned = false;
        if (activeStatusEffect != null) Destroy(activeStatusEffect);
    }
} 