using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerPhysics : MonoBehaviour
{
    public float jumpForce = 5f;  // Adjust the force for jumping
    private Rigidbody2D rb;
    private BattleSystem battlesystem;
    private bool isGrounded;
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        battlesystem = FindObjectOfType<BattleSystem>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && battlesystem.state == RealBattleState.ENEMYTURN)
        {
            Jump();
        }
        else if(Input.GetKeyDown(KeyCode.Space) && battlesystem.state != RealBattleState.ENEMYTURN)
        {
            Debug.Log("USER CANT JUMP AND STATE IS: " + battlesystem.state.ToString());
        }
    }

    // Call this method to make the player jump
    public void Jump()
    {
        // Simple jump logic
        if (rb != null && Mathf.Approximately(rb.velocity.y, 0f))  // Check if player is on the ground
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
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

    void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("COLLISION ENTER!!!!!!!");
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        Debug.Log("COLLISION EXIT!!!!");
    }
}
