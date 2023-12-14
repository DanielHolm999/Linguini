using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysics : MonoBehaviour
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
    }

    // Call this method to make the player jump
    public void Jump()
    {
        // Simple jump logic
        if (rb != null && Mathf.Approximately(rb.velocity.y, 0f))  // Check if player is on the ground
        {
            rb.velocity = Vector2.up * jumpForce;
        }
    }
}
