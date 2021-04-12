using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    PlayerController playerCtrl;
    Rigidbody2D rigidstone;

    private void Start()
    {
        rigidstone = GetComponent<Rigidbody2D>();
        playerCtrl = GameObject.Find("Player").GetComponent<PlayerController>();
    }
    // Update is called once per frame
    void Update()
    {
        if (!playerCtrl.stonePull && rigidstone.velocity.y == 0 || !playerCtrl.grounded && playerCtrl.stonePull)
        {
            rigidstone.bodyType = RigidbodyType2D.Static;
        }
        else
        {
            rigidstone.bodyType = RigidbodyType2D.Dynamic;
        }
    }
}
