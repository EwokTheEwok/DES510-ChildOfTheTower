using Unity.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private GameObject DialogueBox;
    [SerializeField] private GameObject NameBox;
    [SerializeField] private TMP_Text DialogueText;
    [SerializeField] private TMP_Text NameText;

    [SerializeField] private string[] DialogueLines;
    [SerializeField] private string NPCName;
    [SerializeField] private float TypeSpeed = 0.05f;

    private int lineIndex = 0;

    private bool _InRange = false;
    private bool _IsActive = false;
    private bool _IsTyping = false;

    public float _LineDelay = 2.0f;
    public float _EndDelay = 7.0f;
    public float height = 100000f;

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
            dialogueTransform.position = Camera.main.WorldToScreenPoint(transform.position + new Vector3(0, height, 0));
            if (!_IsActive)

            {
                coroutine = StartDia();
                StartCoroutine(coroutine);
            }

            /*
            if (!_IsTalking) {
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
            }*/
        }
    }

    private IEnumerator StartDia()
    {
        lineIndex = 0;
        _IsActive = true;
        DialogueBox.SetActive(true);
        NameBox.SetActive(true);
        NameText.text = NPCName;

        foreach (string line in DialogueLines)
        {
            DialogueText.text = "";
            foreach (char c in line)
            {
                DialogueText.text += c;
                yield return new WaitForSeconds(TypeSpeed);
            }
            yield return new WaitForSeconds(_LineDelay);
        }

        DialogueBox.SetActive(false);
        yield return new WaitForSeconds(Mathf.Abs(_EndDelay - _LineDelay));

        StopTalking();
    }

    private void StopTalking()
    {
        _IsActive = false;
        DialogueText.text = "";
        DialogueBox.SetActive(false);
        NameBox.SetActive(false);
        NameText.text = "";
    }
    /*
    private void StartDialogue()
    {
        _IsTalking = true;

        ControlText.text = "";
        ControlBox.SetActive(true);

        lineIndex = 0;
    }

    private IEnumerator WriteLetters()
    {
        _IsTyping = true;
        foreach (char c in UIText[lineIndex])
        {
            ControlText.text += c;
            yield return new WaitForSeconds(TypeSpeed);
        }
        yield return new WaitForSeconds(_LineDelay);
        _IsTyping = false;
    }

    private void WriteLine()
    {
        if (lineIndex < UIText.Length) 
        {
            ControlText.text = "";

            coroutine = WriteLetters();
            StartCoroutine(coroutine);
            lineIndex++;
        }
        else
        {
            ControlBox.SetActive (false);

            _IsTalking = false;
        }
    }
    */
    private void OnTriggerEnter(Collider collision)
    {
        if (collision.CompareTag("Player") && !_IsActive)
        {
            _InRange = true;
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        if (collision.CompareTag("Player"))
        {
            StopCoroutine(coroutine);
            StopTalking();
            _InRange = false;

        }
    }
}