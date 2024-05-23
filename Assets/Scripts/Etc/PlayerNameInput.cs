using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerNameInput : MonoBehaviour
{
    public InputField playername;
    public string playerName;
    public GameObject Canvas;
    Text userNameInputText;

    // Start is called before the first frame update
    void Start()
    {
        userNameInputText = Canvas.transform.Find("InputField/Text").GetComponent<Text>();
    }
    public void InputYourName()
    {
        playerName = userNameInputText.text;
        DontDestroyOnLoad(transform.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
