using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RunAway : MonoBehaviour
{
    [SerializeField]
    private float runningChance;

    public void tryRunning()
    {
        //float randomNumber = Random.value;
        //if (randomNumber < this.runningChance)
        {
            Debug.Log("Runaway");
            GameManager.instance.LoadSceneAfterBattle();
            GameManager.instance.gameState = GameManager.GameStates.WORLDSTATE;
            GameManager.instance.enemyToBattle.Clear();
        }
      // else
      // {
      //    Debug.Log("Fail to runaway");
      //    FindObjectOfType<PlayerState>().currentState = PlayerState.battleStates.PROCESSING;
            
      //  }
     
    }
    
}