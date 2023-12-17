using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Playercontroller : MonoBehaviour
{
    public float moveSpeed;
    private bool isMoving;
    private Vector2 input;
    public LayerMask solidObjectsLayer;
    public LayerMask InteractableLayer;
    private Animator animator;

    private void Awake()
    {
        Debug.Log("statscontroller = " + StatsController.PlayerPositionMainWorld);
        if (SceneManager.GetActiveScene().name == "MainWorld" && StatsController.PlayerPositionMainWorld != new Vector3())
        {
            Vector3 newPosition = StatsController.PlayerPositionMainWorld;
            newPosition.y -= 0.3f;
            transform.position = newPosition;
        }
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    public void HandleUpdate()
    {
        if (!isMoving)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");

            if (input.x != 0) input.y = 0;

            if(input != Vector2.zero)
            {
                animator.SetFloat("MoveX", input.x);
                animator.SetFloat("MoveY", input.y);

                var targetPos = transform.position;
                targetPos.x += input.x * 0.2f;
                targetPos.y += input.y * 0.2f;

                if (isWalkable(targetPos))
                {
                    //Debug.Log("isWalkAble");
                    StartCoroutine(Move(targetPos));
                }
            }
            animator.SetBool("isMoving", isMoving);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("pressing space");
            Interact();
        }
    }

    public void Interact()
    {
        
        var faceingDir = new Vector3(animator.GetFloat("MoveX"), animator.GetFloat("MoveY"));
        var interactPos = transform.position + faceingDir;
        //Debug.DrawLine(transform.position, faceingDir, Color.red, 10f);
        Debug.Log("inputs " + faceingDir.ToString());

        var collider = Physics2D.OverlapCircle(interactPos, 0.5f, InteractableLayer);
        if(collider != null)
        {
            collider.GetComponent<Interactable>()?.Interact();
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
        if (Physics2D.OverlapCircle(targetpos, 0.1f, solidObjectsLayer | InteractableLayer) != null)
        {
            return false;
        };
        return true;
    }
}
