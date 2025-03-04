using UnityEngine;
using System.Collections;

public class StarSpawner : MonoBehaviour
{
    public GameObject[] starPrefabs; // Array of different star prefabs
    // public GameObject starPrefab; // Original single prefab (remove or comment this)
    public float spawnRate = 0.5f; //time between spawns
    public float spawnRangeY = 2f; //adjust how spread out stars spawn

    private Vector2 spawnPosition;

    void Start()
    {
        //convert screen size to world space for spawning at top-right
        Vector2 screenTopRight = Camera.main.ViewportToWorldPoint(new Vector2(1, 1));

        //set initial spawn position slightly off-screen
        spawnPosition = new Vector2(screenTopRight.x + 1, screenTopRight.y);

        //start spawning
        StartCoroutine(SpawnStars());
    }

    IEnumerator SpawnStars()
    {
        while (true) //keep spawning indefinitely
        {
            //randomize Y position for variety
            float randomY = Random.Range(spawnPosition.y - spawnRangeY, spawnPosition.y);
            
            //randomly select a prefab from the array
            GameObject selectedPrefab = starPrefabs[Random.Range(0, starPrefabs.Length)];
            
            //instantiate a star slightly off-screen
            Instantiate(selectedPrefab, new Vector2(spawnPosition.x, randomY), Quaternion.identity);

            yield return new WaitForSeconds(spawnRate);
        }
    }
}


