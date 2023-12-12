using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;
    public LayerMask solidObjectsLayer;

    // Update is called once per frame
    void Update()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                var targetPos = transform.position;
                targetPos.x += input.x * 0.2f;
                targetPos.y += input.y * 0.2f;

                if (isWalkAble(targetPos))
                {
                    StartCoroutine(Move(targetPos));
                }
                
            }
        }
    }

    IEnumerator Move(Vector3 targetPos)
    {

        isMoving = true;

        while ((targetPos - transform.position).sqrMagnitude > Mathf.Epsilon)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, moveSpeed * Time.deltaTime);
            yield return null;
        }
        transform.position = targetPos;

        isMoving = false;

    }

    private bool isWalkAble(Vector3 targetpos)
    {
        if (Physics2D.OverlapCircle(targetpos, 0.3f, solidObjectsLayer) != null)
        {
            return false;
        };
        return true;
    }
}
