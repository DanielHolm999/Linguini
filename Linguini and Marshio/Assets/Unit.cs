using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public string unitName;
    public int unitLevel;

    public int damage;
    public int maxHp;
    public int currentHp;

    public bool isDead()
    {
        if (currentHp <= 0 )
        {
            currentHp=0;
            return true;
        }
        else
            return false;
    }
    public bool takeDamage(int dmg)
    {
        Debug.Log("in takedamage current hp is: " + currentHp);
        currentHp -= dmg;
        Debug.Log("Current hp is now: " + currentHp);
        return isDead();

        
    }
}
