using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        Debug.Log("interacting test");
        Speak();
    }

    private void Speak()
    {
        Debug.Log("speaking");
        
    }
}
