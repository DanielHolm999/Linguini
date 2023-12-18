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


    public AudioClip takeDamageSFX;
    public AudioClip dyingSFX;

    AudioSource audioSource;

    private void Start()
    {

        if (gameObject.name == "CRYOPlayerSprite(Clone)"
            || gameObject.name == "WotahPlayerSprite(Clone)"
            || gameObject.name == "PlayerSprite(Clone)")
        {
            //unitLevel = StatsController.Level;
            //damage = StatsController.AttackDamage;
            //maxHp = StatsController.MaxHealth;
            //currentHp = StatsController.Health;
        }
        audioSource = GetComponent<AudioSource>();
        Debug.Log("lugini hp is: " + currentHp);
    }

    public bool TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"{unitName} now has {currentHp} HP.");
        if (currentHp <= 0)
        {
            audioSource.PlayOneShot(dyingSFX);
            return true;
        }
        else
        {
            audioSource.PlayOneShot(takeDamageSFX);
            return false;
        }
    }
}
