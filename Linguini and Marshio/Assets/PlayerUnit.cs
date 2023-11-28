using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUnit : Unit
{
    public bool canBlock = false;
    public bool isBlocking {get; private set; } = false;

    private IEnumerator Block()
    {
        isBlocking = true;
        yield return new WaitForSeconds(2f);
        isBlocking = false;
    }
    private void Update()
    {
        if (!isBlocking && canBlock && Input.GetKeyDown(KeyCode.B))
        {
            Debug.Log("is blocking;");
            StartCoroutine(Block());
        }
    }
}
