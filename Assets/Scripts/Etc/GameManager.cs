using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public RegionData currentRegion;
    

    public GameObject heroCharacter;

    public Vector3 nextHeroPosition;
    public Vector3 lastHeroPosition;

    public string sceneToLoad;
    public string lastScene;

    public bool isWalking = false;
    public bool canGetEncounter = false;
    public bool gotAttacked = false;


    public enum GameStates
    {
        WORLDSTATE,
        TOWNSTATE,
        BATTLESTATE,
        IDLE
    }
    

    public List<GameObject> enemyToBattle = new List<GameObject>();
    public GameStates gameState;

    //spawnpoint
    public string nextSpawnPoint;
    void Awake()
    {
      if(instance == null)
        {
            instance = this;
        }
      else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
        if (!GameObject.Find("Character"))
        {
            GameObject Player = Instantiate(heroCharacter, nextHeroPosition, Quaternion.identity) as GameObject;
            Player.name = "Character";
        }
    }

    void Update()
    {
        switch (gameState)
        {
            case (GameStates.WORLDSTATE):
                if (isWalking)
                {
                    RandomEncounter();
                }
                if (gotAttacked)
                {
                    gameState = GameStates.BATTLESTATE;
                }
                break;
            case (GameStates.TOWNSTATE):
                break;
            case (GameStates.BATTLESTATE):
                StartBattle();
                gameState = GameStates.IDLE;
                break;
            case (GameStates.IDLE):
                break;
        }
    }
    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadSceneAfterBattle()
    {
        SceneManager.LoadScene(lastScene);
    }
    void RandomEncounter()
    {
        if (isWalking && canGetEncounter)
        {
            if(Random.Range(0,1000)< 5)
            {
                gotAttacked = true;
                Debug.Log("I got attacked");
            }
        }
    }
    void StartBattle()
    {
        //amount of enemy
        int enemyAmount = Random.Range(1,currentRegion.maxAmountOfEnemy);
        //which enemy
        for(int i = 0; i < enemyAmount; i++)
        {
            enemyToBattle.Add(currentRegion.possibleEnemy[Random.Range(0, currentRegion.possibleEnemy.Count)]);
        }
        //hero last position
        lastHeroPosition = GameObject.Find("Character").gameObject.transform.position;
        nextHeroPosition = lastHeroPosition;
        lastScene = SceneManager.GetActiveScene().name;
        //load battle depending on area
        SceneManager.LoadScene(currentRegion.BattleScene);
        //reset hero
        isWalking = false;
        gotAttacked = false;
        canGetEncounter = false;
    }
}
