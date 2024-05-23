using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemySpawn1 : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyEncounterPrefab;
    private bool spawning = false;
    public GameObject[] objects;
    public float spawnTime = 6f;
    private Vector3 spawnPosition;

    
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
        InvokeRepeating("Spawn", spawnTime, spawnTime);
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Battle")
        {
            if (this.spawning)
            {
                Instantiate(enemyEncounterPrefab);
            }
            SceneManager.sceneLoaded -= OnSceneLoaded;
            Destroy(this.gameObject);
        }
    }
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            this.spawning = true;
            SceneManager.LoadScene("Battle");
        }
    }
    public void SpawnRandom()
    {
        spawnPosition.x = Random.Range(-8, 8);
        spawnPosition.y = 0.5f;
        spawnPosition.z = Random.Range(-8, 8);
        Instantiate(objects[UnityEngine.Random.Range(0, objects.Length - 1)],spawnPosition, Quaternion.identity);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
