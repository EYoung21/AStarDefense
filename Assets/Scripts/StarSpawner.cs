using UnityEngine;
using System.Collections;

public class StarSpawner : MonoBehaviour
{
    public GameObject starPrefab; // Assign the star prefab in Inspector
    public float spawnRate = 0.5f; // Time between spawns
    public float spawnRangeY = 2f; // Adjust how spread out stars spawn

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

            //instantiate a star slightly off-screen
            Instantiate(starPrefab, new Vector2(spawnPosition.x, randomY), Quaternion.identity);

            yield return new WaitForSeconds(spawnRate);
        }
    }
}


