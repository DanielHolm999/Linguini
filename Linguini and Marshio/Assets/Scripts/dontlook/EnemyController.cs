using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int expAmount = 100;
    // Start is called before the first frame update
    //void Start()
    //{
        
    //}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Interact()
    {
        Debug.Log("interacting test");
        Die();
    }

    private void Die()
    {
        Debug.Log("add expierence");
        ExperienceManager.Instance.AddExperience(expAmount);
    }
}
