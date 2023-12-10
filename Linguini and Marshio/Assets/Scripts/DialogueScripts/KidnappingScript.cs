using UnityEngine;
using System.Collections;
using TMPro;

public class KidnappingScript : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI nameText;

    public DialogueLines[] lines;
    public float textSpeed;
    private int index;

    // Start is called before the first frame update
    void Start()
    {
        textComponent.text = string.Empty;
        // Removed the immediate start of the dialogue
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && textComponent.text == lines[index].sentence)
        {
            NextLine();
        }
    }

    public void StartDialogue() // Now public so it can be called from outside
    {
        index = 0;
        StartCoroutine(TypeLine());
    }

    IEnumerator TypeLine()
    {
        string line = lines[index].sentence;
        nameText.text = lines[index].name; // Set the name text

        foreach (char c in line.ToCharArray())
        {
            textComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
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
            gameObject.SetActive(false); // Hides the dialogue box
        }
    }
}

