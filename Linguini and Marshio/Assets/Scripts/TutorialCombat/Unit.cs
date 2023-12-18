using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
   
    public string unitName;
    public int unitLevel = StatsController.Level;

    public int damage = StatsController.AttackDamage;

    public int maxHp = StatsController.MaxHealth;
    public int currentHp = StatsController.Health;


    public AudioClip takeDamageSFX;
    public AudioClip dyingSFX;

    AudioSource audioSource;

    private void Start()
    {
     unitLevel = StatsController.Level;

    damage = StatsController.AttackDamage;

    maxHp = StatsController.MaxHealth;
    currentHp = StatsController.Health;
    audioSource = GetComponent<AudioSource>();
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
