# Enemy Types in A* Defense

This document outlines the different enemy types in the game, their characteristics, and how they are spawned throughout the game's progression.

## Enemy Types

### 1. TestEnemy1 (BlueStar)
- **Health**: 5
- **Damage**: 2
- **Speed**: 4
- **Currency Reward**: 2
- **Description**: The original basic enemy, balanced in all aspects.
- **Spawn Pattern**: Common in early rounds, becomes less frequent as the game progresses.

### 2. SpeedEnemy
- **Health**: 3
- **Damage**: 1
- **Speed**: 7
- **Currency Reward**: 2
- **Description**: Fast-moving enemy that can quickly reach your turrets but has low health and damage.
- **Spawn Pattern**: Starts appearing from round 2, peaks around round 5.

### 3. TankEnemy
- **Health**: 15
- **Damage**: 4
- **Speed**: 2
- **Currency Reward**: 4
- **Description**: Slow-moving but tough enemy with high damage. Prioritize these enemies as they can deal significant damage to your turrets.
- **Spawn Pattern**: Starts appearing from round 3, becomes more common in later rounds.

### 4. SoldierEnemy
- **Health**: 7
- **Damage**: 2
- **Speed**: 4
- **Currency Reward**: 3
- **Description**: Well-balanced enemy with moderate stats in all categories.
- **Spawn Pattern**: Starts appearing from round 2, common throughout the game.

### 5. TitanEnemy
- **Health**: 30
- **Damage**: 8
- **Speed**: 1.5
- **Currency Reward**: 10
- **Special Ability**: Takes 20% less damage from all sources
- **Description**: Mini-boss enemy that appears occasionally in later rounds. Very tough with high damage, but slow-moving.
- **Spawn Pattern**: Can appear from round 5 onwards, guaranteed to appear every 5 rounds (5, 10, 15, etc.) and has a 20% chance to appear in other rounds after round 5.

## Enemy Scaling

Enemies become stronger as the game progresses:

- **Health Scaling**: Starting from round 3, enemy health increases by 15% per round.
- **Damage Scaling**: Starting from round 4, enemy damage increases by 10% per round.

## Difficulty Tiers

The game has 5 difficulty tiers that affect enemy spawning:

1. **Novice** (Rounds 1-4): Mostly basic enemies.
2. **Challenging** (Rounds 5-9): More variety, Titans start appearing.
3. **Difficult** (Rounds 10-14): Higher proportion of tough enemies.
4. **Extreme** (Rounds 15-19): Significantly stronger enemies with high scaling.
5. **Nightmare** (Rounds 20+): The ultimate challenge with maximum difficulty.

## Spawn Patterns

- Early rounds feature mostly basic enemies (BlueStar/TestEnemy1).
- As rounds progress, more specialized enemies appear.
- Titans appear at strategic moments to create challenging encounters.
- Later rounds feature a higher proportion of Tank and Soldier enemies.

## Strategy Tips

- **Against SpeedEnemy**: Use frost turrets to slow them down.
- **Against TankEnemy**: Focus fire with high-damage turrets.
- **Against TitanEnemy**: Prepare in advance with multiple turrets and slow effects. 