# Enemy System Implementation Guide

## Step 1: Set Up ScriptableObjects for Enemy Types

1. Create a "Resources/Units" folder in your Assets directory
2. In Unity, right-click in the Project window > Create > ScriptableObject > ScriptableUnit
3. Create one for each enemy type:
   - BlueStar (basic enemy)
   - SpeedEnemy
   - TankEnemy
   - SoldierEnemy
   - DroneEnemy
   - TitanEnemy

4. For each ScriptableObject:
   - Set Faction to "Enemy"
   - Drag the appropriate prefab to the "Unit Prefab" field
   - Save in the "Resources/Units" folder                              //does it matter where in this folder?

## Step 2: Set Up Enemy Prefabs

For each enemy type, create a prefab with these required components:
- The appropriate enemy script (SpeedEnemy, TankEnemy, etc.)
- Collider2D component
- Sprite Renderer
- EnemyMovement component
- BaseEnemy component                                                      needed?
- FloatingHealthBar (as a child object)

Required settings:
- Set appropriate move speed in EnemyMovement
- Configure health and damage values in BaseEnemy
- Set up appropriate collider size and type

## Step 3: Configure UnitManager

1. Find the UnitManager GameObject in your scene
2. Configure the "Enemy Spawn Weights" array:
   - Each entry needs:
     - Enemy Name (must match prefab name exactly)
     - Spawn Weight Curve (AnimationCurve determining spawn chance by round)
   - Configure curves in the inspector:
     - Early rounds: Higher weights for basic enemies
     - Later rounds: Increase weights for stronger enemies

3. Additional spawn settings:
   ```csharp
   spawnDronesInGroups = true;
   droneGroupSize = 3;
   firstTitanRound = 5;
   ```

## Step 4: Configure RoundManager

1. Find the RoundManager GameObject in your scene
2. Configure scaling settings:
   ```csharp
   healthScalingPerRound = 0.15f;  // 15% increase per round
   damageScalingPerRound = 0.10f;  // 10% increase per round
   healthScalingStartRound = 3;
   damageScalingStartRound = 4;
   ```

3. Configure difficulty tiers:
   ```csharp
   difficultyTierRounds = { 1, 5, 10, 15, 20 };
   difficultyTierNames = { "Novice", "Challenging", "Difficult", "Extreme", "Nightmare" };
   ```

## Step 5: Set Up Wave System

1. In GameManager:
   - Configure base enemy counts
   - Set wave timing parameters
   - Configure round increment settings

2. Wave Control Methods:
   ```csharp
   // Start a new wave
   GameManager.Instance.StartNewWave();        //what is this?
   
   // Check if wave is complete
   UnitManager.Instance.isWaveInProgress
   ```

## Step 6: UI Setup

1. Add RoundInfoUI script to a UI GameObject in your Canvas
2. Configure required references:
   ```csharp
   difficultyText: TextMeshProUGUI
   enemyScalingText: TextMeshProUGUI
   nextWaveInfoText: TextMeshProUGUI
   enemyInfoPanel: GameObject
   ```

3. Enemy Icons Setup:
   - Create icons for each enemy type as children of the enemyInfoPanel
   - The panel will be toggled with the Tab key
   - Organize icons in a layout group for proper spacing

4. Configure UI settings:
   ```csharp
   toggleInfoKey = KeyCode.Tab
   updateInterval = 1.0f
   ```

## Step 7: Testing

1. Runtime Testing:
   - Use Play mode to test enemy spawning
   - Watch the console for detailed spawn logs
   - Check enemy scaling with round progression

2. UI Controls:
   - Tab: Toggle enemy info panel
   - Monitor round progression and difficulty scaling
   - Check enemy health bars and effects

3. Debug Features:
   - Use UnitManager's test methods:
     ```csharp
     UnitManager.Instance.SpawnEnemiesTest();
     ```
   - Monitor enemy counts and wave status
   - Check pathfinding with EnemyMovement

## Step 8: Enemy Effects System

1. Slow Effect:
   ```csharp
   // Apply slow effect to enemy
   EnemyMovement movement = enemy.GetComponent<EnemyMovement>();
   if (movement != null)
   {
       movement.ApplySlow(slowAmount, duration);
   }
   ```

2. Visual Effects:
   - Each enemy prefab should have effect GameObjects:    //??
     - frostEffect
     - poisonEffect
     - splashEffect
   - Effects are toggled based on status

## Troubleshooting

Common Issues:
1. Enemies not spawning:
   - Verify ScriptableUnits are in Resources/Units
   - Check enemy names match exactly in UnitManager
   - Ensure spawn weights are properly configured

2. Pathfinding issues:
   - Check Pathfinding.Instance initialization
   - Verify EnemyMovement component settings
   - Debug path recalculation triggers

3. Scaling problems:
   - Verify RoundManager initialization
   - Check round number progression
   - Monitor scaling multipliers in logs

## Performance Optimization

1. Object Pooling:
   - Implement for frequently spawned enemies
   - Especially important for drone groups

2. Path Optimization:
   - Use path caching when possible
   - Optimize recalculation triggers

3. Effect Management:
   - Disable unused effect GameObjects
   - Pool particle effects

## Future Enhancements

1. Trap System (Planned):
   - Integration with grid system
   - Effect stacking with enemy debuffs
   - Trap upgrade system

2. Additional Features:
   - Enemy behavior variations
   - Advanced pathfinding options
   - Dynamic difficulty adjustment