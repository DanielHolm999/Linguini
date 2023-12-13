using System;
using System.Collections;
using UnityEngine;

public class PortalManager : MonoBehaviour
{
    public SpriteRenderer portalSpriteRenderer; // Assign this in the Inspector
    public event Action OnFullyOpaque;

    private void TriggerFullyOpaqueEvent()
    {
        if (OnFullyOpaque != null)
            OnFullyOpaque();
    }

    // Call this method to start the fade-in process
    public void StartFadeIn(float duration)
    {
        if (portalSpriteRenderer != null)
        {
            StartCoroutine(FadeInSprite(duration));
        }
        else
        {
            Debug.LogError("Portal SpriteRenderer not assigned!");
        }
    }

    private IEnumerator FadeInSprite(float duration)
    {
        // Ensure the portal object is active
        portalSpriteRenderer.gameObject.SetActive(true);

        // Initialize alpha to 0 (fully transparent)
        portalSpriteRenderer.color = new Color(portalSpriteRenderer.color.r, portalSpriteRenderer.color.g, portalSpriteRenderer.color.b, 0f);

        float currentTime = 0f;
        while (currentTime < duration)
        {
            float alpha = Mathf.Lerp(0f, 1f, currentTime / duration);
            portalSpriteRenderer.color = new Color(portalSpriteRenderer.color.r, portalSpriteRenderer.color.g, portalSpriteRenderer.color.b, alpha);
            currentTime += Time.deltaTime;
            yield return null;
        }

        // Ensure it's fully opaque at the end
        portalSpriteRenderer.color = new Color(portalSpriteRenderer.color.r, portalSpriteRenderer.color.g, portalSpriteRenderer.color.b, 1f);
        TriggerFullyOpaqueEvent();
    }
}
