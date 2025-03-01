using UnityEngine;

public class StarMovement : MonoBehaviour
{
    public float speed = 2f; //adjust speed as needed

    void Update()
    {
        //move diagonally down-left
        transform.position += new Vector3(-1, -1, 0) * speed * Time.deltaTime;
    }
}

