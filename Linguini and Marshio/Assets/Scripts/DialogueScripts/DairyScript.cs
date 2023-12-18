using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SpriteManager : MonoBehaviour
{
    public PortalManager portalManager; // Assign in the Inspector
    private SpriteRenderer spriteRenderer;
    public Transform targetPosition;
    public GameObject spriteToHide;     // Lugini front facing
    public GameObject spriteToShow; // Lugini back facing
    public KidnappingScript dialogueScript; //dialogue for during portal kidnapping

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (portalManager != null)
        {
            portalManager.OnFullyOpaque += HandlePortalFullyOpaque;
        }
    }

    private void HandlePortalFullyOpaque()
    {
        StartCoroutine(RotateAndFadeOut());
    }

    private IEnumerator RotateAndFadeOut()
    {
        float fadeOutTime = 2f; // Duration of fade out and movement
        float currentFadeTime = 0f;
        Vector3 startPosition = transform.position; // Starting position of the sprite
        dialogueScript.StartDialogue();
        while (currentFadeTime < fadeOutTime)
        {
            // Rotation
            float rotationSpeed = 360f; // Degrees per second
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            // Fade out
            float alpha = Mathf.Lerp(1f, 0f, currentFadeTime / fadeOutTime);
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, alpha);

            // Gradual Movement
            transform.position = Vector3.Lerp(startPosition, targetPosition.position, currentFadeTime / fadeOutTime);

            currentFadeTime += Time.deltaTime;

            yield return null;
        }

        // Ensure it's fully transparent and at the target position at the end
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0f);
        transform.position = targetPosition.position;

        if (spriteToHide != null)
            spriteToHide.SetActive(false);
        if (spriteToShow != null)
        {
            spriteToShow.SetActive(true);
        }
    }

    public void MoveSpriteToShow()
    {
        if (spriteToShow != null)
        {
            StartCoroutine(MoveSpriteCoroutine());
        }
    }

    private IEnumerator MoveSpriteCoroutine()
    {
        float moveDuration = 2f; // Set the desired movement duration
        Vector3 startPosition = spriteToShow.transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < moveDuration)
        {
            spriteToShow.transform.position = Vector3.Lerp(startPosition, targetPosition.position, elapsedTime / moveDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        spriteToShow.transform.position = targetPosition.position;
        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("MainWorld");
    }

}
