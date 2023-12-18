using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class OpeningCrawlSceneChanger : MonoBehaviour
{
    public string sceneToLoad;
    public Animator animator;

    void Start()
    {
        if (animator != null)
        {
            StartCoroutine(WaitForAnimation());
        }
    }

    IEnumerator WaitForAnimation()
    {
        yield return new WaitForSeconds(1f); // Wait for animation to potentially start

        float animationLength = 60f; // Replace with the actual length of your animation in seconds
        yield return new WaitForSeconds(animationLength);
        // Load the new scene
        SceneManager.LoadScene(sceneToLoad);
    }
}


