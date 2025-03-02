using UnityEngine;

public class PulseEffect : MonoBehaviour
{
    public float scaleSpeed = 2f;   //speed of pulsating
    public float scaleAmount = 0.1f; //how much it expands and contracts
    public Transform healthBar;      //assign this in the Inspector

    private Vector3 initialScale;
    private Vector3 healthBarInitialScale;

    void Start()
    {
        initialScale = transform.localScale; //store the original scale
        if (healthBar != null)
            healthBarInitialScale = healthBar.localScale; //store the health bar's original scale
    }

    void Update()
    {
        //calculate scaling factor using a sine wave
        float scaleFactor = (1 - scaleAmount) + Mathf.Sin(Time.time * scaleSpeed) * scaleAmount;

        //apply scale to the entire prefab
        transform.localScale = initialScale * scaleFactor;

        //counteract the parent's scaling effect on the health bar
        if (healthBar != null)
            healthBar.localScale = healthBarInitialScale / scaleFactor;
    }
}
