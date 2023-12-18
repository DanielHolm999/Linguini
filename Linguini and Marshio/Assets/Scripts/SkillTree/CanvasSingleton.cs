using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingleton : MonoBehaviour
{
    public static CanvasSingleton Instance;

    public void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Call this method to hide the Canvas
    public void HideCanvas()
    {
        gameObject.SetActive(false);
    }

    // Call this method to show the Canvas
    public void ShowCanvas()
    {
        gameObject.SetActive(true);
    }
}
