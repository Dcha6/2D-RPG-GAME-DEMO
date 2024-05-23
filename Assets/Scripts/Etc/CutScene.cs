using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutScene : MonoBehaviour
{
    public bool playerInRange;
    public GameObject cutScene;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (playerInRange)
        {
            if (cutScene.activeInHierarchy)
            {
                cutScene.SetActive(false);
            }
            else
            {
                cutScene.SetActive(true);

            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //player enters range
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //player exits range
        {
            playerInRange = false;
            cutScene.SetActive(false);
            
        }
    }
}
