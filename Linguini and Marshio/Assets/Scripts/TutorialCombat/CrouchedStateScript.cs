using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchedStateScript : MonoBehaviour
{
    public Unit playerUnit; // Reference to the Unit script

    public void TakeDamage(int damageAmount)
    {
        if (playerUnit != null)
        {
            playerUnit.TakeDamage(damageAmount);
            Debug.Log($"Crouched sprite taking {damageAmount} damage.");
        }
        else
        {
            Debug.LogError("Unit script not assigned!");
        }
    }
}
