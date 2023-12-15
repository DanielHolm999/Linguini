using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SkillShop : MonoBehaviour, Interactable
{
    public string sceneName;
    public void Interact()
    {
        Debug.Log("skillshop test");
        ChangeScene();
    }

    public void ChangeScene()
    {
        if (CanvasSingleton.Instance != null)
        {
            CanvasSingleton.Instance.ShowCanvas();
        }
        SceneManager.LoadScene(sceneName);
    }
}
