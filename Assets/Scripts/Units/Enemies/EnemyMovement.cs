using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float moveSpeed = 1f;
    
    private List<Vector3> pathVectorList;
    private int currentPathIndex;
    private Vector3 targetPosition;
    private float currentSlowAmount = 0f;
    private float slowDuration = 0f;
    private float slowTimer = 0f;
    
    private BaseEnemy baseEnemy;
    
    private void Awake()
    {
        baseEnemy = GetComponent<BaseEnemy>();
    }
    
    private void Start()
    {
        //wait for pathfinding to be ready
        StartCoroutine(WaitForPathfinding());
    }
    
    private IEnumerator WaitForPathfinding()
    {
        //wait until Pathfinding.Instance is not null
        while (Pathfinding.Instance == null)
        {
            Debug.Log("Waiting for Pathfinding to initialize...");
            yield return new WaitForSeconds(0.1f);
        }
        
        //now it's safe to find a path
        SetTargetToCenter();
    }
    
    private void Update()
    {
        HandleMovement();
        UpdateSlowEffect();
    }
    
    private void UpdateSlowEffect()
    {
        if (slowTimer > 0)
        {
            slowTimer -= Time.deltaTime;
            if (slowTimer <= 0)
            {
                currentSlowAmount = 0f;
            }
        }
    }
    
    public void SetTargetToCenter()
    {
        if (Pathfinding.Instance == null)
        {
            Debug.LogError("Pathfinding.Instance is null! Make sure Pathfinding is initialized before enemies spawn.");
            return;
        }
        
        pathVectorList = Pathfinding.Instance.FindPathToCenter(transform.position);
        if (pathVectorList != null && pathVectorList.Count > 0)
        {
            currentPathIndex = 0;
            targetPosition = pathVectorList[currentPathIndex];
        }
        else
        {
            Debug.LogWarning("No path found to center for enemy at " + transform.position);
        }
    }
    
    private void HandleMovement()
    {
        float effectiveSpeed = moveSpeed * (1f - currentSlowAmount);
        
        if (pathVectorList == null || currentPathIndex >= pathVectorList.Count)
        {
            //path complete or no path found
            return;
        }
        
        Vector3 moveDir = (targetPosition - transform.position).normalized;
        transform.position += moveDir * effectiveSpeed * Time.deltaTime;
        
        float reachedTargetDistance = 0.1f;
        if (Vector3.Distance(transform.position, targetPosition) < reachedTargetDistance)
        {
            //reached current target position
            currentPathIndex++;
            if (currentPathIndex < pathVectorList.Count)
            {
                //still has more positions to go
                targetPosition = pathVectorList[currentPathIndex];
            }
            else
            {
                //reached final position
                pathVectorList = null;
            }
        }
    }
    
    //call this when blocks are placed to recalculate path
    public void RecalculatePath()
    {
        //store the current position before recalculating
        Vector3 currentPos = transform.position;
        
        //get the new path
        List<Vector3> newPath = Pathfinding.Instance.FindPathToCenter(transform.position);
        
        if (newPath != null && newPath.Count > 0)
        {
            //find the closest point on the new path to continue from
            int closestPointIndex = 0;
            float closestDistance = float.MaxValue;
            Vector3 targetDirection = Vector3.zero;
            
            // If we have a current path and target, calculate our current direction
            if (pathVectorList != null && pathVectorList.Count > 0 && currentPathIndex < pathVectorList.Count)
            {
                targetDirection = (targetPosition - transform.position).normalized;
            }
            
            for (int i = 0; i < newPath.Count; i++)
            {
                float distance = Vector3.Distance(currentPos, newPath[i]);
                
                // Check if this point is in front of us (in our current direction of travel)
                bool isInFrontOfUs = true;
                if (targetDirection != Vector3.zero)
                {
                    Vector3 pointDirection = (newPath[i] - transform.position).normalized;
                    float dotProduct = Vector3.Dot(targetDirection, pointDirection);
                    isInFrontOfUs = dotProduct > 0; // Only consider points in front of us
                }
                
                // Only update if this point is closer AND in front of us (or if we haven't found any valid points yet)
                if (distance < closestDistance && (isInFrontOfUs || closestDistance == float.MaxValue))
                {
                    closestDistance = distance;
                    closestPointIndex = i;
                }
            }
            
            //set the path and continue from the closest point
            pathVectorList = newPath;
            currentPathIndex = closestPointIndex;
            targetPosition = pathVectorList[currentPathIndex];
        }
        else
        {
            Debug.LogWarning("No new path found to center for enemy at " + transform.position);
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        // Take the stronger slow effect
        if (slowAmount > currentSlowAmount)
        {
            currentSlowAmount = Mathf.Clamp01(slowAmount);
            slowDuration = duration;
            slowTimer = duration;
        }
        // If same strength, just refresh duration
        else if (slowAmount == currentSlowAmount)
        {
            slowTimer = duration;
        }
    }
} 