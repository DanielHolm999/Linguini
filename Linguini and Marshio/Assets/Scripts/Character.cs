using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    int currentHealth, maxhealth, currentExperience, maxExperience, currentlevel;
    private LevelWindow levelWindow;

    private void OnEnable()
    {
        ExperienceManager.Instance.onExperienceChange += HandleExperienceChange;
    }

    private void OnDisable()
    {
        ExperienceManager.Instance.onExperienceChange += HandleExperienceChange;
    }

    private void HandleExperienceChange(int newExperience)
    {
        Debug.Log("experience handler called");
        currentExperience += newExperience;
        if (currentExperience >= maxExperience)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        maxhealth += 10;
        currentHealth = maxhealth;
        currentlevel++;
        currentExperience = 0;
        maxExperience += 100;
        levelWindow.SetExperienceBarSize(currentExperience);
        levelWindow.SetLevelNumber(currentlevel);
    }

    private void SetLevelWindow()
    {
        levelWindow = GetComponent<LevelWindow>();
    }
}
