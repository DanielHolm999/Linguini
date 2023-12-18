using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NPCController : MonoBehaviour, Interactable
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
        if (gameObject.name == "CryoShell_0")
        {
            SceneManager.LoadScene("AquatToadBossBattle");
        }
        if (gameObject.name == "AquaToad")
        {
            SceneManager.LoadScene("WaterHomieBossBattle");
        }
    }

    private void Speak()
    {
        Debug.Log("speaking");
        
    }
}
