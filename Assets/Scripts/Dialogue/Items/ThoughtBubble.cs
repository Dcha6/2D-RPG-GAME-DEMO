using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ThoughtBubble: MonoBehaviour
{
    public GameObject dialogBox;
    public Text dialogText;
    public string dialog;
    public bool playerInRange;
    public Dialogue dialogue;
    public Signal contextOn;
    public Signal contextOff;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //player enters range
        {
            contextOn.Raise();
            playerInRange = true;
            if (dialogBox.activeInHierarchy)
            {
                dialogBox.SetActive(false);

            }
            else
            {
                dialogBox.SetActive(true);
                dialogText.text = dialog;

            }
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player")) //player exits range
        {
            contextOff.Raise();
            playerInRange = false;
            dialogBox.SetActive(false);
            Destroy(this.gameObject);
        }
    }
}
