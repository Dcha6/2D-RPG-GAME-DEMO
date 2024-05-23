using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerState : MonoBehaviour
{
    public BaseHero player;
    private BattleStateMachine BSM;
   


    public enum battleStates
    {
        PROCESSING,
        ADDTOLIST,
        WAITING,
        SELECTING,
        ACTION,
        TALKING,
        DEAD
    }

    public battleStates currentState;
    //for progress bar
    private float currentCooldown = 0f;
    private float maxCooldown = 5f;
    public Image ProgressBar;
    public GameObject Selector;

    //Ienumerator
    public GameObject EnemyToAttack;
    private bool actionStarted = false;
    private Vector3 startposition;
    public float animSpeed;
    public float spaceBtwnUnits;

    //dead
    private bool alive = true;

    //hero panel
    private HeroPanelStats stats;
    public GameObject HeroPanel;
    private Transform HeroPanelSpacer;

    //for talking
    public Text nameText;
    public Text dialogueText;
    public Animator animator;
    private Queue<string> sentences;
    public bool playerInRange;
    public Dialogue dialogue;
    void Start()
    {
        HeroPanelSpacer = GameObject.Find("PlayerCanvas").transform.Find("PlayerPanel").Find("HeroPanelSpacer");
        CreateHeroPanel();
        sentences = new Queue<string>();

        startposition = transform.position;
        currentCooldown = Random.Range(0, 2.5f);
        Selector.SetActive(false);
        currentState = battleStates.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (battleStates.PROCESSING):
                UpgradeProgressBar();
                break;
            case (battleStates.ADDTOLIST):
                BSM.HerosToManage.Add(this.gameObject);
                currentState = battleStates.WAITING;
                break;
            case (battleStates.WAITING):
                //idle state
                break;
            case (battleStates.ACTION):
                StartCoroutine(TimeForAction());
                break;
            case (battleStates.TALKING):
                FindObjectOfType<DialogueManager>().StartDialogue(dialogue);
                Debug.Log("Player starts talking");
                break;
            case (battleStates.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    //change tag
                    this.gameObject.tag = "DeadHero";

                    //not attackeable by enemy
                    BSM.PlayersInBattle.Remove(this.gameObject);

                    //not manageable
                    BSM.HerosToManage.Remove(this.gameObject);

                    //deactivate the selector
                    Selector.SetActive(false);

                    //reset gui
                    BSM.AttackPanel.SetActive(false);
                    BSM.EnemySelectPanel.SetActive(false);

                    //remove item from performlist
                    if (BSM.PlayersInBattle.Count > 0)
                    {
                        for (int i = 0; i < BSM.PerformList.Count; i++)
                        {
                            if (BSM.PerformList[i].AttacksGamesObject == this.gameObject)
                            {
                                BSM.PerformList.Remove(BSM.PerformList[i]);
                            }

                            if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                            {
                                BSM.PerformList[i].AttackersTarget = BSM.PlayersInBattle[Random.Range(0, BSM.PlayersInBattle.Count)];
                            }
                        }
                    }
                    

                    //change color / play animation
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);

                    //reset heropoint
                    BSM.HeroInput = BattleStateMachine.HeroGUI.ACTIVATE;
                    alive = false;
                    Debug.Log("Player is dead");
                }
                break;
        }
    }

    void UpgradeProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime;
        float calculateCooldown = currentCooldown / maxCooldown;
        ProgressBar.transform.localScale = new Vector3(Mathf.Clamp(calculateCooldown, 0, 1), ProgressBar.transform.localScale.y, ProgressBar.transform.localScale.z);
        if (currentCooldown >= maxCooldown)
        {
            currentState = battleStates.ADDTOLIST;
        }
    }
   
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        //animate attack?
        Vector3 enemyPosition = new Vector3(EnemyToAttack.transform.position.x - spaceBtwnUnits, EnemyToAttack.transform.position.y, EnemyToAttack.transform.position.z);
        while (MoveTowardsEnemy(enemyPosition)) { yield return null; }

        //wait
        yield return new WaitForSeconds(0.5f);

        //dmg
        DoDamage();

        //animate back to position
        Vector3 firstPosition = startposition;
        while (MoveTowardsStart(firstPosition)) { yield return null; }

        //remove this performer form the list in bsm
        BSM.PerformList.RemoveAt(0);

        //reset BSM -> wait
        if(BSM.performStates != BattleStateMachine.PerformAction.WIN && BSM.performStates != BattleStateMachine.PerformAction.LOSE)
        {
            BSM.performStates = BattleStateMachine.PerformAction.PROCESSACTION;

            //reset this enemy state
            currentCooldown = 0f;
            currentState = battleStates.PROCESSING;
        }
        else
        {
            currentState = battleStates.WAITING;
        }

        //end coroutine
        actionStarted = false;
    }
    

    private bool MoveTowardsEnemy(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        player.currentHP -= getDamageAmount;
        if(player.currentHP <= 0)
        {
            player.currentHP = 0;
            currentState = battleStates.DEAD;
        }
        UpdateHeroPanel();
    }

    void DoDamage()
    {
        float calculateDamage = player.currentATK + BSM.PerformList[0].choosenAttack.attackDamage;
        EnemyToAttack.GetComponent<EnemyState>().TakeDamage(calculateDamage);
    }
    void CreateHeroPanel()
    {
        HeroPanel = Instantiate(HeroPanel as GameObject);
        stats = HeroPanel.GetComponent<HeroPanelStats>();
        stats.PlayerName.text = player.theName;
        stats.PlayerHP.text = "HP: " + player.currentHP;
        stats.PlayerMP.text = "MP: " + player.currentMP;
        ProgressBar = stats.ProgressBar;
        HeroPanel.transform.SetParent(HeroPanelSpacer, false);
    }

    void UpdateHeroPanel()
    {
        stats.PlayerHP.text = "HP: " + player.currentHP;
        stats.PlayerMP.text = "MP: " + player.currentMP;
    }

}
