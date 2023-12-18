using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyController : MonoBehaviour, Interactable
{
    public string sceneName;
    [SerializeField] NPCDialogueLines dialog;

    // Update is called once per frame
    void Update()
    {

    }

    public void Interact()
    {
        Debug.Log("interacting test");
        StartCoroutine(DialogManager.Instance.ShowDialog(dialog));
        //if(DialogManager.Instance == inactive){
        //StartBattle();
        //}
        Debug.Log("when is this called");
    }

    private void StartBattle()
    {
        Debug.Log("speaking");
        SceneManager.LoadScene(sceneName);
    }
}