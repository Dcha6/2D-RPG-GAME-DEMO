using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurn
{
    public string Attacker; //name of attacker
    public string Type;
    public GameObject AttacksGamesObject; //who attacks
    public GameObject AttackersTarget; //who is going to be attacked

    public BasicAttacks choosenAttack; //which attack is performed

}
