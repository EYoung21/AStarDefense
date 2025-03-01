using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class BaseTurret : BaseUnit
{
    // // start is called once before the first execution of update after the monobehaviour is created
    // void Start()
    // {
        
    // }

    // // update is called once per frame
    // void Update()
    // {
        
    // }


    [SerializeField] protected FloatingHealthBar healthBar;
    [Header("Base Stats")]
    [SerializeField] protected float baseAttackDamage = 8f;
    [SerializeField] protected float baseAttackSpeed = 0.8f;
    [SerializeField] protected float baseRange = 2.5f;
    [SerializeField] protected GameObject projectilePrefab;

    [Header("Current Stats")]
    protected float currentDamage;
    protected float currentAttackSpeed;
    protected float currentRange;
    protected float lastAttackTime;

    [Header("Status Effects")]
    protected float slowEffect = 0f;
    protected float poisonDamage = 0f;
    protected float splashRadius = 0f;
    protected float splashDamageMultiplier = 0f;
    protected float lifeLeechAmount = 0f;

    [Header("Visual Effects")]
    [SerializeField] protected GameObject rangeIndicator;
    [SerializeField] protected ParticleSystem upgradeParticles;
    [SerializeField] protected Color frostColor = Color.cyan;
    [SerializeField] protected Color poisonColor = Color.green;
    [SerializeField] protected Color splashColor = Color.yellow;
    [SerializeField] protected Color rapidFireColor = new Color(1.0f, 0.5f, 0.0f); // Orange
    [SerializeField] protected Color sniperColor = new Color(0.8f, 0.0f, 0.8f); // Purple
    
    protected SpriteRenderer spriteRenderer;
    protected List<Enemy> enemiesInRange = new List<Enemy>();
    protected Enemy currentTarget;

    public AudioClip shotSound;

    private float lastTimeFired = 0;

    public float rateOfFire;

    public Vector3 directionToEnemy;
    
    // Flag to track if this is the central turret
    protected bool isCentralTurret = false;
    
    // Flag to track if this turret is currently selected for manual control
    protected bool isSelected = false;

    public float maxHealth;

    public float health;

    protected Color currentProjectileColor = Color.white;
    protected bool hasFrostEffect = false;
    protected bool hasPoisonEffect = false;
    protected bool hasSplashEffect = false;
    protected bool hasRapidFireEffect = false;
    protected bool hasSniperEffect = false;

    protected virtual void Start() {
        healthBar = GetComponentInChildren<FloatingHealthBar>();
        
        // Check if this is the central turret
        Vector2 centerPosition = new Vector2(
            GridManager.Instance._width / 2, 
            GridManager.Instance._height / 2
        );
        
        Vector2 currPosition = new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        isCentralTurret = (centerPosition == currPosition);

        spriteRenderer = GetComponent<SpriteRenderer>();
        InitializeStats();
        StartCoroutine(ScanForTargets());
    }

    protected virtual void InitializeStats()
    {
        currentDamage = baseAttackDamage;
        currentAttackSpeed = baseAttackSpeed;
        currentRange = baseRange;
        UpdateRangeIndicator();
    }

    public virtual void UpdateStats(float damageMultiplier, float rangeMultiplier, float attackSpeedMultiplier)
    {
        Debug.Log($"BaseTurret.UpdateStats called with multipliers - Damage: {damageMultiplier}, Range: {rangeMultiplier}, Attack Speed: {attackSpeedMultiplier}");
        
        float oldDamage = currentDamage;
        float oldRange = currentRange;
        float oldAttackSpeed = currentAttackSpeed;
        
        // Make sure we're not using zero values
        if (damageMultiplier <= 0) damageMultiplier = 1f;
        if (rangeMultiplier <= 0) rangeMultiplier = 1f;
        if (attackSpeedMultiplier <= 0) attackSpeedMultiplier = 1f;
        
        currentDamage = baseAttackDamage * damageMultiplier;
        currentRange = baseRange * rangeMultiplier;
        currentAttackSpeed = baseAttackSpeed * attackSpeedMultiplier;
        
        Debug.Log($"Stats updated - Damage: {oldDamage} -> {currentDamage}, Range: {oldRange} -> {currentRange}, Attack Speed: {oldAttackSpeed} -> {currentAttackSpeed}");
        
        // Make sure the range indicator is updated
        UpdateRangeIndicator();
        
        // Play a visual effect to show the upgrade
        PlayUpgradeEffect();
        
        // If this is a significant upgrade, change the turret's appearance
        if (damageMultiplier > 1.5f || rangeMultiplier > 1.5f || attackSpeedMultiplier > 1.5f)
        {
            if (spriteRenderer != null)
            {
                // Add a slight tint to show the turret is upgraded
                Color oldColor = spriteRenderer.color;
                spriteRenderer.color = new Color(1.0f, 1.0f, 0.8f); // Slight yellow tint
                Debug.Log($"Turret appearance changed - Color: {oldColor} -> {spriteRenderer.color}");
            }
            else
            {
                Debug.LogWarning("Cannot change turret appearance: spriteRenderer is null!");
            }
        }
    }

    public virtual void UpdateEffects(float slowEffect, float poisonDamage, float splashRadius, float splashDamageMultiplier, float lifeLeechAmount)
    {
        Debug.Log($"BaseTurret.UpdateEffects called with effects - Slow: {slowEffect}, Poison: {poisonDamage}, Splash Radius: {splashRadius}, Splash Damage: {splashDamageMultiplier}, Life Leech: {lifeLeechAmount}");
        
        float oldSlowEffect = this.slowEffect;
        float oldPoisonDamage = this.poisonDamage;
        float oldSplashRadius = this.splashRadius;
        float oldSplashDamageMultiplier = this.splashDamageMultiplier;
        float oldLifeLeechAmount = this.lifeLeechAmount;
        
        this.slowEffect = slowEffect;
        this.poisonDamage = poisonDamage;
        this.splashRadius = splashRadius;
        this.splashDamageMultiplier = splashDamageMultiplier;
        this.lifeLeechAmount = lifeLeechAmount;
        
        Debug.Log($"Effects updated - Slow: {oldSlowEffect} -> {slowEffect}, Poison: {oldPoisonDamage} -> {poisonDamage}, Splash Radius: {oldSplashRadius} -> {splashRadius}");
        
        // Update the projectile effects
        UpdateProjectileEffects();
        
        // Apply visual changes based on effects
        if (spriteRenderer != null)
        {
            Color oldColor = spriteRenderer.color;
            
            // Change the turret color based on its strongest effect
            if (slowEffect > 0.3f)
            {
                spriteRenderer.color = new Color(0.7f, 0.9f, 1.0f); // Light blue for frost
                Debug.Log($"Applied frost color to turret: {oldColor} -> {spriteRenderer.color}");
            }
            else if (poisonDamage > 3f)
            {
                spriteRenderer.color = new Color(0.7f, 1.0f, 0.7f); // Light green for poison
                Debug.Log($"Applied poison color to turret: {oldColor} -> {spriteRenderer.color}");
            }
            else if (splashRadius > 1f)
            {
                spriteRenderer.color = new Color(1.0f, 1.0f, 0.7f); // Light yellow for splash
                Debug.Log($"Applied splash color to turret: {oldColor} -> {spriteRenderer.color}");
            }
        }
        else
        {
            Debug.LogWarning("Cannot change turret appearance: spriteRenderer is null!");
        }
        
        Debug.Log("Turret effects updated successfully");
    }

    protected virtual void UpdateProjectileEffects()
    {
        // Update projectile color based on strongest effect
        Color effectColor = Color.white;
        hasFrostEffect = false;
        hasPoisonEffect = false;
        hasSplashEffect = false;
        hasRapidFireEffect = false;
        hasSniperEffect = false;
        
        // Check which effects are active
        if (slowEffect > 0) {
            hasFrostEffect = true;
            effectColor = frostColor;
        }
        if (poisonDamage > 0) {
            hasPoisonEffect = true;
            effectColor = poisonColor;
        }
        if (splashRadius > 0) {
            hasSplashEffect = true;
            effectColor = splashColor;
        }
        
        // Check for stat-based upgrades
        if (currentAttackSpeed > baseAttackSpeed * 1.2f) {
            hasRapidFireEffect = true;
            // Only override color if no special effect is active
            if (!hasFrostEffect && !hasPoisonEffect && !hasSplashEffect) {
                effectColor = rapidFireColor;
            }
        }
        if (currentDamage > baseAttackDamage * 1.2f && currentRange > baseRange * 1.1f) {
            hasSniperEffect = true;
            // Only override color if no special effect is active
            if (!hasFrostEffect && !hasPoisonEffect && !hasSplashEffect && !hasRapidFireEffect) {
                effectColor = sniperColor;
            }
        }
        
        // Store the color for use when firing projectiles
        currentProjectileColor = effectColor;
        
        Debug.Log($"Updated projectile effects - Color: {currentProjectileColor}, Frost: {hasFrostEffect}, Poison: {hasPoisonEffect}, Splash: {hasSplashEffect}");
    }

    protected virtual void UpdateRangeIndicator()
    {
        if (rangeIndicator != null)
        {
            rangeIndicator.transform.localScale = Vector3.one * (currentRange * 2);
        }
    }

    protected virtual void PlayUpgradeEffect()
    {
        Debug.Log("Playing upgrade effect");
        if (upgradeParticles != null)
        {
            upgradeParticles.Play();
            
            // Play a sound effect if available
            if (shotSound != null)
            {
                AudioSource.PlayClipAtPoint(shotSound, transform.position, 0.5f);
            }
        }
        else
        {
            Debug.LogWarning("No upgrade particles assigned to turret");
        }
    }

    protected virtual IEnumerator ScanForTargets()
    {
        while (true)
        {
            // Update enemies in range
            enemiesInRange.Clear();
            Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, currentRange);
            
            foreach (Collider2D collider in colliders)
            {
                Enemy enemy = collider.GetComponent<Enemy>();
                if (enemy != null)
                {
                    enemiesInRange.Add(enemy);
                }
            }

            // Select target
            SelectTarget();

            // Attack if we have a target
            if (currentTarget != null && Time.time >= lastAttackTime + (1f / currentAttackSpeed))
            {
                Attack();
                lastAttackTime = Time.time;
            }

            yield return new WaitForSeconds(0.1f); // Scan every 0.1 seconds
        }
    }

    protected virtual void SelectTarget()
    {
        // Simple target selection - choose closest enemy
        float closestDistance = float.MaxValue;
        currentTarget = null;

        foreach (Enemy enemy in enemiesInRange)
        {
            if (enemy == null || !enemy.gameObject.activeSelf) continue;

            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = enemy;
            }
        }
    }

    protected virtual void Attack()
    {
        if (currentTarget == null || projectilePrefab == null) return;

        GameObject projectileObj = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        Projectile projectile = projectileObj.GetComponent<Projectile>();
        
        if (projectile != null)
        {
            // Set up projectile with all effects
            projectile.Initialize(currentTarget, currentDamage, new ProjectileEffects
            {
                slowEffect = slowEffect,
                poisonDamage = poisonDamage,
                splashRadius = splashRadius,
                splashDamageMultiplier = splashDamageMultiplier,
                lifeLeechAmount = lifeLeechAmount
            });
        }

        // Play attack sound if available
        if (shotSound != null)
        {
            AudioSource.PlayClipAtPoint(shotSound, transform.position);
        }
    }

    protected virtual void OnDrawGizmos()
    {
        // Draw range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, currentRange);
    }

    //forward mouse events to the occupied tile
    void OnMouseEnter() {
        if (OccupiedTile != null) {
            //call the tile's onmouseenter method
            OccupiedTile.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMouseExit() {
        if (OccupiedTile != null) {
            //call the tile's onmouseexit method
            OccupiedTile.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
        }
    }

    void OnMouseDown() {
        if (OccupiedTile != null) {
            //call the tile's onmousedown method
            OccupiedTile.SendMessage("OnMouseDown", SendMessageOptions.DontRequireReceiver);
        }
        
        //if this is the central turret, toggle selection state
        //only allow selection for manual shooting during enemy turn
        if (isCentralTurret && GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            isSelected = true;
            Debug.Log("Central turret selected for manual control");
        }
    }
    
    // Method to handle selection state changes
    public void SetSelected(bool selected) {
        isSelected = selected;
        if (selected) {
            Debug.Log(gameObject.name + " selected");
        } else {
            Debug.Log(gameObject.name + " deselected");
        }
    }
    
    // Method to get the selected state for other scripts
    public void GetSelectedState(Action<bool> callback) {
        callback?.Invoke(isSelected);
    }

    protected void Update() {
        //check for space key to deselect turret
        if (Input.GetKeyDown(KeyCode.Space) && isSelected) {
            isSelected = false;
            Debug.Log("Turret deselected with space key - reverting to automatic mode");
            UnitManager.Instance.SetSelectedUnit(null);
            
            //force update of tile highlights by simulating mouse movement
            //this will make tiles respond to mouse hover again
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10; //set this to be the distance from the camera
            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mousePos);
            RaycastHit2D hit = Physics2D.Raycast(worldPos, Vector2.zero);
            if (hit.collider != null) {
                hit.collider.SendMessage("OnMouseExit", SendMessageOptions.DontRequireReceiver);
                hit.collider.SendMessage("OnMouseEnter", SendMessageOptions.DontRequireReceiver);
            }
        }
        
        //central turret behavior
        if (isCentralTurret) {
            //in player prep mode: always follow mouse but don't shoot
            if (GameManager.Instance.GameState == GameState.PlayerPrepTurn) {
                FollowMouse();
            }
            //in enemy wave mode: manual control if selected, automatic if not
            else if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
                if (isSelected) {
                    HandleManualControl();
                } else {
                    AutomaticFiring();
                }
            }
        }
        //non-central turrets: always automatic in enemy wave mode
        else if (GameManager.Instance.GameState == GameState.EnemyWaveTurn) {
            AutomaticFiring();
        }
    }
    
    // Just follow the mouse without firing
    protected virtual void FollowMouse() {
        // Get mouse position in world coordinates
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0; // Ensure we're in the same z-plane
        
        // Calculate direction from turret to mouse
        Vector3 directionToMouse = (mousePos - transform.position).normalized;
        
        // Calculate rotation angle
        float ydir = directionToMouse.y;
        float xdir = directionToMouse.x;
        float angle = Mathf.Atan2(ydir, xdir) * Mathf.Rad2Deg - 90;
        
        // Apply rotation
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        
        // Store direction for firing
        directionToEnemy = directionToMouse;
    }
    
    // Automatic firing at enemies
    protected virtual void AutomaticFiring() {
        if ((lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            Fire();
        }
    }
    
    // Handle manual aiming and firing for the central turret
    protected virtual void HandleManualControl() {
        // Follow the mouse
        FollowMouse();
        
        // Fire on left mouse button click with rate limit
        if (Input.GetMouseButtonDown(0) && (lastTimeFired + 1 / rateOfFire) < Time.time) {
            lastTimeFired = Time.time;
            FireManually();
        }
    }
    
    // Fire method for manual control
    protected virtual void FireManually() {
        GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
        BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>();
        baseProjectile.SetDirection(directionToEnemy);
        
        // Set the projectile color and effects
        baseProjectile.SetProjectileColor(currentProjectileColor);
        baseProjectile.SetProjectileEffects(hasFrostEffect, hasPoisonEffect, hasSplashEffect);
        
        Debug.Log("Manual fire!");
    }

    protected virtual void Fire() {
        //store the instantiated projectile in a variable

        var enemies = UnitManager.Instance.GetAllCurrentEnemies();
        var closestEnemy = enemies.OrderBy(enemy => Vector3.Distance(transform.position, enemy.transform.position)).FirstOrDefault();

        if (closestEnemy != null) {
            directionToEnemy = (closestEnemy.transform.position - transform.position).normalized;
        }

        float ydir = directionToEnemy.y;
        float xdir = directionToEnemy.x;

        float correctAngle = Mathf.Atan2(ydir, xdir) * Mathf.Rad2Deg; //finds angle in rads and converts to degrees

        correctAngle = correctAngle - 90;

        transform.rotation = Quaternion.AngleAxis(correctAngle, Vector3.forward); //the axis we want is the world's global z-axis, this equals to vector3.forward, or new vector3(0,0,1)


        GameObject projectile = Instantiate(projectilePrefab, transform.position, UnityEngine.Quaternion.identity);
        //set direction on the instantiated projectile, not the prefab
        BaseProjectile baseProjectile = projectile.GetComponent<BaseProjectile>();
        baseProjectile.SetDirection(directionToEnemy);
        
        // Set the projectile color and effects
        baseProjectile.SetProjectileColor(currentProjectileColor);
        baseProjectile.SetProjectileEffects(hasFrostEffect, hasPoisonEffect, hasSplashEffect);
    }


    public void RemoveHealth(float amount) {
        health -= amount;
        healthBar.UpdateHealthBar(health, maxHealth);

        Vector2 centerPosition = new Vector2(
            GridManager.Instance._width / 2, 
            GridManager.Instance._height / 2
        );
        
        Vector2 currPosition = new Vector2(
            Mathf.RoundToInt(transform.position.x),
            Mathf.RoundToInt(transform.position.y)
        );

        if (centerPosition == currPosition) { //if we're the center turret
            HealthManager.Instance.RemoveHealth(amount); //will handle game over if health <= 0
        }
        
        if (health <= 0) {
            Destroy(gameObject); //destroy the (noncentral) turret if it runs out of health
        }
        
    }

    public void AddHealth(float amount) {
        health += amount;
        healthBar.UpdateHealthBar(health, maxHealth);
        if (health > maxHealth) {
            health = maxHealth;
        }
    }

    // Add healing method for life leech
    public virtual void Heal(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
        if (healthBar != null)
        {
            healthBar.UpdateHealthBar(health, maxHealth);
        }
    }

    protected virtual void UpdateTurretStats()
    {
        // This method should be removed or renamed since it's now handled by TurretUpgrade
    }
}
