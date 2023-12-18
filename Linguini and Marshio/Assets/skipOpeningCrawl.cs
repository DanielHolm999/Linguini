using UnityEngine;
using UnityEngine.SceneManagement;

public class skipOpeningCrawl : MonoBehaviour
{
    public string sceneToLoad = "MainWorld"; // Set this to the name of the scene you want to load
    private float keyHoldTime = 0f;
    private bool isKeyBeingHeld = false;

    void Update()
    {
       
        if (Input.GetKeyDown(KeyCode.S))
        {
            isKeyBeingHeld = true;
            keyHoldTime = 0f; // Reset the timer
        }

        // Update the timer if the key is being held
        if (isKeyBeingHeld && Input.GetKey(KeyCode.S))
        {
            keyHoldTime += Time.deltaTime;

            // Check if the key has been held for 3 seconds
            if (keyHoldTime >= 3f)
            {
                // Load the new scene
                SceneManager.LoadScene(sceneToLoad);
            }
        }

        // Reset if the key is released
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isKeyBeingHeld = false;
            keyHoldTime = 0f;
        }
    }
}
