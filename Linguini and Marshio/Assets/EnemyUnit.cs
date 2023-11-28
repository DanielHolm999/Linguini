using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class EnemyUnit : Unit
{
    public virtual IEnumerator AttackPlayer(PlayerUnit player, TextMeshProUGUI dialogueText, GameObject dialoguePanel)
    {
        // Create 1 second block window
        Debug.Log(unitName + " is attacking in 1 second");
        yield return new WaitForSeconds(2f);
        Debug.Log(unitName + " attacked!");

        // check if player is blocking
        // half damage if they are blocking

        if (player.isBlocking)
        {
            player.takeDamage(damage/2);
            Debug.Log("Player took half damage!");
        }
        else 
            player.takeDamage(damage);

    }
}
