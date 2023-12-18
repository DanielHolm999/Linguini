using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public string tutorialScreen = "TutorialBattle";
    public string storyStartupScreen = "StoryStartup";

    public void OnStartButton ()
    {
        StatsController.InitialSetup();
        SceneManager.LoadScene(storyStartupScreen);
    }
    public void OnTutorialButton()
    {
        StatsController.InitialSetup();
        SceneManager.LoadScene(tutorialScreen);
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }
  
}
