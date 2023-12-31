﻿using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class TutorialPlayerPhysics : MonoBehaviour
{
    public float jumpForce = 5f;  // Adjust the force for jumping
    private Rigidbody2D rb;
    private TutorialBattleSystem battlesystem;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        battlesystem = FindObjectOfType<TutorialBattleSystem>();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && battlesystem.state == BattleState.ENEMYTURN)
        {
            Jump();
        }
        else if (Input.GetKeyDown(KeyCode.Space) && battlesystem.state != BattleState.ENEMYTURN)
        {
            Debug.Log("USER CANT JUMP AND STATE IS: " + battlesystem.state.ToString());
        }


    }

    public void Jump()
    {
        // Simple jump logic
        if (rb != null && Mathf.Approximately(rb.velocity.y, 0f))  // Check if player is on the ground
        {
            rb.velocity = Vector2.up * jumpForce;
        }
        else
        {
            Debug.Log("something went wrong??");
            if (rb == null)
            {
                Debug.Log("rb is null");
            }
            else if (!Mathf.Approximately(rb.velocity.y, 0f))
            {
                Debug.Log("player isnt on ground, velocity is " + rb.velocity.y.ToString());
            }
        }
    }

}

