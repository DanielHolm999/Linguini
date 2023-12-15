using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PortalController : MonoBehaviour
{
    public string sceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger entered");
        if(collision.CompareTag("Player"))
        {
            print("switching scene to " + sceneName);
            SceneManager.LoadScene(sceneName);
        }
    }
}
