using UnityEngine;

public class RotateSprite : MonoBehaviour
{
    public float rotationSpeed = 100f; //adjust this value to control speed

    void Update()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Time.deltaTime);
    }
}
