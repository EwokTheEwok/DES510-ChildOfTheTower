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

    private bool _InRange = false;
    private bool _IsActive = false;
    private bool _IsTyping = false;

    public float delay = 2.0f;
    public float height = 2.0f;

    private RectTransform dialogueTransform;
    private IEnumerator coroutine;
  
    private void Start()
    {
        dialogueTransform = DialogueBox.transform as RectTransform;
        
    }

    private void Update()
    {

        if (_InRange)
        {
            if (!_IsActive) {
                StartDialogue();
            }
            else
            {
                dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));
                if (!_IsTyping)
                {
                    Debug.Log("a");
                    WriteLine();
                }
            }
        }
    }   

    private void StartDialogue()
    {
        _IsActive = true;

        DialogueText.text = "";
        DialogueBox.SetActive(true);

        lineIndex = 0;
    }

    private IEnumerator WriteLetters()
    {
        _IsTyping = true;
        foreach (char c in DialogueLines[lineIndex])
        {
            DialogueText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }
        yield return new WaitForSeconds(delay);
        _IsTyping = false;
    }

    private void WriteLine()
    {
        if (lineIndex < DialogueLines.Length) 
        {
            DialogueText.text = "";

            coroutine = WriteLetters();
            StartCoroutine(coroutine);
            lineIndex++;
        }
        else
        {
            DialogueBox.SetActive (false);

            _IsActive = false;
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !_IsActive)
        {
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if(collision.CompareTag("Player"))
        {
            StopCoroutine(coroutine);
            _InRange = false;
            _IsActive = false;
            _IsTyping = false;
            DialogueBox.SetActive(false);
        }
    }
}
