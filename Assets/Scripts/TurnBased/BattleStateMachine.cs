using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class BattleStateMachine : MonoBehaviour
{
    public enum PerformAction
    {
        PROCESSACTION,
        TAKEACTION,
        PERFORMACTION,
        CHECKALIVE,
        WIN,
        LOSE
    }
    public PerformAction performStates;

    public List<HandleTurn> PerformList = new List<HandleTurn>();
    public List<GameObject> PlayersInBattle = new List<GameObject>();
    public List<GameObject> EnemysInBattle = new List<GameObject>();


    public enum HeroGUI
    {
        ACTIVATE,
        WAITING,
        INPUT1, //selecting action
        INPUT2, //selecting enemy

        DONE
    }

    public HeroGUI HeroInput;

    public List<GameObject> HerosToManage = new List<GameObject>(); //list to manage heroes
    private HandleTurn HeroChoice; //heroes' actions
    public GameObject enemyButton;
    public Transform Spacer;
    public GameObject AttackPanel;
    public GameObject EnemySelectPanel;
    public GameObject TalkPanel;
    public Dialogue dialogue;

    private List<GameObject> enemyButtons = new List<GameObject>();
    void Start()
    {
        performStates = PerformAction.PROCESSACTION;
        EnemysInBattle.AddRange(GameObject.FindGameObjectsWithTag("EnemyUnit"));
        PlayersInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
        EnemyButtons();
        HeroInput = HeroGUI.ACTIVATE;

        //sets it invisible while bar is loading
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(false);
        TalkPanel.SetActive(false);
    }

    
    void Update()
    {
        switch (performStates)
        {
            case (PerformAction.PROCESSACTION):
                if(PerformList.Count > 0)
                {
                    performStates = PerformAction.TAKEACTION;
                    
                }
                break;
            case (PerformAction.TAKEACTION):
                GameObject performer = GameObject.Find(PerformList[0].Attacker); //action towards attacker?
                if(PerformList[0].Type == "Enemy")
                {
                    EnemyState ESM = performer.GetComponent<EnemyState>();
                        for(int i=0; i < PlayersInBattle.Count; i++)
                    {
                        if(PerformList[0].AttackersTarget == PlayersInBattle[0])
                        {
                            ESM.PlayerToAttack = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyState.battleStates.ACTION;
                            break;
                        }
                        else
                        {
                            PerformList[0].AttackersTarget = PlayersInBattle[Random.Range(0, PlayersInBattle.Count)];
                            ESM.PlayerToAttack = PerformList[0].AttackersTarget;
                            ESM.currentState = EnemyState.battleStates.ACTION;
                        }
                    }
                    
                }

                if (PerformList[0].Type == "Player")
                {
                    PlayerState PSM = performer.GetComponent<PlayerState>();
                    PSM.EnemyToAttack = PerformList[0].AttackersTarget;
                    PSM.currentState = PlayerState.battleStates.ACTION;
                    
                }
                performStates = PerformAction.PERFORMACTION;
                break;
            case (PerformAction.PERFORMACTION):
                //idle state
                break;
            case (PerformAction.CHECKALIVE):
                if(PlayersInBattle.Count < 1)
                {
                    performStates = PerformAction.LOSE;
                }
                else if (EnemysInBattle.Count < 1)
                {
                    performStates = PerformAction.WIN;
                }
                else
                {
                    HeroInput = HeroGUI.ACTIVATE;
                }
                break;
            case (PerformAction.LOSE):
                {
                    Debug.Log("You lose");
                }
                break;
            case (PerformAction.WIN):
                {
                    Debug.Log("You win");
                    for (int i = 0; i < PlayersInBattle.Count; i++)
                    {
                        PlayersInBattle[i].GetComponent<PlayerState>().currentState = PlayerState.battleStates.WAITING;
                    }
                    GameManager.instance.LoadSceneAfterBattle();
                    GameManager.instance.gameState = GameManager.GameStates.WORLDSTATE;
                    GameManager.instance.enemyToBattle.Clear();
                }
                break;

        }
        switch (HeroInput)
        {
            case (HeroGUI.ACTIVATE):
                if(HerosToManage.Count > 0)
                {
                    
                    HeroChoice = new HandleTurn();
                    HerosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
                    AttackPanel.SetActive(true);
                    HeroInput = HeroGUI.WAITING;
                }
                break;
            case (HeroGUI.WAITING):
                //idle
                break;

            case (HeroGUI.DONE):
                HeroInputDone();
                break;
        }
        
    }
    public void CollectActions(HandleTurn input)
    {
        PerformList.Add(input);

    }

    public void EnemyButtons()
    {
        foreach(GameObject enemyBtn in enemyButtons)
        {
            Destroy(enemyBtn);
        }
        enemyButtons.Clear();

        foreach(GameObject enemy in EnemysInBattle)
        {
            GameObject newButton = Instantiate(enemyButton) as GameObject;
            EnemySelectButton button = newButton.GetComponent<EnemySelectButton>();
            EnemyState currentEnemy = enemy.GetComponent<EnemyState>();
            Text buttonText = newButton.transform.Find("Text").gameObject.GetComponent<Text>();
            buttonText.text = currentEnemy.enemy.theName;
            button.EnemyPrefab = enemy;
            newButton.transform.SetParent(Spacer,false);
            enemyButtons.Add(newButton);
        }
    }
    public void Input1() //attack button
    {
        HeroChoice.Attacker = HerosToManage[0].name;
        HeroChoice.AttacksGamesObject = HerosToManage[0];
        HeroChoice.Type = "Player";
        HeroChoice.choosenAttack = HerosToManage[0].GetComponent<PlayerState>().player.attacks[0];
        AttackPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);

    }

    

    public void TalkInput1() //talk button
    {
        HeroChoice.Attacker = HerosToManage[0].name;
        HeroChoice.AttacksGamesObject = HerosToManage[0];
        HeroChoice.Type = "Player";

        AttackPanel.SetActive(false);
        TalkPanel.SetActive(true);

    }
    public void talk() //choosing talking button
    {
        HeroChoice.Attacker = HerosToManage[0].name;
        HeroChoice.AttacksGamesObject = HerosToManage[0];
        HeroChoice.Type = "Player";
        HeroChoice.choosenAttack = HerosToManage[0].GetComponent<PlayerState>().player.attacks[0];

        AttackPanel.SetActive(false);
        TalkPanel.SetActive(false);
        EnemySelectPanel.SetActive(true);

        GameObject performer = GameObject.Find(PerformList[0].Attacker);
        if (PerformList[0].Type == "Player")
        {
            PlayerState PSM = performer.GetComponent<PlayerState>();
            PSM.EnemyToAttack = PerformList[0].AttackersTarget;
            PSM.currentState = PlayerState.battleStates.TALKING;

        }

    }

    
    public void Input2(GameObject choosenEnemy) // enemy selection
    {
        HeroChoice.AttackersTarget = choosenEnemy;
        HeroInput = HeroGUI.DONE;
    }

    void HeroInputDone()
    {
        PerformList.Add(HeroChoice);
        EnemySelectPanel.SetActive(false);
        HerosToManage[0].transform.Find("Selector").gameObject.SetActive(true);
        HerosToManage.RemoveAt(0);
        HeroInput = HeroGUI.ACTIVATE;
    }
    
}
