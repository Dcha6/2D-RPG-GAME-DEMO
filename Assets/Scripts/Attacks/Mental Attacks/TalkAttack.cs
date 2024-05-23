using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TalkAttack : BasicAttacks
{
    public void Talking()
    {
        attackName = "Talk";
        attackDescription = "You talk to the enemy";
        attackDamage = 0;
        attackCost = 0;
    }
}
