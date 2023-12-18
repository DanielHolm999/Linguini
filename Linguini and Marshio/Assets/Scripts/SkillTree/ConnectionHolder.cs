using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectionHolder : MonoBehaviour
{
    public static ConnectionHolder Instance;

    public void Awake()
    {
        if(Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
