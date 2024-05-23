using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomEnemySpawner : MonoBehaviour
{
    public GameObject enemy;
    float randx;
    Vector2 whereToSpawn;
    public float spawnRate = 2f;
    float nextSpawn = 0.0f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > nextSpawn)
        {
            nextSpawn = Time.time + spawnRate;
            randx = Random.Range(-9.9f, 9.9f);
            whereToSpawn = new Vector3(randx, transform.position.y, transform.position.x);
            Instantiate(enemy, whereToSpawn, Quaternion.identity);
        }
    }
}
