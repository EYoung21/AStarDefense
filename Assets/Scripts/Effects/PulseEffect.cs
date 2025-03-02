using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float scaleSpeed = 2f;   // Speed of pulsating
    public float scaleAmount = 0.1f; // How much it expands and contracts

    private Vector3 initialScale;   

    void Start()
    {
        initialScale = transform.localScale; // Store the original scale
    }

    void Update()
    {
        // Calculate scaling factor using a sine wave
        float scaleFactor = (1 - scaleAmount) + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;

        // Apply scale to the entire prefab
        transform.localScale = initialScale * scaleFactor;
    }
}
