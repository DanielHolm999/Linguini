using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playercontroller : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;
    public LayerMask solidObjectsLayer;
    public LayerMask InteractableLayer;

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
                targetPos.x += input.x;
                targetPos.y += input.y;

                if (isWalkable(targetPos))
                {
                    Debug.Log("isWalkAble");
                    StartCoroutine(Move(targetPos));
                }
                
            }
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pressing space");
            Interact();
        }
    }

    public void Interact()
    {
        
        var faceingDir = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        var interactPos = transform.position + faceingDir;
        Debug.DrawLine(transform.position, faceingDir, Color.red, 10f);
        Debug.Log("inputs " + (Input.GetAxisRaw("Horizontal") + Input.GetAxisRaw("Vertical") + faceingDir.ToString()));

        //LineRenderer l = gameObject.AddComponent<LineRenderer>();
        //List<Vector3> pos = new List<Vector3>();
        //pos.Add(new Vector3(0, 0));
        //pos.Add(new Vector3(10, 10));
        //l.startWidth = 1f;
        //l.endWidth = 1f;
        //l.SetPositions(pos.ToArray());
        //l.useWorldSpace = true;

        var collider = Physics2D.OverlapCircle(interactPos, 0.3f, InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<EnemyController>().Interact();
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

    private bool isWalkable(Vector3 targetpos)
    {
        if (Physics2D.OverlapCircle(targetpos, 0.3f, solidObjectsLayer | InteractableLayer) != null)
        {
            return false;
        };
        return true;
    }
}
