using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    public Transform playerPrefab;
    public Transform spawnpoint;
    public void RespawnPlayer()
    {
        Instantiate(playerPrefab, spawnpoint.position, spawnpoint.rotation);
        Debug.Log("TODO: Add Spawn Particles");
    }

}
