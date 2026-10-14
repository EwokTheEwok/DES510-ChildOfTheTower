using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private GameObject DialogueBox;
    [SerializeField] private TMP_Text DialogueText;

    [SerializeField] private string[] DialogueLines;
    [SerializeField] private float TypeSpeed = 2.0f;

    private int lineIndex = 0;

    private bool isInteracting = false;
    private bool canInteract = false;

    public float timer;
    public float delay = 2.0f;
    public bool running;

    private void Update()
    {

        if (canInteract && !isInteracting)
        {
            StartDialogue();
        }

        if (isInteracting)
        {
            running = true;

            if (running)
            {
                timer += Time.deltaTime;
                if (timer > delay)
                {
                    NextLine();
                    running = false;
                    timer = 0.0f;
                }
            }
        }
    }   

    private void StartDialogue()
    {
        canInteract = false;
        isInteracting = true;

        DialogueText.text = "";
        DialogueBox.SetActive(true);

        lineIndex = 0;
        StartCoroutine(WriteLine());
    }

    private IEnumerator WriteLine()
    {
        foreach (char c in DialogueLines[lineIndex])
        {
            DialogueText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }
    }

    private void NextLine()
    {
        if (lineIndex < DialogueLines.Length - 1) 
        {
            DialogueText.text = "";

            lineIndex++;
            StartCoroutine(WriteLine());
        }
        else
        {
            DialogueBox.SetActive (false);

            isInteracting = false;
            canInteract = true;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !isInteracting)
        {
            canInteract = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.CompareTag("Player"))
        {
            canInteract = false;
            isInteracting = false;
            DialogueBox.SetActive(false);
            running = false;
            timer = 0.0f;
        }
    }
}
