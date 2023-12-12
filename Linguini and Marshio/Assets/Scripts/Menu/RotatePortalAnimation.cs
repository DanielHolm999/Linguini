using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatePortalAnimation : MonoBehaviour
{
   public float rotationSpeed = 30f;
    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
    }
}
