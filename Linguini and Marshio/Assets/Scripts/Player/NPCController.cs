using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour
{
    [SerializeField] NPCDialogueLines dialog;

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        Debug.Log("interacting test");
        Speak();
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
    }

    private void Speak()
    {
        Debug.Log("speaking");
        
    }
}
