using UnityEngine;
using System.Collections;

public class ImageFader : MonoBehaviour
{
    [SerializeField] private CanvasGroup[] canvasGroups;
    public float fadeSpeed = 1f;
    public float displayTime = 15f;

    private void Start()
    {
        StartCoroutine(CycleCanvasGroups());
    }

    private IEnumerator CycleCanvasGroups()
    {
        foreach (var canvasGroup in canvasGroups)
        {
            yield return StartCoroutine(FadeIn(canvasGroup));
            yield return new WaitForSeconds(displayTime);
            yield return StartCoroutine(FadeOut(canvasGroup));
        }

        // Optionally, do something after all canvas groups have been cycled through
    }

    private IEnumerator FadeIn(CanvasGroup group)
    {
        while (group.alpha < 0.5f)
        {
            group.alpha += fadeSpeed * Time.deltaTime;
            yield return null;
        }
        group.alpha = 0.5f; // Ensure fully visible
    }

    private IEnumerator FadeOut(CanvasGroup group)
    {
        while (group.alpha > 0)
        {
            group.alpha -= fadeSpeed * Time.deltaTime;
            yield return null;
        }
        group.alpha = 0; // Ensure fully invisible
    }
}


/*
using UnityEngine;
using System.Collections;

public class ImageFader : MonoBehaviour
{
    private bool _fadeIn = false;
    private bool _fadeOut = false;
    [SerializeField] private CanvasGroup canvasGroup;

    public float TimeToFade;


    private void Update()
    {
        if(_fadeIn)
        {
            if (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += TimeToFade * Time.deltaTime;
                if (canvasGroup.alpha >= 1)
                {
                    _fadeIn = false;
                }
            }
        }
        if (_fadeOut)
        {
            if (canvasGroup.alpha >= 0)
            {
                canvasGroup.alpha -= TimeToFade * Time.deltaTime;
                if (canvasGroup.alpha == 0)
                {
                    _fadeOut = false;
                }
            }
        }


    }
    public void FadeIn()
    {
        _fadeIn = true;
    }

    public void FadeOut()
    {
        _fadeOut = true;
    }
}
*/