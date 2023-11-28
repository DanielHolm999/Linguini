using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EnemyUnitTest : EnemyUnit
{
    public GameObject projectile;
        public override IEnumerator AttackPlayer(PlayerUnit player, TextMeshProUGUI dialogueText, GameObject dialoguePanel)
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = $"{unitName} is attacking!";


        // Create 1 second block window
        Debug.Log(unitName + " is attacking in 1 second");
        yield return new WaitForSeconds(1f);
        dialogueText.text = "Block!";
        yield return new WaitForSeconds(0.5f);
        Debug.Log(unitName + " attacked!");

        // check if player is blocking
        // half damage if they are blocking

        if (player.isBlocking)
        {
            player.takeDamage(damage/2); 
            dialogueText.text = $"{player.unitName} took {damage/2} damage";
        }
        else 
        {
            player.takeDamage(damage);
            dialogueText.text = $"{player.unitName} took {damage} damage";

        }
    }
}
