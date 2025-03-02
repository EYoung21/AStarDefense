using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float scaleSpeed = 2f;   // Speed of pulsating
    public float scaleAmount = 0.1f; // How much it expands and contracts
    public Transform healthBar;      // Assign this in the Inspector

    private Vector3 initialScale;
    private Vector3 healthBarInitialScale;

    void Start()
    {
        initialScale = transform.localScale; // Store the original scale
        if (healthBar != null)
            healthBarInitialScale = healthBar.localScale; // Store the health bar's original scale
    }

    void Update()
    {
        // Calculate scaling factor using a sine wave
        float scaleFactor = (1 - scaleAmount) + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;

        // Apply scale to the entire prefab
        transform.localScale = initialScale * scaleFactor;

        // Counteract the parent's scaling effect on the health bar
        if (healthBar != null)
            healthBar.localScale = healthBarInitialScale / scaleFactor;
    }
}
