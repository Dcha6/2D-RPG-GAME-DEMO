using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RandomEnemyEncounter : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyEncounterPrefab;
    private bool spawning = false;
    private int RN;
    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
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
    // Update is called once per frame
    void Update()
    {
        RN = Random.Range(1, 100);
        if (Input.GetAxisRaw("Horizontal")!=0 && Input.GetAxisRaw("Vertical")!=0)
        {
            if (RN > 5)
            {
                if(RN < 7)
                {
                    this.spawning = true;
                    SceneManager.LoadScene("Battle");
                }
            }
        }
    }
}
