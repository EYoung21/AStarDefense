# Projectile Color System

This document explains how the projectile color system works in the game, allowing projectiles to visually reflect the turret's upgrades.

## Overview

When a turret is upgraded with special effects (Frost, Poison, Splash) or stat improvements (Rapid Fire, Sniper), both the turret and its projectiles change color to reflect these upgrades. This provides visual feedback to the player about the turret's capabilities.

## Color Scheme

The following colors are used for different upgrade types:

- **Frost (Slow)**: Cyan/Light Blue - Slows down enemies
- **Poison**: Green - Deals damage over time
- **Splash**: Yellow - Damages enemies in an area
- **Rapid Fire**: Orange - Increased attack speed
- **Sniper**: Purple - Increased damage and range

## Priority System

Since a turret can have multiple upgrades, there's a priority system for determining which color to use:

1. Special effects (Frost, Poison, Splash) take priority over stat upgrades
2. Among special effects, the priority is: Frost > Poison > Splash
3. Among stat upgrades, the priority is: Rapid Fire > Sniper

## Implementation Details

### BaseTurret.cs

- The `UpdateProjectileEffects()` method determines which color to use based on the turret's upgrades
- The `currentProjectileColor` field stores the current color to apply to projectiles
- Boolean flags track which effects are active (hasFrostEffect, hasPoisonEffect, etc.)
- When firing a projectile, the turret passes its current color and effect flags to the projectile

### BaseProjectile.cs

- The `SetProjectileColor(Color color)` method applies the color to the projectile's sprite
- The `SetProjectileEffects(bool frost, bool poison, bool splash)` method sets flags for special effects
- The projectile uses these flags to determine what effects to apply when hitting enemies

### BasicProjectile.cs

- Implements the specific behavior for each effect type when hitting enemies
- Can enable/disable visual effect GameObjects based on the active effects

## Testing

You can test the projectile color system using the `ProjectileColorTest.cs` script:

1. Attach the script to any GameObject in your scene
2. Assign a turret to the `testTurret` field, or let it find one automatically
3. Use the following keys to test different effects:
   - F: Test Frost effect
   - P: Test Poison effect
   - S: Test Splash effect
   - R: Test Rapid Fire effect
   - N: Test Sniper effect
   - X: Reset all effects

## Adding New Effects

To add a new effect type with its own color:

1. Add a new color field in `BaseTurret.cs` (e.g., `[SerializeField] protected Color newEffectColor = Color.magenta;`)
2. Add a new boolean flag to track the effect (e.g., `protected bool hasNewEffect = false;`)
3. Update the `UpdateProjectileEffects()` method to check for the new effect and set the appropriate color
4. Update the `Fire()` and `FireManually()` methods to pass the new effect flag to the projectile
5. Update the `BaseProjectile.cs` class to handle the new effect
6. Implement the specific behavior for the new effect in `BasicProjectile.cs` 