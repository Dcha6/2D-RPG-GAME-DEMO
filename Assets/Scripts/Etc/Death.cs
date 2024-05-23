using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Death : MonoBehaviour
{
    public Transform respawnPoint;
    void OnCollisionEnter2D(Collision2D col)
    {
        if(col.collider.CompareTag("Player"))
        {
            col.collider.GetComponent<Rigidbody2D>().position = respawnPoint.position;
        }
    }
}

