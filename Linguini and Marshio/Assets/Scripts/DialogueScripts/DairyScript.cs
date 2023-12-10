using System.Collections;
using UnityEngine;

public class SpriteManager : MonoBehaviour
{
    public PortalManager portalManager; // Assign in the Inspector
    private SpriteRenderer spriteRenderer;
    public Transform targetPosition;

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
    }

}
