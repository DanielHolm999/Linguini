using System.Collections;
using UnityEngine;
using TMPro;

public class Dialogue : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameText;
    
    public DialogueLines[] lines;
    public float textSpeed;
    private int index;
    private bool isTyping;

    public GameObject sprite1; // Reference to the first sprite GameObject
    public GameObject sprite2; // 


    public AudioSource typingSound; // Reference to the AudioSource component
    //public AudioClip audioClip;
    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        StartDialogue();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTyping)
        {
            if (textComponent.text == lines[index].sentence)
            {
                NextLine();
            }
            else
            {
                StopAllCoroutines();
                textComponent.text = lines[index].sentence;
                isTyping = false;
            }
        }
    }

    void StartDialogue()
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        isTyping = true;
        string line = lines[index].sentence;
        nameText.text = lines[index].name; // Set the name text


        //if (nameText.text == "Lugini")
        //{
        //    nameText.rectTransform.anchoredPosition = new Vector2(1300, nameText.rectTransform.anchoredPosition.y);
        //    StartCoroutine(MoveSprite(sprite1, 1f, 0.1f));
        //}
        //else
        //{
        //    nameText.rectTransform.anchoredPosition = new Vector2(0, nameText.rectTransform.anchoredPosition.y);
        //    StartCoroutine(MoveSprite(sprite2, 1f, 0.1f));
        //}

        GameObject activeSprite = (nameText.text == "Lugini") ? sprite1 : sprite2;

        //typingSound = GetComponent<AudioSource>();
        //typingSound.clip = audioClip;
        typingSound.Play();


        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            if (char.IsWhiteSpace(c))
            {
                StartCoroutine(JumpSprite(activeSprite, 0.2f, 0.1f));
            }
            yield return new WaitForSeconds(textSpeed);
        }
        isTyping = false;
    }

    void NextLine()
    {
        if (index < lines.Length - 1)
        {
            index++;
            textComponent.text = string.Empty;
            StartCoroutine(TypeLine());
        }
        else
        {
            gameObject.SetActive(false);
        }
    }

    private IEnumerator MoveSprite(GameObject sprite, float duration, float height)
    {
        float time = 0;
        Vector3 startPosition = sprite.transform.position;
        Vector3 endPosition = startPosition + new Vector3(0, height, 0);

        while (time < duration)
        {
            sprite.transform.position = Vector3.Lerp(startPosition, endPosition, Mathf.Sin(time / duration * Mathf.PI));
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator JumpSprite(GameObject sprite, float duration, float height)
    {
        Vector3 originalPosition = sprite.transform.position;
        Vector3 peakPosition = originalPosition + new Vector3(0, height, 0);

        // Move up
        float halfDuration = duration / 2;
        float elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            sprite.transform.position = Vector3.Lerp(originalPosition, peakPosition, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Move down
        elapsedTime = 0;
        while (elapsedTime < halfDuration)
        {
            sprite.transform.position = Vector3.Lerp(peakPosition, originalPosition, elapsedTime / halfDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        sprite.transform.position = originalPosition; // Reset to original position
    }
}
