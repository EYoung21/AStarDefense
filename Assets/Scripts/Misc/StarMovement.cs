using UnityEngine;

public class StarMovement : MonoBehaviour
{
    public float speed = 2f; //adjust speed as needed

    void Update()
    {
        //move diagonally down-left
        transform.position += new Vector3(-1, -1, 0) * speed * Time.deltaTime;
        
        // Debug every few seconds
        if (Time.frameCount % 60 == 0)
        {
            Debug.Log($"Star position: {transform.position}");
        }
        
        // Destroy stars once they're well off-screen
        Vector2 screenBottomLeft = Camera.main.ViewportToWorldPoint(new Vector2(0, 0));
        if (transform.position.x < screenBottomLeft.x - 2f || transform.position.y < screenBottomLeft.y - 2f)
        {
            Destroy(gameObject);
        }
    }
}

