using UnityEngine;

public class TurretUpgradeTest : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TurretUpgradeTest Start - Checking all turrets in the scene");
        
        //find all turrets in the scene
        BaseTurret[] turrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
        Debug.Log($"Found {turrets.Length} turrets in the scene");
        
        foreach (BaseTurret turret in turrets)
        {
            Debug.Log($"Checking turret: {turret.name}");
            
            //check if the turret has a TurretUpgrade component
            TurretUpgrade upgrade = turret.GetComponent<TurretUpgrade>();
            if (upgrade != null)
            {
                Debug.Log($"Turret {turret.name} has TurretUpgrade component directly attached");
            }
            else
            {
                Debug.LogWarning($"Turret {turret.name} does not have TurretUpgrade component directly attached");
                
                //check children
                upgrade = turret.GetComponentInChildren<TurretUpgrade>();
                if (upgrade != null)
                {
                    Debug.Log($"Turret {turret.name} has TurretUpgrade component in children");
                }
                else
                {
                    Debug.LogError($"Turret {turret.name} does not have TurretUpgrade component at all!");
                    
                    //add the component
                    upgrade = turret.gameObject.AddComponent<TurretUpgrade>();
                    Debug.Log($"Added TurretUpgrade component to turret {turret.name}");
                }
            }
        }
    }
    
    //add a key press test for easier debugging
    void Update()
    {
        //press 'T' to check turrets again
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Manual turret check triggered");
            
            //find all turrets in the scene
            BaseTurret[] turrets = FindObjectsByType<BaseTurret>(FindObjectsSortMode.None);
            Debug.Log($"Found {turrets.Length} turrets in the scene");
            
            foreach (BaseTurret turret in turrets)
            {
                Debug.Log($"Checking turret: {turret.name}");
                
                //check if the turret has a TurretUpgrade component
                TurretUpgrade upgrade = turret.GetComponent<TurretUpgrade>();
                if (upgrade != null)
                {
                    Debug.Log($"Turret {turret.name} has TurretUpgrade component directly attached");
                }
                else
                {
                    Debug.LogWarning($"Turret {turret.name} does not have TurretUpgrade component directly attached");
                    
                    //check children
                    upgrade = turret.GetComponentInChildren<TurretUpgrade>();
                    if (upgrade != null)
                    {
                        Debug.Log($"Turret {turret.name} has TurretUpgrade component in children");
                    }
                    else
                    {
                        Debug.LogError($"Turret {turret.name} does not have TurretUpgrade component at all!");
                    }
                }
            }
        }
    }
} 