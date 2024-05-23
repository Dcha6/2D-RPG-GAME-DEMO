using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyState : MonoBehaviour
{
    public BaseEnemy enemy;
    
    private BattleStateMachine BSM;

   
    public enum battleStates
    {
        PROCESSING,
        CHOOSEACTION,
        WAITING,
        ACTION,
        DEAD
    }
    public battleStates currentState;
    //Progressbar
    private float currentCooldown = 0f;
    private float maxCooldown = 5f;
    //Position
    private Vector3 startposition;
    //Starting action phase
    private bool actionStarted = false;
    public GameObject PlayerToAttack;
    public float animSpeed;
    public float spaceBtwnUnits;
    private float missedAttackChance = 0f;
    

    private bool alive = true;
    void Start()
    {
        currentState = battleStates.PROCESSING;
        BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
        startposition = transform.position;
        missedAttackChance = Random.Range(0, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case (battleStates.PROCESSING):
                UpgradeProgressBar();
                break;
            case (battleStates.CHOOSEACTION):
                if (BSM.PlayersInBattle.Count > 0)
                {
                    ChooseAction();
                }
                currentState = battleStates.WAITING;
                
                break;
            case (battleStates.WAITING):
                break;
            case (battleStates.ACTION):
                
                StartCoroutine(TimeForAction());
                break;
            case (battleStates.DEAD):
                if (!alive)
                {
                    return;
                }
                else
                {
                    //tag
                    this.gameObject.tag = "DeadEnemy";
                    //remove from list
                    BSM.EnemysInBattle.Remove(this.gameObject);
                    //reset all inputs heroattacks
                    if (BSM.PlayersInBattle.Count > 0)
                    {
                        for (int i = 0; 1 < BSM.PerformList.Count; i++)
                        {
                            if (BSM.PerformList[i].AttacksGamesObject == this.gameObject)
                            {
                                BSM.PerformList.Remove(BSM.PerformList[i]);
                            }
                            if (BSM.PerformList[i].AttackersTarget == this.gameObject)
                            {
                                BSM.PerformList[i].AttackersTarget = BSM.EnemysInBattle[Random.Range(0, BSM.PlayersInBattle.Count)];
                            }
                        }
                    }
                        
                    this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105, 105, 105, 255);
                    alive = false;
                    BSM.EnemyButtons();
                    //check alive
                    BSM.performStates = BattleStateMachine.PerformAction.CHECKALIVE;
                }
                break;
        }
    }

    void UpgradeProgressBar()
    {
        currentCooldown = currentCooldown + Time.deltaTime;
        
        if (currentCooldown >= maxCooldown)
        {
            currentState = battleStates.CHOOSEACTION;
        }
    }
    void ChooseAction()
    {
        HandleTurn myAttack = new HandleTurn();
        myAttack.Attacker = enemy.theName;
        myAttack.Type = "Enemy";
        myAttack.AttacksGamesObject = this.gameObject;
        myAttack.AttackersTarget = BSM.PlayersInBattle[Random.Range (0, BSM.PlayersInBattle.Count)];

        int num = Random.Range(0, enemy.attacks.Count);
        myAttack.choosenAttack = enemy.attacks[num];

        Debug.Log(this.gameObject.name + " has chosen " + myAttack.choosenAttack.attackName + " and does " + myAttack.choosenAttack.attackDamage + " damage!");

        BSM.CollectActions(myAttack);

    }
    private IEnumerator TimeForAction()
    {
        if (actionStarted)
        {
            yield break;
        }
        actionStarted = true;

        //animate attack?
        Vector3 playerPosition = new Vector3(PlayerToAttack.transform.position.x+spaceBtwnUnits, PlayerToAttack.transform.position.y, PlayerToAttack.transform.position.z);
        while (MoveTowardsEnemy(playerPosition)){yield return null;}

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
        BSM.performStates = BattleStateMachine.PerformAction.PROCESSACTION;

        //end coroutine
        actionStarted = false;

        //reset this enemy state
        currentCooldown = 0f;
        currentState = battleStates.PROCESSING;
    }

    private bool MoveTowardsEnemy (Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }
    private bool MoveTowardsStart(Vector3 target)
    {
        return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
    }

    public void TakeDamage(float getDamageAmount)
    {
        enemy.currentHP -= getDamageAmount;
        if (enemy.currentHP <= 0)
        {
            enemy.currentHP = 0;
            currentState = battleStates.DEAD;
        }
        
    }
    void DoDamage()
    {
        float calculateDamage = enemy.currentATK + BSM.PerformList[0].choosenAttack.attackDamage;
        PlayerToAttack.GetComponent<PlayerState>().TakeDamage(calculateDamage);
    }


}
