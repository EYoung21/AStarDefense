using UnityEngine;

public class TestEnemy1 : BaseEnemy
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();  // Call the base class Start method
        health = 5;
    }

    // // Update is called once per frame
    // void Update()
    // {
        
    // }
}
