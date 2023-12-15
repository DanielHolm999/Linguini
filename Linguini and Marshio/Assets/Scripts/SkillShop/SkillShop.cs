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
        SceneManager.LoadScene(sceneName);
    }

    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
