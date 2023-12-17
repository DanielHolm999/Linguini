using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class StatsController
{
    public static int Health { get; set; }
    public static int MaxHealth { get; set; }
    public static int SkillPoints { get; set; }
    public static int Experience { get; set; }
    public static int MaxExperience { get; set; }
    public static int AttackDamage { get; set; }
    public static int Armor { get; set; }
    public static int Money { get; set; }

    internal static void InitialSetup()
    {
        Health = 100;
        MaxHealth = 100;
        SkillPoints = 5;
        Experience = 0;
        MaxExperience = 100;
        AttackDamage = 5;
        Armor = 0;
        Money = 0;
    }

}
