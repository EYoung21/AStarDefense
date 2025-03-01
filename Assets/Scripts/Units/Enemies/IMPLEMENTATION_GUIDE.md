# Enemy System Implementation Guide

This guide explains how to set up the new enemy types in your Unity project.

## Step 1: Create ScriptableObjects for Each Enemy Type

1. In Unity, right-click in the Project window > Create > ScriptableObject > Scriptable Unit
2. Create one for each enemy type:
   - BlueStar (original enemy)
   - SpeedEnemy
   - TankEnemy
   - SoldierEnemy
   - DroneEnemy
   - TitanEnemy

3. For each ScriptableObject:
   - Set Faction to "Enemy"
   - Drag the appropriate prefab to the "Unit Prefab" field

## Step 2: Set Up Enemy Prefabs

For each enemy type, create a prefab with the following components:
- The appropriate enemy script (SpeedEnemy, TankEnemy, etc.)
- Collider2D component
- Sprite Renderer
- FloatingHealthBar (as a child object)

## Step 3: Configure UnitManager in the Inspector

1. Find the UnitManager GameObject in your scene
2. In the Inspector, configure the "Enemy Spawn Weights" array:
   - Set Size to 5 (or the number of enemy types you have)
   - For each element, set the "Enemy Name" to match the prefab name
   - Configure the spawn weight curves as desired

3. Additional settings:
   - Spawn Drones In Groups: Enable this to spawn drones in groups
   - Drone Group Size: Set to 3 (or your preferred group size)
   - First Titan Round: Set to 5 (or when you want Titans to start appearing)

## Step 4: Configure RoundManager in the Inspector

1. Find the RoundManager GameObject in your scene
2. Configure the scaling settings:
   - Health Scaling Per Round: 0.15 (15% increase per round)
   - Damage Scaling Per Round: 0.10 (10% increase per round)
   - Health Scaling Start Round: 3
   - Damage Scaling Start Round: 4

3. Configure difficulty tiers if desired:
   - Difficulty Tier Rounds: [1, 5, 10, 15, 20]
   - Difficulty Tier Names: ["Novice", "Challenging", "Difficult", "Extreme", "Nightmare"]

## Step 5: Configure GameManager in the Inspector

1. Find the GameManager GameObject in your scene
2. Configure the round settings:
   - Base Enemy Count: 5
   - Early Round Enemy Increment: 2
   - Mid Round Enemy Increment: 3
   - Late Round Enemy Increment: 4

## Step 6: Add the RoundInfoUI to Your Canvas

1. Add the RoundInfoUI script to a UI GameObject in your Canvas
2. Assign the appropriate TextMeshProUGUI components for:
   - Round Text
   - Difficulty Text
   - Enemy Scaling Text
   - Next Wave Info Text

## Step 7: Testing

1. Start the game and observe the enemy spawning patterns
2. Check the console for detailed logs about enemy spawning and round progression
3. Use the Tab key to toggle the enemy info panel
4. Use P to pause/unpause and 1/2/3 keys to adjust game speed

## Troubleshooting

If enemies don't spawn correctly:
1. Check the console for error messages
2. Verify that all ScriptableObjects are properly configured
3. Ensure that the enemy prefabs have the correct scripts attached
4. Check that the enemy names in UnitManager match the prefab names exactly

If enemy scaling doesn't work:
1. Verify that RoundManager is properly initialized
2. Check the round number is incrementing correctly
3. Ensure that the scaling values are set appropriately

## Additional Customization

- Adjust enemy stats in their respective scripts
- Modify spawn weights to change when different enemies appear
- Adjust scaling values to make the game easier or harder
- Create new enemy types by following the same pattern 